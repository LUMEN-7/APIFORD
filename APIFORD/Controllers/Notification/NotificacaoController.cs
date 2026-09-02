using APIFORD.Data.DTOS.Notifications;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Notification;

[ApiController]
[Route("[controller]")]
//[Authorize] // reativado — antes estava comentado
public class NotificacaoController : ControllerBase
{
    private readonly NotificacaoService _notificacaoService;

    public NotificacaoController(NotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    private string ObterUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    [HttpGet("minhas")]
    public async Task<IActionResult> ListarMinhasNotificacoes()
    {
        var result = await _notificacaoService.ListarNotificacoesDoUsuarioAsync(ObterUsuarioId());
        return Ok(result);
    }

    [HttpPatch("{id}/lida")]
    public async Task<IActionResult> MarcarComoLida(int id)
    {
        var sucesso = await _notificacaoService.MarcarComoLidaAsync(id, ObterUsuarioId());
        return Ok(sucesso);
    }

    [HttpPatch("lidas/todas")]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        await _notificacaoService.MarcarTodasComoLidasAsync(ObterUsuarioId());
        return NoContent();
    }

    [HttpPost("criar")]
    //[Authorize(Roles = "Admin")] // além do [Authorize] de classe, exige role de admin
    public async Task<IActionResult> CriarBroadcast(CreateNotificationDTO dto)
    {
        var result = await _notificacaoService.CriarBroadcastAsync(dto);
        return Ok(result);
    }
}