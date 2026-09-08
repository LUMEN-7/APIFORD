using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Data.DTOS.Comparison.Direct;
using APIFORD.Services.Comparison;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Comparison;

/// <summary>
/// Controller responsável por expor os endpoints de comparação entre carros.
/// Suporta dois cenários: comparação de um carro contra um grupo/categoria de mercado (BI)
/// e comparação direta entre dois ou mais carros específicos (embate 1x1).
/// </summary>
[ApiController]
[Route("[controller]")]
public class ComparacaoController : ControllerBase
{
    private readonly ComparacaoService _comparacaoService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="ComparacaoController"/>.
    /// </summary>
    /// <param name="comparacaoService">Serviço responsável pelas regras de negócio de comparação de carros.</param>
    public ComparacaoController(ComparacaoService comparacaoService)
    {
        _comparacaoService = comparacaoService;
    }

    /// <summary>
    /// Gera uma análise comparativa (BI de mercado) entre um carro base e os demais carros
    /// de sua categoria ou que atendam aos filtros avançados informados.
    /// </summary>
    /// <param name="dto">
    /// Dados da requisição contendo o Id do carro base, a categoria (opcional) e/ou
    /// a lista de filtros avançados a serem aplicados na busca dos concorrentes.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o <c>200 OK</c> com o resultado da comparação
    /// (carro base, concorrentes encontrados, médias da categoria e conclusões matemáticas).
    /// </returns>
    /// <response code="200">Comparação gerada com sucesso.</response>
    /// <response code="404">Carro base não encontrado.</response>
    [HttpPost("grupo")]
    [ProducesResponseType(typeof(ComparacaoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnalisarGrupo([FromBody] ComparacaoRequestDTO dto)
    {
        var resultado = await _comparacaoService.GerarComparacaoEmGrupoAsync(dto);
        return Ok(resultado);
    }

    /// <summary>
    /// Gera uma comparação direta (embate 1x1) entre dois ou mais carros específicos,
    /// confrontando seus atributos numéricos e categóricos.
    /// </summary>
    /// <param name="dto">
    /// Dados da requisição contendo a lista de Ids dos carros a serem comparados diretamente.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o <c>200 OK</c> com o resultado do embate
    /// (carros comparados e conclusões matemáticas por atributo).
    /// </returns>
    /// <response code="200">Comparação direta gerada com sucesso.</response>
    /// <response code="400">Menos de dois carros informados na requisição.</response>
    /// <response code="404">Nenhum carro encontrado para comparação.</response>
    [HttpPost("direta")]
    [ProducesResponseType(typeof(ComparacaoDiretaResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnalisarDireta([FromBody] ComparacaoDiretaRequestDTO dto)
    {
        var resultado = await _comparacaoService.GerarComparacaoDiretaAsync(dto);
        return Ok(resultado);
    }
}