using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Services.Comparison;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Comparison;

[ApiController]
[Route("api/[controller]")]
public class ComparacaoController : ControllerBase
{
    private readonly ComparacaoService _comparacaoService;

    public ComparacaoController(ComparacaoService comparacaoService)
    {
        _comparacaoService = comparacaoService;
    }

    [HttpPost("analisar")]
    public async Task<IActionResult> AnalisarConcorrencia([FromBody] ComparacaoRequestDTO dto)
    {
        //var resultado = await _comparacaoService.GerarComparacaoAsync(dto);
        //if (resultado == null) return NotFound("Carro base não encontrado.");

        return Ok();
    }
}
