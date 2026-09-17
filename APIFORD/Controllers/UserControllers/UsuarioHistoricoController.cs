using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Middleware;
using APIFORD.Model;
using APIFORD.Services.Comparison;
using APIFORD.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.UserControllers;

/// <summary>
/// Controller responsável por expor os endpoints do histórico pessoal do usuário autenticado,
/// contemplando dois recursos: modelos de carro favoritados (salvos) e comparações salvas
/// (histórico de análises/embates gerados anteriormente).
/// </summary>
[ApiController]
[Route("user")]
[Authorize]
public class UsuarioHistoricoController : ControllerBase
{
    private readonly UsuarioHistoricoService _service;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="UsuarioHistoricoController"/>.
    /// </summary>
    /// <param name="service">Serviço responsável pelas regras de negócio do histórico do usuário.</param>
    public UsuarioHistoricoController(UsuarioHistoricoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém o Id do usuário autenticado a partir das claims do token JWT da requisição atual.
    /// </summary>
    /// <returns>O Id do usuário autenticado, ou uma string vazia caso a claim não esteja presente.</returns>
    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    // =======================================================================
    // MODELOS
    // =======================================================================

    /// <summary>
    /// Lista todos os modelos de carro favoritados (salvos) pelo usuário autenticado.
    /// </summary>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com a lista de carros favoritados.</returns>
    /// <response code="200">Modelos listados com sucesso (lista pode vir vazia).</response>
    [HttpGet("modelos")]
    [ProducesResponseType(typeof(List<ReadCarroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarModelos()
    {
        var result = await _service.ListarModeloSalvosAsync(ObterUsuarioId());
        return Ok(result);
    }

    /// <summary>
    /// Favorita (salva) um modelo de carro para o usuário autenticado.
    /// </summary>
    /// <param name="dto">Dados do modelo a ser salvo, incluindo o Id do carro.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com uma mensagem de confirmação.</returns>
    /// <response code="200">Modelo favoritado com sucesso.</response>
    /// <response code="400">O modelo informado já havia sido favoritado anteriormente pelo usuário.</response>
    [HttpPost("modelos")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SalvarModelo([FromBody] CreateModeloSalvoDTO dto)
    {
        var userId = ObterUsuarioId();
        var sucesso = await _service.SalvarModeloAsync(userId, dto);
        if (!sucesso) return NotFound();
        return Created("",sucesso);
    }

    [HttpGet("modelos/quantidade-salvos")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterQuantidadeDeModelosSalvos()
        => Ok(await _service.ObterQuantidadeDeModelosSalvosAsync(ObterUsuarioId()));

    /// <summary>
    /// Remove um modelo de carro previamente favoritado pelo usuário autenticado.
    /// </summary>
    /// <param name="linhagemId">Id da linhagem do carro a ser removido dos favoritos.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após a remoção.</returns>
    /// <response code="204">Modelo removido dos favoritos com sucesso.</response>
    /// <response code="404">Modelo favoritado não encontrado para o usuário informado.</response>
    [HttpDelete("modelos/{linhagemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Padrão REST 204 para deleção com sucesso
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverModelo(int linhagemId)
    {
        var sucesso = await _service.RemoverModeloAsync(ObterUsuarioId(), linhagemId);
        if (!sucesso) return NotFound();
        return NoContent();
    }

    // =======================================================================
    // COMPARAÇÕES
    // =======================================================================

    /// <summary>
    /// Lista o histórico de comparações salvas pelo usuário autenticado, da mais recente para a mais antiga.
    /// </summary>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>200 OK</c> com a lista de comparações salvas.</returns>
    /// <response code="200">Comparações listadas com sucesso (lista pode vir vazia).</response>
    [HttpGet("comparacoes")]
    [ProducesResponseType(typeof(List<ReadComparacaoSalvaDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarComparacoes()
    {
        var result = await _service.ListarComparacoesSalvasAsync(ObterUsuarioId());
        return Ok(result);
    }

    /// <summary>
    /// Salva uma comparação (grupo ou direta) no histórico do usuário autenticado, para consulta posterior.
    /// </summary>
    /// <param name="dto">Dados da comparação a ser salva: título, tipo e payload da requisição original.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>201 Created</c> com a comparação recém-salva.</returns>
    /// <response code="201">Comparação salva com sucesso.</response>
    [HttpPost("comparacoes")]
    [ProducesResponseType(typeof(ReadComparacaoSalvaDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> SalvarComparacao([FromBody] SalvarComparacaoDTO dto)
    {
        var result = await _service.SalvarComparacaoAsync(ObterUsuarioId(), dto);
        return Created("", result); // Retorna 201 Created com o DTO recém-criado
    }

    /// <summary>
    /// Remove uma comparação previamente salva no histórico do usuário autenticado.
    /// </summary>
    /// <param name="comparacaoId">Id da comparação a ser removida.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após a remoção.</returns>
    /// <response code="204">Comparação removida com sucesso.</response>
    /// <response code="404">Comparação não encontrada para o usuário informado.</response>
    [HttpDelete("comparacoes/{comparacaoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverComparacao(int comparacaoId)
    {
        var sucesso = await _service.RemoverComparacaoAsync(ObterUsuarioId(), comparacaoId);
        return NoContent();
    }

    [HttpGet("comparacoes/quantidade-salvas")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterQuantidadeDeComparacoesSalvas()
    => Ok(await _service.ObterQuantidadeDeComparacoesSalvasAsync(ObterUsuarioId()));

    [HttpGet("comparacoes/quantidade-semana")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterQuantidadeDeComparacoesSemana()
        => Ok(await _service.ObterQuantidadeDeComparacoesSemanaAsync(ObterUsuarioId()));
}
