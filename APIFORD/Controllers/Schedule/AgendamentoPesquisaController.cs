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
public class AgendamentoPesquisaController : ControllerBase
{
    private readonly AgendamentoPesquisaService _agendamentoService;

    public AgendamentoPesquisaController(AgendamentoPesquisaService agendamentoService)
        => _agendamentoService = agendamentoService;

    private string ObterUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpPost]
    public async Task<ActionResult<AgendamentoPesquisa>> Criar([FromBody] CriarAgendamentoDTO dto)
        => Ok(await _agendamentoService.CriarAsync(ObterUsuarioId(), dto));

    [HttpGet("meus")]
    public async Task<ActionResult<List<AgendamentoPesquisa>>> ListarMeus()
        => Ok(await _agendamentoService.ListarPorUsuarioAsync(ObterUsuarioId()));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await _agendamentoService.CancelarAsync(id, ObterUsuarioId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
