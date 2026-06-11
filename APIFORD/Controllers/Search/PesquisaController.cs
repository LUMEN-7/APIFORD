using APIFORD.Data.DTOS.Search;
using APIFORD.Services.CarroServices;
using APIFORD.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Search;


[ApiController]
[Route("[controller]")]
public class PesquisaController : ControllerBase
{
    private readonly PesquisaService _pesquisaService;

    public PesquisaController(PesquisaService PesquisaService)
    {
        _pesquisaService = PesquisaService;
    }


    [HttpPost("busca")]
    public  async Task<IActionResult> Busca([FromBody] BuscaDTO dto)
    {
        //var result = await _pesquisaService.Busca(dto);
        return Ok();
    }


}
