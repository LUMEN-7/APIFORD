using APIFORD.Data.DTOS.Schedule;
using APIFORD.Model.Schedule;
using APIFORD.Model.User;
using APIFORD.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Schedule;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class EscutaLancamentoController : ControllerBase
{
    private readonly EscutaLancamentoService _escutaService;

    public EscutaLancamentoController(EscutaLancamentoService escutaService)
        => _escutaService = escutaService;

    private string ObterUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarEscutaLancamentoDTO dto)
    {
        await _escutaService.CriarOuEntrarAsync(ObterUsuarioId(), dto.Marca, dto.Modelo, dto.Ano, dto.ExpiraEm);
        return NoContent();
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<List<EscutaLancamento>>> ListarMinhas()
        => Ok(await _escutaService.ListarPorUsuarioAsync(ObterUsuarioId()));

    [HttpDelete("{escutaId}")]
    public async Task<IActionResult> SairDaEscuta(int escutaId)
    {
        await _escutaService.SairAsync(escutaId, ObterUsuarioId());
        return NoContent();
    }
}
