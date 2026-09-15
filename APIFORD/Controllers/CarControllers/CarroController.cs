using APIFORD.Data.DTOS.CarrosDto;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Middleware;
using APIFORD.Services.CarroServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Implementation;
using System.Security.Claims;
using System.Text.Json;

namespace APIFORD.Controllers.CarroControllers;

/// <summary>
/// Endpoints públicos de consulta de veículos e de edição administrativa das especificações.
/// Cada atualização de um carro gera uma nova versão (linha nova) em vez de sobrescrever a existente —
/// ver <see cref="CarroService.ObterVersaoMaisRecenteAsync"/> para o motivo (preservar histórico de comparações salvas).
/// </summary>
[ApiController]
[Route("[controller]")]
public class CarroController : BaseController<Model.CarroClasses.Carro, CreateCarroDTO, ReadCarroDTO, UpdateCarroDTO, int>
{
    private readonly CarroService _CarroService;

    public CarroController(CarroService CarroService) : base(CarroService)
    {
        _CarroService = CarroService;
    }

    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    /// <summary>
    /// Retorna a versão mais recente de um veículo dentro de uma linhagem
    /// (ex: a última atualização de dados do "Mustang 2025", não necessariamente a mais antiga cadastrada).
    /// </summary>
    /// <param name="linhagemId">Id da linhagem do veículo (agrupa todas as versões de um mesmo modelo/ano).</param>
    /// <response code="200">Versão mais recente encontrada.</response>
    /// <response code="404">Nenhuma versão encontrada para essa linhagem.</response>
    [HttpGet("recente/{linhagemId}")]
    [ProducesResponseType(typeof(ReadCarroDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadCarroDTO>> ObterVersaoMaisRecente(int linhagemId)
    {
        var dto = await _CarroService.ObterVersaoMaisRecenteAsync(linhagemId);
        return Ok(dto);
    }

    [HttpGet("versao/{carroId}")]
    public async Task<ActionResult<ReadCarroDTO>> ObterVersaoEspecifica(int carroId)
    {
        var carro = await _CarroService.ObterPorIdExatoAsync(carroId);
        return carro == null ? NotFound() : Ok(carro);
    }

    [HttpGet("Imagem-Carro/{carroId}")]
    public async Task<ActionResult<string>> GetImagemById(int carroId)
        =>Ok(new { ImagemUrl = await _CarroService.GetImagemByIdAsync(carroId) });
    

    /// <summary>
    /// Lista o histórico de versões de uma linhagem, da mais recente para a mais antiga.
    /// A mais recente vem marcada com <c>EhVersaoAtual = true</c>.
    /// </summary>
    /// <param name="linhagemId">Id da linhagem do veículo.</param>
    [HttpGet("{linhagemId}/versoes")]
    [ProducesResponseType(typeof(List<VersaoResumoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VersaoResumoDTO>>> ListarVersoes(int linhagemId)
    {
        return Ok(await _CarroService.ListarVersoesAsync(linhagemId));
    }

    /// <summary>
    /// Edita propriedades específicas do JSONB do carro dinamicamente.
    /// </summary>
    /// <param name="id">ID do Carro</param>
    /// <param name="alteracoes">Dicionário de alterações (Ex: {"Categoria": "SUV"})</param>
    /// <remarks>
    /// Não sobrescreve o carro original — cria uma nova versão com <c>VersaoAnteriorId</c> apontando pra atual,
    /// preservando o histórico. Cada valor alterado é registrado como uma nova fonte "Edição Manual"
    /// (confiança 1.0) dentro do envelope <see cref="Model.CarroClasses.PropriedadeScraping{T}"/> da propriedade.
    /// </remarks>
    [HttpPatch("edicao-admin/{id}")]
    // [Authorize(Roles = "Admin")] // Descomente quando integrar a autenticação
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarPropriedades(int id, [FromBody] Dictionary<string, object> alteracoes)
    {
        var carroAtualizado = await _CarroService.EditarPropriedadesAdminAsync(id, alteracoes, ObterUsuarioId());
        return Ok(carroAtualizado);
    }

    [HttpGet("listarPaginado")]
    public async Task<ActionResult<List<ReadCarroDTO>>> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20)
    => Ok(await _CarroService.ListarMaisRecentesAsync(pagina, tamanhoPagina));
}