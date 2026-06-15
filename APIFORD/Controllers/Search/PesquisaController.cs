// APIFORD/Controllers/Search/PesquisaController.cs
using APIFORD.Data.DTOS.Search;
using APIFORD.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Search;

[ApiController]
[Route("[controller]")]
public class PesquisaController : ControllerBase
{
    private readonly PesquisaService _pesquisaService;

    public PesquisaController(PesquisaService pesquisaService)
    {
        _pesquisaService = pesquisaService;
    }

    /// <summary>
    /// Inicia uma busca de especificações. Retorna job_id imediatamente.
    /// Use GET /Pesquisa/jobs/{id} para acompanhar o status.
    /// </summary>
    [HttpPost("busca")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Busca([FromBody] BuscaDTO dto)
    {
        try
        {
            var jobId = await _pesquisaService.IniciarBusca(dto);
            return Accepted(new { job_id = jobId });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(503, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retorna o status de um job de busca.
    /// status: pending | running | done | error | not_found
    /// Quando done, inclui o carro salvo no campo "carro".
    /// </summary>
    [HttpGet("jobs/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChecarJob(Guid id)
    {
        var resultado = await _pesquisaService.ChecarJob(id);

        if (resultado.Status == "not_found")
            return NotFound(new { error = "Job não encontrado." });

        return Ok(resultado);
    }
}