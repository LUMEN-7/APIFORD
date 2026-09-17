using APIFORD.Data.DTOS.Export;
using APIFORD.Services.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Export;

/// <summary>
/// Controller responsável por expor os endpoints de exportação de dados de carros,
/// permitindo tanto a verificação prévia de conflitos entre fontes de dados quanto
/// a geração do arquivo de exportação em si (ex: CSV, Excel).
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class ExportacaoController : ControllerBase
{
    private readonly ExportacaoService _exportacaoService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="ExportacaoController"/>.
    /// </summary>
    /// <param name="exportacaoService">Serviço responsável pelas regras de negócio de exportação de carros.</param>
    public ExportacaoController(ExportacaoService exportacaoService) => _exportacaoService = exportacaoService;

    /// <summary>
    /// Verifica e retorna os conflitos de dados entre fontes distintas para os carros/linhagens
    /// informados, permitindo que o cliente decida como resolvê-los antes de solicitar a exportação.
    /// </summary>
    /// <param name="itens">Lista de itens de exportação (linhagem e/ou carro específico) a serem verificados, via query string.</param>
    /// <returns>Um <see cref="ActionResult{T}"/> contendo <c>200 OK</c> com a lista de conflitos encontrados por campo.</returns>
    [HttpGet("conflitos")]
    [ProducesResponseType(typeof(List<ConflitoCampoResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConflitoCampoResponseDTO>>> ObterConflitos([FromQuery] List<ItemExportacaoDTO> itens)
    {
        return Ok(await _exportacaoService.ObterConflitosAsync(itens));
    }

    /// <summary>
    /// Gera e retorna o arquivo de exportação dos carros solicitados, no formato especificado
    /// na requisição, já com os conflitos de fonte resolvidos (manual ou automaticamente).
    /// </summary>
    /// <param name="request">
    /// Dados da requisição contendo o formato de exportação, os itens a exportar, o modo de
    /// resolução de conflitos e as escolhas manuais de fonte (se aplicável).
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o arquivo gerado (<see cref="FileResult"/>), pronto
    /// para download, com o <c>Content-Type</c> e nome de arquivo apropriados ao formato escolhido.
    /// </returns>
    /// <response code="200">Arquivo exportado com sucesso.</response>
    /// <response code="400">Formato de exportação não suportado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Exportar([FromBody] ExportarRequestDTO request)
    {
        var (arquivo, contentType, nomeArquivo) = await _exportacaoService.ExportarAsync(request);
        return File(arquivo, contentType, nomeArquivo);
    }
}
