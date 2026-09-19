using APIFORD.Data.DTOS.Workspace;
using APIFORD.Services.Workspace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Workpace;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WorkspaceController : ControllerBase
{

    private readonly WorkspaceService _workspaceService;
    public WorkspaceController(WorkspaceService workspaceService) => _workspaceService = workspaceService;

    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    [HttpPost("posts")]
    public async Task<ActionResult<ReadPostDTO>> Criar([FromBody] CriarPostDTO dto)
        => Ok(await _workspaceService.CriarPostAsync(ObterUsuarioId(), dto));

    [HttpGet("posts")]
    public async Task<ActionResult<List<ReadPostDTO>>> Listar()
        => Ok(await _workspaceService.ListarAsync(ObterUsuarioId()));

    [HttpPost("posts/{id}/comentarios")]
    public async Task<ActionResult<ReadComentarioDTO>> Comentar(int id, [FromBody] CriarComentarioDTO dto)
        => Ok(await _workspaceService.ComentarAsync(id, ObterUsuarioId(), dto.Conteudo));

    [HttpPost("posts/{id}/curtida")]
    public async Task<IActionResult> ToggleCurtida(int id)
        => Ok(new { totalCurtidas = await _workspaceService.ToggleCurtidaAsync(id, ObterUsuarioId()) });

    [HttpPatch("posts/{id}/fixar")]
    public async Task<IActionResult> TogglePin(int id)
    {
        await _workspaceService.TogglePinAsync(id);
        return NoContent();
    }

    [HttpPatch("posts/{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDTO dto)
    {
        await _workspaceService.AtualizarStatusAsync(id, dto.Status);
        return NoContent();
    }

    [HttpDelete("posts/{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _workspaceService.ExcluirAsync(id, ObterUsuarioId());
        return NoContent();
    }
}