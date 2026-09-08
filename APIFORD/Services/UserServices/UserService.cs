using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.User;
using APIFORD.Model.User;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace APIFORD.Services.UserServices;

public class UserService
{
    private IMapper _mapper;
    protected readonly FordDbContext _context;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;
    private readonly DoisFatoresService _doisFatoresService;
    private readonly IConfiguration _configuration;


    public UserService
        (IMapper mapper, FordDbContext context, UserManager<User> userManager, IConfiguration configuration,
        SignInManager<User> signInManager, TokenService tokenService, DoisFatoresService doisFatoresService)
    {
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _doisFatoresService = doisFatoresService;
    }

    public async Task CreateUser(CreateUserDTO dto)
    {
        User user = _mapper.Map<User>(dto);

        IdentityResult resultado = await _userManager.CreateAsync(user, dto.Password);


        var erros = resultado.Errors.Select(e => e.Description);

        if (!resultado.Succeeded) throw new ApplicationException($"Falha ao criar usuário, \nError: {erros}");

    }

    public async Task<User> FindUser(string identifier)
    {   
        var a =  _userManager.Users.ToList();
        User user = await _userManager.FindByNameAsync(identifier);
        Console.WriteLine(a);
        Console.WriteLine(user.UserName);

        if (user == null) throw new ApplicationException("Falha ao autentiCarro: Credenciais inválidas.");

        return user;
    }

    private async Task<LoginResponseDTO> CompletarLoginAsync(User user)
    {
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var tokenDesafio = _doisFatoresService.GerarTokenDesafio(user);
            return new LoginResponseDTO { RequerDoisFatores = true, TokenDesafio = tokenDesafio };
        }

        return new LoginResponseDTO { RequerDoisFatores = false, AccessToken = _tokenService.GenerateToken(user) };
    }

    public async Task<LoginResponseDTO> Login(LoginUserDTO dto)
    {
        User user = await FindUser(dto.UserIdentifier);
        var resultado = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!resultado.Succeeded) throw new ApplicationException("Falha ao autenticar usuário");

        return await CompletarLoginAsync(user);
    }

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
            throw new UnauthorizedAccessException("Token do Google inválido.");
        }

        if (!payload.EmailVerified)
            throw new UnauthorizedAccessException("O e-mail dessa conta Google não está verificado.");

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new User { UserName = payload.Email, Email = payload.Email, EmailConfirmed = true };
            var criar = await _userManager.CreateAsync(user); // sem senha — essa conta só loga via Google
            if (!criar.Succeeded)
                throw new ApplicationException(string.Join("; ", criar.Errors.Select(e => e.Description)));

            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
        }

        return await CompletarLoginAsync(user);
    }

    public async Task<LoginResponseDTO> VerificarDoisFatoresELogarAsync(string tokenDesafio, string codigo)
    {
        var user = await _doisFatoresService.ValidarDesafioAsync(tokenDesafio, codigo);
        if (user == null) throw new UnauthorizedAccessException("Código inválido ou expirado.");

        return new LoginResponseDTO { RequerDoisFatores = false, AccessToken = _tokenService.GenerateToken(user) };
    }

    public async Task<HabilitarDoisFatoresResponseDTO> IniciarDoisFatoresAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        var (uri, chave) = await _doisFatoresService.IniciarConfiguracaoAsync(user);
        return new HabilitarDoisFatoresResponseDTO { QrCodeUri = uri, ChaveManual = chave };
    }

    public async Task ConfirmarDoisFatoresAsync(string userId, string codigo)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (!await _doisFatoresService.ConfirmarAtivacaoAsync(user, codigo))
            throw new ArgumentException("Código inválido.");
    }

    public async Task DesativarDoisFatoresAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        await _doisFatoresService.DesativarAsync(user);
    }


    public async Task UpdateUser(string id, UpdateUserDTO dto)
    {
        User user = await _userManager.FindByIdAsync(id);

        if (user == null) throw new ApplicationException("Usuário não encontrado.");

        _mapper.Map(dto, user);
        
        IdentityResult resultado = await _userManager.UpdateAsync(user);
        var erros = resultado.Errors.Select(e => e.Description);
        if (!resultado.Succeeded) throw new ApplicationException($"Falha ao atualizar usuário, \nError: {erros}");
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public List<ShowUserDTO> GetUsers()
    {
        List<User> users = _userManager.Users.ToList();
        return _mapper.Map<List<ShowUserDTO>>(users); ;
    }


    public async Task SoftDelete(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        user.Excluido = true;
        var resultado = await _userManager.UpdateAsync(user);
        if (!resultado.Succeeded)
        {
            var erros = string.Join(" | ", resultado.Errors.Select(e => e.Description));
            throw new ApplicationException($"Falha ao deletar usuário. Erro: {erros}");
        }

    }
    public async Task AnonymizeAsync(string id)
    {
        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        // Usamos Reflexão para varrer todas as propriedades e alterar os valores
        var properties = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.Name != "Id"); // Mantém o ID intacto

        foreach (var prop in properties)
        {
            if (prop.Name == "IsDeleted") prop.SetValue(user, true);
            // Se for string, vira "Unknown"
            if (prop.PropertyType == typeof(string)) prop.SetValue(user, "Unknown");
            // Se for número, zera (opcional)
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal)) prop.SetValue(user, 0);
            // Se for data, pode ser nula
            else if (prop.PropertyType == typeof(DateTime?)) prop.SetValue(user, null);

        }


        var resultado = await _userManager.UpdateAsync(user);
        if (!resultado.Succeeded)
        {
            var erros = string.Join(" | ", resultado.Errors.Select(e => e.Description));
            throw new ApplicationException($"Falha ao deletar usuário. Erro: {erros}");
        }

    }



}
