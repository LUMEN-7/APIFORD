using APIFORD.Data.DTOS.Workspace.teams;
using APIFORD.Services.Workspace.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Workpace.Teams;

[ApiController]
[Route("[controller]")]
[Authorize]
public class EquipeController : ControllerBase
{
    private readonly EquipeService _equipeService;
    public EquipeController(EquipeService equipeService) => _equipeService = equipeService;

    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    [HttpPost]
    public async Task<ActionResult<ReadEquipeDTO>> Criar([FromBody] CriarEquipeDTO dto)
        => Ok(await _equipeService.CriarAsync(ObterUsuarioId(), dto.Nome));

    [HttpGet("minhas")]
    public async Task<ActionResult<List<ReadEquipeDTO>>> ListarMinhas()
        => Ok(await _equipeService.ListarMinhasAsync(ObterUsuarioId()));

    [HttpGet("{id}/membros")] // getTeamWorkers
    public async Task<ActionResult<List<ReadMembroDTO>>> ListarMembros(int id)
        => Ok(await _equipeService.ListarMembrosAsync(id, ObterUsuarioId()));

    [HttpPost("{id}/membros")]
    public async Task<IActionResult> AdicionarMembro(int id, [FromBody] AdicionarMembroDTO dto)
    {
        await _equipeService.AdicionarMembroAsync(id, dto.UserId, ObterUsuarioId());
        return NoContent();
    }

    [HttpDelete("{id}/membros/{userId}")]
    public async Task<IActionResult> RemoverMembro(int id, string userId)
    {
        await _equipeService.RemoverMembroAsync(id, userId, ObterUsuarioId());
        return NoContent();
    }
}