using APIFORD.Data.DTOS.Export;
using APIFORD.Services.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Export;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class ExportacaoController : ControllerBase
{
    private readonly ExportacaoService _exportacaoService;

    public ExportacaoController(ExportacaoService exportacaoService) => _exportacaoService = exportacaoService;

[HttpGet("conflitos")]
public async Task<ActionResult<List<ConflitoCampoDTO>>> ObterConflitos([FromQuery] List<ItemExportacaoDTO> itens)
{
    try { return Ok(await _exportacaoService.ObterConflitosAsync(itens)); }
    catch (ArgumentException ex) { return BadRequest(ex.Message); }
}

    [HttpPost]
    public async Task<IActionResult> Exportar([FromBody] ExportarRequestDTO request)
    {
        var (arquivo, contentType, nomeArquivo) = await _exportacaoService.ExportarAsync(request);
        return File(arquivo, contentType, nomeArquivo);
    }
}
