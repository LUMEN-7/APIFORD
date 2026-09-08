using APIFORD.Data.DTOS.Notifications;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Notification;

/// <summary>
/// Controller responsável por expor os endpoints de gerenciamento de notificações do usuário
/// autenticado (listagem, marcação de leitura) e pela criação de broadcasts de notificação
/// (restrita a administradores).
/// </summary>
[ApiController]
[Route("[controller]")]
//[Authorize] // reativado — antes estava comentado
public class NotificacaoController : ControllerBase
{
    private readonly NotificacaoService _notificacaoService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="NotificacaoController"/>.
    /// </summary>
    /// <param name="notificacaoService">Serviço responsável pelas regras de negócio de notificações.</param>
    public NotificacaoController(NotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    /// <summary>
    /// Obtém o Id do usuário autenticado a partir das claims da requisição atual.
    /// </summary>
    /// <returns>O Id do usuário autenticado, ou uma string vazia caso a claim não esteja presente.</returns>
    private string ObterUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    /// <summary>
    /// Lista todas as notificações do usuário atualmente autenticado.
    /// </summary>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com a lista de notificações do usuário.</returns>
    [HttpGet("minhas")]
    [ProducesResponseType(typeof(List<ReadNotificationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarMinhasNotificacoes()
    {
        var result = await _notificacaoService.ListarNotificacoesDoUsuarioAsync(ObterUsuarioId());
        return Ok(result);
    }

    /// <summary>
    /// Marca uma notificação específica do usuário autenticado como lida.
    /// </summary>
    /// <param name="id">Id da notificação a ser marcada como lida.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com um booleano indicando se a operação teve sucesso.</returns>
    [HttpPatch("{id}/lida")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarcarComoLida(int id)
    {
        var sucesso = await _notificacaoService.MarcarComoLidaAsync(id, ObterUsuarioId());
        return Ok(sucesso);
    }

    /// <summary>
    /// Marca todas as notificações não lidas do usuário autenticado como lidas.
    /// </summary>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após a operação ser concluída.</returns>
    [HttpPatch("lidas/todas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        await _notificacaoService.MarcarTodasComoLidasAsync(ObterUsuarioId());
        return NoContent();
    }

    /// <summary>
    /// Cria e dispara um broadcast de notificação para os destinatários definidos no DTO.
    /// Endpoint restrito a usuários com role de administrador.
    /// </summary>
    /// <param name="dto">Dados da notificação a ser criada e distribuída.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com os dados da notificação criada.</returns>
    [HttpPost("criar")]
    //[Authorize(Roles = "Admin")] // além do [Authorize] de classe, exige role de admin
    [ProducesResponseType(typeof(ReadNotificationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarBroadcast(CreateNotificationDTO dto)
    {
        var result = await _notificacaoService.CriarBroadcastAsync(dto);
        return Ok(result);
    }
}