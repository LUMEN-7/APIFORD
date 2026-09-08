using APIFORD.Data.DTOS.Annotations;
using APIFORD.Model.User;
using APIFORD.Services.Annotation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Annotation;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AnotacaoController : ControllerBase
{
    private readonly AnotacaoService _anotacaoService;
    public AnotacaoController(AnotacaoService anotacaoService) => _anotacaoService = anotacaoService;
    private string ObterUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpPost]
    public async Task<ActionResult<ReadAnotacaoDTO>> Criar([FromBody] CriarAnotacaoDTO dto)
        => Ok(await _anotacaoService.CriarAsync(ObterUsuarioId(), dto));

    [HttpGet("minhas")]
    public async Task<ActionResult<List<ReadAnotacaoDTO>>> ListarMinhas()
        => Ok(await _anotacaoService.ListarPorUsuarioAsync(ObterUsuarioId()));

    [HttpPut("{id}/blocos")]
    public async Task<ActionResult<ReadAnotacaoDTO>> AtualizarBlocos(int id, [FromBody] AtualizarBlocosDTO dto)
        => Ok(await _anotacaoService.AtualizarBlocosAsync(id, ObterUsuarioId(), dto));


    [HttpPatch("{id}/blocos/{blocoId}")]
    public async Task<IActionResult> AtualizarTextoBloco(int id, string blocoId, [FromBody] AtualizarTextoBlocoDTO dto)
    {
        await _anotacaoService.AtualizarTextoBlocoAsync(id, ObterUsuarioId(), blocoId, dto.Texto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _anotacaoService.ExcluirAsync(id, ObterUsuarioId());
        return NoContent();
    }
}