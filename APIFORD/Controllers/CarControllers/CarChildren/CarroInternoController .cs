using APIFORD.Data.DTOS;
using APIFORD.Filters;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarControllers.CarChildren;

[ApiController]
[Route("internal/carros")]
[InternalApiKey]
public class CarroInternoController : ControllerBase
{
    private readonly NotificacaoService _notificacaoService;

    public CarroInternoController(NotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    [HttpPost("notificar-atualizacao")]
    public async Task<IActionResult> NotificarAtualizacao([FromBody] NotificarAtualizacaoInternoDTO dto)
    {
        await _notificacaoService.NotificarAtualizacaoCarroAsync(dto.LinhagemId, dto.Marca, dto.Modelo);
        return NoContent();
    }
}