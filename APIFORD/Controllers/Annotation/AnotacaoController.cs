using APIFORD.Data.DTOS.Annotations;
using APIFORD.Middleware;
using APIFORD.Model.User;
using APIFORD.Services.Annotation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Annotation;

/// <summary>
/// Endpoints do sistema de anotações do usuário: notas organizadas em blocos, que podem
/// referenciar um veículo (por linhagem) ou uma comparação salva, exibidos como cards de preview.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class AnotacaoController : ControllerBase
{
    private readonly AnotacaoService _anotacaoService;
    public AnotacaoController(AnotacaoService anotacaoService) => _anotacaoService = anotacaoService;

    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    /// <summary>
    /// Cria uma nova anotação vazia (sem blocos) para o usuário logado.
    /// </summary>
    /// <param name="dto">Título e subtítulo da anotação.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ReadAnotacaoDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadAnotacaoDTO>> Criar([FromBody] CriarAnotacaoDTO dto)
         => Ok(await _anotacaoService.CriarAsync(ObterUsuarioId(), dto));

    /// <summary>
    /// Lista todas as anotações do usuário logado, da mais recentemente atualizada para a mais antiga.
    /// </summary>
    [HttpGet("minhas")]
    [ProducesResponseType(typeof(List<ReadAnotacaoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReadAnotacaoDTO>>> ListarMinhas()
        => Ok(await _anotacaoService.ListarPorUsuarioAsync(ObterUsuarioId()));

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadAnotacaoDTO>> ObterPorId(int id)
    => Ok(await _anotacaoService.ObterPorIdAsync(id, ObterUsuarioId()));

    /// <summary>
    /// Insere um novo bloco de conteúdo em uma anotação, numa posição específica (ou no final, se omitida).
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    /// <param name="dto">Tipo, texto e/ou referência (carro ou comparação) do novo bloco, e a posição de inserção.</param>
    [HttpPost("{id}/blocos")]
    [ProducesResponseType(typeof(ReadBlocoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadBlocoDTO>> InserirBloco(int id, [FromBody] InserirBlocoDTO dto)
    => Ok(await _anotacaoService.InserirBlocoAsync(id, ObterUsuarioId(), dto));

    /// <summary>
    /// Remove um bloco de uma anotação.
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    /// <param name="blocoId">Id do bloco a remover.</param>
    [HttpDelete("{id}/blocos/{blocoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverBloco(int id, string blocoId)
    {
        await _anotacaoService.RemoverBlocoAsync(id, ObterUsuarioId(), blocoId);
        return NoContent();
    }

    /// <summary>
    /// Substitui todos os blocos de uma anotação de uma vez (reordenação, inserção e remoção em lote).
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    /// <param name="dto">Lista completa de blocos, na ordem final desejada.</param>
    [HttpPut("{id}/blocos")]
    [ProducesResponseType(typeof(ReadAnotacaoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadAnotacaoDTO>> AtualizarBlocos(int id, [FromBody] AtualizarBlocosDTO dto)
        => Ok(await _anotacaoService.AtualizarBlocosAsync(id, ObterUsuarioId(), dto));

    /// <summary>
    /// Atualiza apenas o texto de um bloco específico, sem afetar sua posição ou referência.
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    /// <param name="blocoId">Id do bloco.</param>
    /// <param name="dto">Novo texto do bloco.</param>
    [HttpPatch("{id}/blocos/{blocoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarTextoBloco(int id, string blocoId, [FromBody] AtualizarTextoBlocoDTO dto)
    {
        await _anotacaoService.AtualizarTextoBlocoAsync(id, ObterUsuarioId(), blocoId, dto.Texto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> AtualizarTitulo(int id, [FromBody] AtualizarTituloAnotacaoDTO dto)
    {
        await _anotacaoService.AtualizarTituloAsync(id, ObterUsuarioId(), dto);
        return NoContent();
    }

    /// <summary>
    /// Atualiza o veículo ou a comparação referenciada por um bloco do tipo card
    /// (CardCarro ou CardComparacao).
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    /// <param name="blocoId">Id do bloco (precisa ser um card).</param>
    /// <param name="dto">Novo Id de referência (linhagem ou comparação, conforme o tipo do bloco).</param>
    [HttpPut("{id}/blocos/{blocoId}/referencia")]
    [ProducesResponseType(typeof(ReadBlocoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadBlocoDTO>> AtualizarReferenciaBloco(int id, string blocoId, [FromBody] AtualizarReferenciaBlocoDTO dto)
    => Ok(await _anotacaoService.AtualizarReferenciaBlocoAsync(id, ObterUsuarioId(), blocoId, dto));

    /// <summary>
    /// Exclui uma anotação inteira, com todos os seus blocos.
    /// </summary>
    /// <param name="id">Id da anotação.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        await _anotacaoService.ExcluirAsync(id, ObterUsuarioId());
        return NoContent();
    }
}