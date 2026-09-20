using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.User;
using APIFORD.Middleware;
using APIFORD.Model.User;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Reflection;

namespace APIFORD.Services.UserServices;


/// <summary>
/// Regras de negócio de conta de usuário: cadastro, autenticação (senha, Google, 2FA),
/// atualização de perfil e as duas formas de "remoção" — soft delete (marca como excluído,
/// mantém os dados) e anonimização (mantém a linha, mas apaga os dados pessoais).
/// </summary>
public class UserService
{
    private IMapper _mapper;
    protected readonly FordDbContext _context;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;
    private readonly DoisFatoresService _doisFatoresService;
    private readonly IConfiguration _configuration;
    private readonly IArmazenamentoService _armazenamentoService;
    private readonly IEmailSenderService _emailSender; // isso aqui


    public UserService
        (IMapper mapper, FordDbContext context, UserManager<User> userManager, IConfiguration configuration,
        SignInManager<User> signInManager, TokenService tokenService, DoisFatoresService doisFatoresService,
        IArmazenamentoService armazenamentoService, IEmailSenderService emailSender)
    {
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _doisFatoresService = doisFatoresService;
        _armazenamentoService = armazenamentoService;
        _emailSender = emailSender;
    }


    /// <summary>
    /// Cria uma nova conta de usuário e já retorna o token de autenticação
    /// (contas criadas por cadastro direto não têm 2FA habilitado por padrão).
    /// </summary>
    /// <param name="dto">Dados de cadastro (username, e-mail e senha).</param>
    /// <returns>O token JWT da nova conta.</returns>
    /// <exception cref="ConflictException">Já existe uma conta com esse username ou e-mail.</exception>
    /// <exception cref="ValidationException">Um ou mais campos não passaram nas regras do Identity (ex: senha fraca).</exception>
    public async Task<LoginResponseDTO> CreateUser(CreateUserDTO dto)
    {
        User user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            NomeExibicao = dto.Nome,
            FotoPerfilUrl = dto.FotoPerfilUrl
        };

        IdentityResult resultado = await _userManager.CreateAsync(user, dto.Password);


        var erros = resultado.Errors.Select(e => e.Description);

        
        LancarSeIdentityFalhou(resultado);
        return new LoginResponseDTO { RequerDoisFatores = false, AccessToken = await _tokenService.GenerateTokenAsync(user), Usuario = await MontarResumoAsync(user) };

    }

    /// <summary>
    /// Busca um usuário por username ou e-mail (aceita qualquer um dos dois no mesmo campo).
    /// </summary>
    /// <param name="identifier">Username ou e-mail do usuário.</param>
    /// <exception cref="NotFoundException">Nenhum usuário encontrado com esse identificador.</exception>
    public async Task<User> FindUser(string identifier)
    {   
        
        User user = await _userManager.FindByNameAsync(identifier) ?? await _userManager.FindByEmailAsync(identifier);

        if (user == null) throw new NotFoundException("Usuario não Encontrado");

        return user;
    }

    /// <summary>
    /// Monta o resumo público de um usuário — dados seguros pra expor ao front (sem hash de senha
    /// ou qualquer campo sensível da entidade <see cref="User"/>). Usado para embutir os dados básicos
    /// do usuário logado dentro do <see cref="LoginResponseDTO"/>, evitando uma chamada separada
    /// do front só para "quem sou eu" depois do login.
    /// </summary>
    /// <param name="user">Usuário já carregado do banco.</param>
    /// <returns>Um <see cref="UsuarioResumoDTO"/> com os dados públicos do usuário.</returns>
    private async Task<UsuarioResumoDTO> MontarResumoAsync(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        Name = user.NomeExibicao,
        FotoPerfilUrl = user.FotoPerfilUrl,
        DoisFatoresAtivo = await _userManager.GetTwoFactorEnabledAsync(user)
    };

    /// <summary>
    /// Ponto único de saída de todo fluxo de login (senha, Google): decide se o usuário
    /// precisa passar por 2FA ou se já pode receber o token de acesso direto.
    /// </summary>
    /// <param name="user">Usuário já autenticado por algum meio (senha verificada, Google verificado, etc.).</param>
    /// <returns>Um desafio de 2FA (se habilitado) ou o token JWT de acesso.</returns>
    private async Task<LoginResponseDTO> CompletarLoginAsync(User user)
    {
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var tokenDesafio = _doisFatoresService.GerarTokenDesafio(user);
            return new LoginResponseDTO { RequerDoisFatores = true, TokenDesafio = tokenDesafio };
        }
        
        return new LoginResponseDTO { RequerDoisFatores = false, AccessToken = await _tokenService.GenerateTokenAsync(user), Usuario = await MontarResumoAsync(user) };
    }

    /// <summary>
    /// Autentica por username/e-mail + senha.
    /// </summary>
    /// <param name="dto">Identificador (username ou e-mail) e senha.</param>
    /// <returns>Um desafio de 2FA (se habilitado) ou o token JWT de acesso.</returns>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="UnauthorizedException">Senha incorreta.</exception>
    public async Task<LoginResponseDTO> Login(LoginUserDTO dto)
    {
        User user = await FindUser(dto.UserIdentifier);
        var resultado = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!resultado.Succeeded) throw new UnauthorizedException("Credenciais inválidas.");

        return await CompletarLoginAsync(user);
    }

    /// <summary>
    /// Autentica via Google Sign-In. Se for o primeiro acesso dessa conta Google, cria o usuário
    /// automaticamente (sem senha — a conta só pode logar por esse método).
    /// </summary>
    /// <param name="idTokenGoogle">ID Token emitido pelo Google, a ser validado contra o Client ID configurado.</param>
    /// <returns>Um desafio de 2FA (se habilitado) ou o token JWT de acesso.</returns>
    /// <exception cref="UnauthorizedException">Token do Google inválido, ou e-mail da conta Google não verificado.</exception>
    /// <exception cref="ConflictException">Já existe uma conta com esse e-mail (corrida rara entre duas requisições simultâneas).</exception>
    /// <exception cref="ValidationException">Falha ao criar a conta a partir dos dados do Google.</exception>
    public async Task<LoginResponseDTO> LoginComGoogleAsync(string idTokenGoogle)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idTokenGoogle, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["GoogleClientId"] }
            });
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedException("Token do Google inválido.");
        }


        if (!payload.EmailVerified)
            throw new UnauthorizedException("O e-mail dessa conta Google não está verificado.");

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            var nomeExibicao = !string.IsNullOrWhiteSpace(payload.Name)
            ? payload.Name
            : payload.Email.Split('@')[0]; // fallback: prefixo do e-mail quando o Google não manda o nome

            user = new User
            {
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = true,
                NomeExibicao = nomeExibicao,
                FotoPerfilUrl = payload.Picture, // pode continuar null — não tem a mesma restrição NOT NULL no banco
            };
            
            var criar = await _userManager.CreateAsync(user); // sem senha — essa conta só loga via Google

            LancarSeIdentityFalhou(criar);

            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
        }

        return await CompletarLoginAsync(user);
    }

    /// <summary>
    /// Conclui o login de um usuário com 2FA habilitado, validando o código contra o token de desafio
    /// emitido em <see cref="CompletarLoginAsync"/>.
    /// </summary>
    /// <param name="tokenDesafio">Token de desafio recebido no login inicial.</param>
    /// <param name="codigo">Código de 6 dígitos gerado pelo app autenticador.</param>
    /// <returns>O token JWT de acesso.</returns>
    /// <exception cref="UnauthorizedException">Token de desafio ou código inválido/expirado.</exception>
    public async Task<LoginResponseDTO> VerificarDoisFatoresELogarAsync(string tokenDesafio, string codigo)
    {
        var user = await _doisFatoresService.ValidarDesafioAsync(tokenDesafio, codigo);
        if (user == null) throw new UnauthorizedException("Código inválido ou expirado.");

        return new LoginResponseDTO { RequerDoisFatores = false, AccessToken = await _tokenService.GenerateTokenAsync(user), Usuario = await MontarResumoAsync(user) };
    }

    /// <summary>
    /// Inicia a configuração de 2FA, gerando o QR Code (e a chave manual equivalente) para
    /// cadastro num app autenticador. Não ativa o 2FA — isso só acontece em <see cref="ConfirmarDoisFatoresAsync"/>.
    /// </summary>
    /// <param name="userId">Id do usuário logado.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    public async Task<HabilitarDoisFatoresResponseDTO> IniciarDoisFatoresAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        var (uri, chave) = await _doisFatoresService.IniciarConfiguracaoAsync(user);
        return new HabilitarDoisFatoresResponseDTO { QrCodeUri = uri, ChaveManual = chave };
    }

    /// <summary>
    /// Confirma a ativação do 2FA, validando o primeiro código gerado a partir do QR Code cadastrado.
    /// </summary>
    /// <param name="userId">Id do usuário logado.</param>
    /// <param name="codigo">Código de 6 dígitos gerado pelo app autenticador.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="BadRequestException">Código inválido.</exception>
    public async Task ConfirmarDoisFatoresAsync(string userId, string codigo)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (!await _doisFatoresService.ConfirmarAtivacaoAsync(user, codigo))
            throw new ArgumentException("Código inválido.");
    }

    /// <summary>
    /// Desativa o 2FA do usuário.
    /// </summary>
    /// <param name="userId">Id do usuário logado.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    public async Task DesativarDoisFatoresAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        await _doisFatoresService.DesativarAsync(user);
    }


    /// <summary>
    /// Atualiza parcialmente os dados de perfil do usuário — só os campos preenchidos no DTO são alterados.
    /// </summary>
    /// <param name="id">Id do usuário.</param>
    /// <param name="dto">Campos a atualizar (username, e-mail).</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="ConflictException">O novo username ou e-mail já pertence a outra conta.</exception>
    public async Task UpdateUser(string id, UpdateUserDTO dto)
    {
        User user = await _userManager.FindByIdAsync(id) ?? throw new NotFoundException("Usuário não encontrado."); ;

        if (dto.NomeExibicao != null) LancarSeIdentityFalhou(await _userManager.SetUserNameAsync(user, dto.NomeExibicao));
        
        if (dto.Email != null) LancarSeIdentityFalhou(await _userManager.SetEmailAsync(user, dto.Email)); 
    }

    /// <summary>
    /// Troca a senha do usuário, validando a senha atual antes de aplicar a nova.
    /// </summary>
    /// <param name="userId">Id do usuário logado.</param>
    /// <param name="senhaAtual">Senha atual, exigida pelo Identity como prova de posse da conta.</param>
    /// <param name="senhaNova">Nova senha, validada pela política de senha do Identity (tamanho, complexidade etc.).</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="UnauthorizedException">Senha atual incorreta.</exception>
    /// <exception cref="ValidationException">A nova senha não atende à política de senha do Identity.</exception>
    public async Task TrocarSenhaAsync(string userId, string senhaAtual, string senhaNova)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("Usuário não encontrado.");

        LancarSeIdentityFalhou(await _userManager.ChangePasswordAsync(user, senhaAtual, senhaNova));
    }

    /// <summary>
    /// Gera um código de verificação e envia por e-mail para iniciar a redefinição de senha.
    /// </summary>
    /// <param name="email">E-mail da conta que deseja redefinir a senha.</param>
    /// <exception cref="NotFoundException">Nenhuma conta encontrada com esse e-mail.</exception>
    public async Task EsqueciSenhaAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("Nenhuma conta encontrada com esse e-mail.");

        var codigo = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

        await _emailSender.EnviarAsync(user.Email, "Código de redefinição de senha", $"Seu código de verificação é: {codigo}");
    }

    /// <summary>
    /// Redefine a senha do usuário a partir do código de verificação enviado por e-mail, sem exigir a senha atual.
    /// </summary>
    /// <param name="email">E-mail da conta.</param>
    /// <param name="codigo">Código de verificação recebido por e-mail.</param>
    /// <param name="senhaNova">Nova senha, validada pela política de senha do Identity.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="UnauthorizedException">Código de verificação inválido ou expirado.</exception>
    /// <exception cref="ValidationException">A nova senha não atende à política de senha do Identity.</exception>
    public async Task RedefinirSenhaComCodigoAsync(string email, string codigo, string senhaNova)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("Usuário não encontrado.");

        var codigoValido = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", codigo);
        if (!codigoValido) throw new UnauthorizedException("Código inválido ou expirado.");

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        LancarSeIdentityFalhou(await _userManager.ResetPasswordAsync(user, resetToken, senhaNova));
    }

    /// <summary>
    /// Encerra a sessão do usuário logado.
    /// </summary>
    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Lista todos os usuários cadastrados (endpoint administrativo).
    /// </summary>
    public List<ShowUserDTO> GetUsers()
    {
        List<User> users = _userManager.Users.ToList();
        return _mapper.Map<List<ShowUserDTO>>(users); ;
    }


    /// <summary>
    /// Marca o usuário como excluído sem apagar seus dados (exclusão reversível/lógica).
    /// Use <see cref="AnonymizeAsync"/> quando for necessário apagar os dados pessoais de fato
    /// (ex: solicitação de LGPD).
    /// </summary>
    /// <param name="id">Id do usuário.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="ValidationException">Falha ao persistir a exclusão.</exception>
    public async Task SoftDelete(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        user.Excluido = true;
        var resultado = await _userManager.UpdateAsync(user);
        LancarSeIdentityFalhou(resultado);

    }

    /// <summary>
    /// Apaga os dados pessoais do usuário via reflection (todo campo string vira "Unknown", número
    /// vira 0, data anulável vira null), preservando o Id, e marca a conta como excluída.
    /// Diferente de <see cref="SoftDelete"/>: aqui os dados originais não são recuperáveis.
    /// </summary>
    /// <param name="id">Id do usuário.</param>
    /// <exception cref="NotFoundException">Usuário não encontrado.</exception>
    /// <exception cref="ValidationException">Falha ao persistir a anonimização.</exception>
    public async Task AnonymizeAsync(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        // Usamos Reflexão para varrer todas as propriedades e alterar os valores
        var properties = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.Name != "Id"); // Mantém o ID intacto

        foreach (var prop in properties)
        {
            if (prop.Name == "Excluido") prop.SetValue(user, true);
            // Se for string, vira "Unknown"
            if (prop.PropertyType == typeof(string)) prop.SetValue(user, "Unknown");
            // Se for número, zera (opcional)
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal)) prop.SetValue(user, 0);
            // Se for data, pode ser nula
            else if (prop.PropertyType == typeof(DateTime?)) prop.SetValue(user, null);

        }


        var resultado = await _userManager.UpdateAsync(user);
        LancarSeIdentityFalhou(resultado);

    }

    /// <summary>
    /// Traduz um <see cref="IdentityResult"/> falho para a exception certa, por categoria de erro:
    /// duplicidade e concorrência viram <see cref="ConflictException"/> (409); formato/política de senha
    /// vira <see cref="ValidationException"/> (422); qualquer código não reconhecido vira
    /// <see cref="InternalServerErrorException"/> (500), como sinal de que é um caso novo que essa
    /// tradução ainda não cobre. Não faz nada se o resultado teve sucesso.
    /// </summary>
    /// <param name="resultado">Resultado retornado por uma operação do <see cref="UserManager{TUser}"/>.</param>
    /// <exception cref="ConflictException">Username/e-mail já em uso, ou conflito de concorrência.</exception>
    /// <exception cref="ValidationException">Um ou mais campos não passam nas regras do Identity.</exception>
    /// <exception cref="InternalServerErrorException">Código de erro do Identity não mapeado.</exception>
    private static void LancarSeIdentityFalhou(IdentityResult resultado)
    {
        if (resultado.Succeeded) return;

        var erros = resultado.Errors.ToList();

        // 409 - o valor é válido, só que já pertence a outra conta
        var duplicados = erros.Where(e => e.Code is "DuplicateUserName" or "DuplicateEmail").ToList();
        if (duplicados.Any())
            throw new ConflictException(string.Join(" ", duplicados.Select(e => e.Description)));

        // 409 - outra requisição alterou o registro entre a leitura e a gravação
        if (erros.Any(e => e.Code == "ConcurrencyFailure"))
            throw new ConflictException("O usuário foi alterado por outra requisição. Tente novamente.");
        // 401 - senha atual incorreta
        if (erros.Any(e => e.Code == "PasswordMismatch"))
            throw new UnauthorizedException("Senha atual incorreta.");

        // 422 - formato de username/e-mail ou política de senha não atendida
        var validacao = erros.Where(e =>
            e.Code is "InvalidUserName" or "InvalidEmail" || e.Code.StartsWith("Password")).ToList();
        if (validacao.Any())
            throw new ValidationException(validacao.ToDictionary(e => e.Code, e => new[] { e.Description }));

        // 500 - código de erro do Identity que essa tradução ainda não conhece
        var mensagens = string.Join("; ", erros.Select(e => $"{e.Code}: {e.Description}"));
        throw new InternalServerErrorException($"Falha inesperada do Identity: {mensagens}");
    }

    public async Task<string?> AtualizarFotoPerfilAsync(string userId, IFormFile arquivo, string fileName, string contentType)
    {
        if (arquivo.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Imagem muito grande. Máximo de 5MB.");

        var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!tiposPermitidos.Contains(contentType))
            throw new ArgumentException("Formato não suportado. Use JPEG, PNG ou WebP.");

        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (!string.IsNullOrEmpty(user.FotoPerfilUrl))
            await _armazenamentoService.ExcluirArquivoAsync(user.FotoPerfilUrl);

        user.FotoPerfilUrl = await _armazenamentoService.SalvarArquivoAsync(arquivo.OpenReadStream(), fileName, contentType);
        await _userManager.UpdateAsync(user);
        return user.FotoPerfilUrl;
    }
    public async Task RemoverFotoPerfilAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (!string.IsNullOrEmpty(user.FotoPerfilUrl))
        {
            await _armazenamentoService.ExcluirArquivoAsync(user.FotoPerfilUrl);
            user.FotoPerfilUrl = null;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<bool> EhAdminAsync(Func<string> obterUsuarioId)
    {
        return _context.Hasro
    }
}
