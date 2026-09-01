using APIFORD.Data.DTOS.Notifications;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Notification;

[ApiController]
[Route("[controller]")]
//[Authorize] // Obriga estar logado
public class NotificacaoController : BaseController<Model.Notificacao, CreateNotificationDTO, ReadNotificationDTO, UpdateNotificationDTO, int>
{
    private readonly NotificacaoService _notificacaoService;

    public NotificacaoController(NotificacaoService notificacaoService) : base(notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    private string ObterUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    [HttpPost("criar")]
    public async Task<IActionResult> CriarNotificacao(CreateNotificationDTO dto)
    {
        dto.userId = ObterUsuarioId();
        var result = await _notificacaoService.CreateAsync(dto);
        return Ok(result);
    }

    // ==========================================
    // ENDPOINTS ESPECÍFICOS DE NEGÓCIO
    // ==========================================

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
        //return NoContent(); // 204 Sucesso sem conteúdo
    }

    [HttpPatch("lidas/todas")]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        await _notificacaoService.MarcarTodasComoLidasAsync(ObterUsuarioId());
        return NoContent();
    }
}