using APIFORD.Data.DTOS.CarrosDto;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

using APIFORD.Services.CarroServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;

namespace APIFORD.Controllers.CarroControllers;


[ApiController]
[Route("[controller]")]
public class CarroController : BaseController<Model.CarroClasses.Carro, CreateCarroDTO, ReadCarroDTO, UpdateCarroDTO, int>
{
    private readonly CarroService _CarroService;

    public CarroController(CarroService CarroService): base(CarroService)
    {
        _CarroService = CarroService;
    }

    [HttpGet("{linhagemId}")]
    public async Task<ActionResult<ReadCarroDTO>> ObterVersaoMaisRecente(int linhagemId)
    {
        var dto = await _CarroService.ObterVersaoMaisRecenteAsync(linhagemId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{linhagemId}/versoes")]
    public async Task<ActionResult<List<VersaoResumoDTO>>> ListarVersoes(int linhagemId)
    {
        return Ok(await _CarroService.ListarVersoesAsync(linhagemId));
    }

    /// <summary>
    /// Edita propriedades específicas do JSONB do carro dinamicamente.
    /// </summary>
    /// <param name="id">ID do Carro</param>
    /// <param name="alteracoes">Dicionário de alterações (Ex: {"Categoria": "SUV"})</param>
    [HttpPatch("edicao-admin/{id}")]
    // [Authorize(Roles = "Admin")] // Descomente quando integrar a autenticação
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarPropriedades(int id, [FromBody] Dictionary<string, object> alteracoes)
    {
        try
        {
            // Em produção, extrairíamos o ID do admin direto do Token JWT (User.Claims)
            int simulacaoAdminId = 999;

            if (alteracoes == null || alteracoes.Count == 0)
                return BadRequest(new { message = "Nenhuma alteração enviada no payload." });

            var carroAtualizado = await _CarroService.EditarPropriedadesAdminAsync(id, alteracoes, simulacaoAdminId);

            return Ok(carroAtualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao processar as alterações.", detalhe = ex.Message });
        }
    }


}
