using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Data.DTOS.Comparison.Direct;
using APIFORD.Services.Comparison;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Comparison;

[ApiController]
[Route("[controller]")]
public class ComparacaoController : ControllerBase
{
    private readonly ComparacaoService _comparacaoService;

    public ComparacaoController(ComparacaoService comparacaoService)
    {
        _comparacaoService = comparacaoService;
    }

    [HttpPost("grupo")]
    public async Task<IActionResult> AnalisarGrupo([FromBody] ComparacaoRequestDTO dto)
    {
        var resultado = await _comparacaoService.GerarComparacaoEmGrupoAsync(dto);
        if (resultado == null) return NotFound("Carro base não encontrado.");

        return Ok(resultado);
    }

    [HttpPost("direta")]
    public async Task<IActionResult> AnalisarDireta([FromBody] ComparacaoDiretaRequestDTO dto)
    {
        try
        {
            var resultado = await _comparacaoService.GerarComparacaoDiretaAsync(dto);
            if (resultado == null) return BadRequest("Não foi possível encontrar os veículos informados.");

            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}