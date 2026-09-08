// APIFORD/Controllers/Search/PesquisaController.cs
using APIFORD.Data.DTOS.Search;
using APIFORD.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Search;

/// <summary>
/// Controller responsável por expor os endpoints de busca de especificações de carros,
/// que são processadas de forma assíncrona por um microserviço Python. O fluxo funciona
/// em duas etapas: (1) inicia a busca e recebe um <c>job_id</c> imediatamente, e
/// (2) consulta periodicamente o status desse job até que ele seja concluído.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PesquisaController : ControllerBase
{
    private readonly PesquisaService _pesquisaService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="PesquisaController"/>.
    /// </summary>
    /// <param name="pesquisaService">Serviço responsável pelas regras de negócio de busca de especificações de carros.</param>
    public PesquisaController(PesquisaService pesquisaService)
    {
        _pesquisaService = pesquisaService;
    }

    /// <summary>
    /// Inicia uma busca de especificações de um carro, delegando o processamento ao
    /// microserviço Python. Se já existir no banco um carro correspondente e "fresco"
    /// (dentro do prazo de validade dos dados) e <paramref name="forcarNovaBusca"/> for
    /// <c>false</c>, retorna um job já concluído a partir do cache em vez de disparar uma
    /// nova busca externa.
    /// </summary>
    /// <param name="dto">Dados da busca: marca, modelo e ano do carro desejado.</param>
    /// <param name="forcarNovaBusca">
    /// Quando <c>true</c>, ignora qualquer resultado em cache e força o disparo de uma
    /// nova busca no microserviço Python. Padrão: <c>false</c>.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo <c>202 Accepted</c> com o <c>job_id</c> gerado
    /// (ou reaproveitado do cache), a ser usado para consultar o andamento via
    /// <see cref="ChecarJob"/>.
    /// </returns>
    /// <response code="202">Busca iniciada (ou resultado em cache disponibilizado) com sucesso; retorna o <c>job_id</c>.</response>
    /// <response code="503">O microserviço Python está indisponível no momento.</response>
    [HttpPost("busca")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Busca([FromBody] BuscaDTO dto, [FromQuery] bool forcarNovaBusca = false)
    {
        var jobId = await _pesquisaService.BuscarOuIniciarAsync(dto, forcarNovaBusca);
        return Accepted(new { job_id = jobId });
    }

    /// <summary>
    /// Consulta o status atual de um job de busca previamente iniciado. Quando o status é
    /// <c>done</c> pela primeira vez, o carro retornado pelo microserviço Python é processado
    /// e persistido no banco antes de ser incluído na resposta.
    /// </summary>
    /// <param name="id">Id (GUID) do job de busca a ser consultado, retornado pelo endpoint <see cref="Busca"/>.</param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo <c>200 OK</c> com o status do job
    /// (<c>pending</c>, <c>running</c>, <c>done</c>, <c>error</c> ou <c>not_found</c>).
    /// Quando o status é <c>done</c>, a resposta inclui o carro salvo no campo <c>carro</c>.
    /// </returns>
    /// <response code="200">Status do job retornado com sucesso.</response>
    /// <response code="404">Nenhum job encontrado com o Id informado.</response>
    [HttpGet("jobs/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChecarJob(Guid id)
    {
        return Ok(await _pesquisaService.ChecarJob(id));
    }
}