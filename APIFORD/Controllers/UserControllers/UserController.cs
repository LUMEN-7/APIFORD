using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.User;
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

    private string ObterUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
    {
        await _userService.CreateUser(dto);
        return Ok("Usuário criado com sucesso");
    }

    /// <summary>
    /// Realiza a autenticação do usuário e gera o token JWT.
    /// </summary>
    /// <param name="dto">As credenciais do usuário (Username/Email e Password).</param>
    /// <returns>O Token JWT de autenticação para as próximas requisições.</returns>
    /// <response code="200">Autenticação realizada com sucesso. Retorna o Token JWT.</response>
    /// <response code="400">Credenciais inválidas informadas.</response>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginUserDTO dto)
        => Ok(await _userService.Login(dto));

    [HttpPost("login/google")]
    public async Task<ActionResult<LoginResponseDTO>> LoginGoogle([FromBody] GoogleLoginDTO dto)
        => Ok(await _userService.LoginComGoogleAsync(dto.IdToken));

    [HttpPost("login/2fa")]
    public async Task<ActionResult<LoginResponseDTO>> VerificarDoisFatores([FromBody] VerificarDoisFatoresDTO dto)
        => Ok(await _userService.VerificarDoisFatoresELogarAsync(dto.TokenDesafio, dto.Codigo));

    [Authorize]
    [HttpPost("2fa/iniciar")]
    public async Task<ActionResult<HabilitarDoisFatoresResponseDTO>> IniciarDoisFatores()
        => Ok(await _userService.IniciarDoisFatoresAsync(ObterUsuarioId()));

    [Authorize]
    [HttpPost("2fa/confirmar")]
    public async Task<IActionResult> ConfirmarDoisFatores([FromBody] ConfirmarAtivacaoDoisFatoresDTO dto)
    {
        await _userService.ConfirmarDoisFatoresAsync(ObterUsuarioId(), dto.Codigo);
        return NoContent();
    }

    [Authorize]
    [HttpPost("2fa/desativar")]
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
        await _userService.UpdateUser(dto.id, dto);
        return Ok("Usuário atualizado com sucesso");
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
