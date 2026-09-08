using APIFORD.Data.DTOS;
using APIFORD.Filters;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarControllers.CarChildren;

/// <summary>
/// Rotas internas, chamadas exclusivamente pela API Python (scraping/IA), nunca pelo front-end.
/// Protegido pelo header X-Internal-Api-Key (ver <see cref="InternalApiKeyAttribute"/>), não por JWT de usuário.
/// </summary>
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

    /// <summary>
    /// Notifica os usuários que favoritaram um veículo de que ele acaba de ser atualizado.
    /// Chamado pela API Python assim que ela termina de salvar uma nova versão do carro no banco.
    /// </summary>
    /// <param name="dto">Linhagem, marca e modelo do veículo atualizado.</param>
    /// <response code="204">Notificações disparadas (ou nenhum usuário interessado — não é erro).</response>
    /// <response code="401">Header X-Internal-Api-Key ausente ou inválido.</response>
    [HttpPost("notificar-atualizacao")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> NotificarAtualizacao([FromBody] NotificarAtualizacaoInternoDTO dto)
    {
        await _notificacaoService.NotificarAtualizacaoCarroAsync(dto.LinhagemId, dto.Marca, dto.Modelo);
        return NoContent();
    }
}