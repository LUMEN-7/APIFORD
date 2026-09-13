using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.User;
using APIFORD.Middleware;
using APIFORD.Model;
using APIFORD.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APIFORD.Controllers.UserControllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    //=============================
    //  Variaveis de abiente
    //=============================

    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    //===============
    //   Gets
    //===============


    /// <summary>
    /// Recupera a lista de todos os usuários cadastrados no sistema.
    /// </summary>
    /// <returns>Uma lista contendo os usuários.</returns>
    /// <response code="200">A lista de usuários foi recuperada com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet("usuarios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers()
    {
        List<ShowUserDTO> users = _userService.GetUsers();
        return Ok(users);
    }

    //[HttpGet("usuarios")] // versao paginada
    //public async Task<IActionResult> GetUsers()
    //{
    //    List<User> users = await _userService.GetUsers();
    //    return Ok(users);
    //}

    //[HttpGet("usuarios")] // versao por id ou filtro
    //public async Task<IActionResult> GetUsers()
    //{
    //    List<User> users = await _userService.GetUsers();
    //    return Ok(users);
    //}


    //===============
    //   Posts
    //===============


    /// <summary>
    /// Realiza o cadastro de um novo usuário no sistema.
    /// </summary>
    /// <param name="dto">Dados necessários para criar o usuário (Username, Email, Password, etc.).</param>
    /// <returns>Uma mensagem confirmando o sucesso do cadastro.</returns>
    /// <response code="200">Usuário criado com sucesso.</response>
    /// <response code="400">Os dados enviados no DTO são inválidos ou as regras de senha falharam.</response>
    [HttpPost("cadastro")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDTO>> CreateUser([FromBody] CreateUserDTO dto)
        => Ok(await _userService.CreateUser(dto));

    /// <summary>
    /// Realiza a autenticação do usuário e gera o token JWT (ou inicia o desafio de 2FA, se habilitado).
    /// </summary>
    /// <param name="dto">As credenciais do usuário (Username/Email e Password).</param>
    /// <returns>O Token JWT de autenticação, ou um token de desafio se o usuário tiver 2FA habilitado.</returns>
    /// <response code="200">Autenticação realizada com sucesso. Retorna o Token JWT ou o desafio de 2FA.</response>
    /// <response code="401">Credenciais inválidas.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginUserDTO dto)
        => Ok(await _userService.Login(dto));

    /// <summary>
    /// Autentica (ou cria, no primeiro acesso) um usuário a partir de um ID Token do Google.
    /// </summary>
    /// <param name="dto">O ID Token emitido pelo Google Sign-In.</param>
    /// <returns>O Token JWT de autenticação, ou um token de desafio se o usuário tiver 2FA habilitado.</returns>
    /// <response code="200">Autenticação realizada com sucesso.</response>
    /// <response code="401">Token do Google inválido, expirado, ou e-mail da conta não verificado.</response>
    [HttpPost("login/google")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDTO>> LoginGoogle([FromBody] GoogleLoginDTO dto)
        => Ok(await _userService.LoginComGoogleAsync(dto.IdToken));

    /// <summary>
    /// Conclui o login de um usuário com 2FA habilitado, validando o código enviado contra o token de desafio.
    /// </summary>
    /// <param name="dto">O token de desafio (recebido no login inicial) e o código de 6 dígitos do app autenticador.</param>
    /// <returns>O Token JWT de autenticação.</returns>
    /// <response code="200">Código válido. Retorna o Token JWT.</response>
    /// <response code="401">Token de desafio ou código inválido/expirado.</response>
    [HttpPost("login/2fa")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDTO>> VerificarDoisFatores([FromBody] VerificarDoisFatoresDTO dto)
        => Ok(await _userService.VerificarDoisFatoresELogarAsync(dto.TokenDesafio, dto.Codigo));

    /// <summary>
    /// Inicia a configuração de 2FA para o usuário logado, gerando o QR Code (e a chave manual)
    /// para cadastro num app autenticador. Não ativa o 2FA ainda — precisa confirmar com um código válido
    /// via <see cref="ConfirmarDoisFatores"/>.
    /// </summary>
    /// <returns>A URI do QR Code e a chave manual equivalente.</returns>
    /// <response code="200">Configuração iniciada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [Authorize]
    [HttpPost("2fa/iniciar")]
    [ProducesResponseType(typeof(HabilitarDoisFatoresResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabilitarDoisFatoresResponseDTO>> IniciarDoisFatores()
        => Ok(await _userService.IniciarDoisFatoresAsync(ObterUsuarioId()));

    /// <summary>
    /// Confirma a ativação do 2FA, validando o primeiro código gerado a partir do QR Code cadastrado.
    /// </summary>
    /// <param name="dto">O código de 6 dígitos gerado pelo app autenticador.</param>
    /// <response code="204">2FA ativado com sucesso.</response>
    /// <response code="400">Código inválido.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [Authorize]
    [HttpPost("2fa/confirmar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmarDoisFatores([FromBody] ConfirmarAtivacaoDoisFatoresDTO dto)
    {
        await _userService.ConfirmarDoisFatoresAsync(ObterUsuarioId(), dto.Codigo);
        return NoContent();
    }

    /// <summary>
    /// Desativa o 2FA do usuário logado.
    /// </summary>
    /// <response code="204">2FA desativado com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [Authorize]
    [HttpPost("2fa/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DesativarDoisFatores()
    {
        await _userService.DesativarDoisFatoresAsync(ObterUsuarioId());
        return NoContent();
    }


    /// <summary>
    /// Realiza o logout do usuário atual no sistema, encerrando a sessão.
    /// </summary>
    /// <returns>Uma mensagem confirmando que o logout foi efetuado.</returns>
    /// <response code="200">Logout realizado com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _userService.Logout();
        return Ok("Logout realizado com sucesso");
    }

    //===============
    //   Updates
    //===============


    /// <summary>
    /// Atualiza as informações de perfil de um usuário existente.
    /// </summary>
    /// <param name="dto">Os novos dados para atualização do usuário.</param>
    /// <returns>Uma mensagem confirmando a atualização.</returns>
    /// <response code="200">Dados do usuário atualizados com sucesso.</response>
    /// <response code="400">Os dados informados para atualização são inválidos.</response>
    /// <response code="404">Usuário não encontrado para o ID informado.</response>
    [HttpPut("atualizar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO dto)
    {
        await _userService.UpdateUser(ObterUsuarioId(), dto);
        return Ok("Usuário atualizado com sucesso");
    }

    
    [HttpPost("foto-perfil")]
    public async Task<IActionResult> AtualizarFotoPerfil(IFormFile arquivo)
    {
        var url = await _userService.AtualizarFotoPerfilAsync(ObterUsuarioId(), arquivo, arquivo.FileName, arquivo.ContentType);
        return Ok(new { fotoPerfilUrl = url });
    }

    
    [HttpDelete("foto-perfil")]
    public async Task<IActionResult> RemoverFotoPerfil()
    {
        await _userService.RemoverFotoPerfilAsync(ObterUsuarioId());
        return NoContent();
    }



    //===============
    //   Deletes
    //===============


    /// <summary>
    /// Inativa logicamente a conta de um usuário (Soft Delete).
    /// </summary>
    /// <param name="id">A chave pAroária do usuário a ser desativado.</param>
    /// <returns>Uma mensagem confirmando a remoção.</returns>
    /// <response code="200">Usuário inativado com sucesso.</response>
    /// <response code="404">Usuário não encontrado para o ID informado.</response>
    [HttpDelete("deletar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        await _userService.SoftDelete(id);
        return Ok("Usuário deletado com sucesso");
    }

    /// <summary>
    /// Executa a anonimização dos dados de um usuário alterando informações sensíveis para "Unknown".
    /// </summary>
    /// <param name="id">A chave pAroária do usuário a ser anonimizado.</param>
    /// <returns>Uma mensagem confirmando a anonimização.</returns>
    /// <response code="200">Usuário anonimizado com sucesso.</response>
    /// <response code="404">Usuário não encontrado para o ID informado.</response>
    [HttpDelete("anonimizar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Anonymize([FromQuery] string id)
    {
        await _userService.AnonymizeAsync(id);
        return Ok("Usuário anonimizado com sucesso");
    }

}
