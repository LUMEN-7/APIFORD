using APIFORD.Data.DTOS.Schedule;
using APIFORD.Model.Schedule;
using APIFORD.Model.User;
using APIFORD.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Schedule;

/// <summary>
/// Controller responsável por expor os endpoints de escuta de lançamento, permitindo que o
/// usuário autenticado se inscreva para ser avisado sobre um carro ainda não catalogado,
/// consulte suas escutas ativas e cancele sua inscrição em uma escuta específica.
/// </summary>
[ApiController]
[Route("[controller]")]
//[Authorize]
public class EscutaLancamentoController : ControllerBase
{
    private readonly EscutaLancamentoService _escutaService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="EscutaLancamentoController"/>.
    /// </summary>
    /// <param name="escutaService">Serviço responsável pelas regras de negócio de escuta de lançamentos.</param>
    public EscutaLancamentoController(EscutaLancamentoService escutaService)
        => _escutaService = escutaService;

    /// <summary>
    /// Obtém o Id do usuário autenticado a partir das claims da requisição atual.
    /// </summary>
    /// <returns>O Id do usuário autenticado, ou uma string vazia caso a claim não esteja presente.</returns>
    private string ObterUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    /// <summary>
    /// Cria (ou entra em) uma escuta de lançamento para o carro informado, inscrevendo o
    /// usuário autenticado para ser notificado assim que ele for encontrado.
    /// </summary>
    /// <param name="dto">Dados da escuta desejada: marca, modelo, ano (opcional) e data de expiração (opcional).</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após a inscrição.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Criar([FromBody] CriarEscutaLancamentoDTO dto)
    {
        await _escutaService.CriarOuEntrarAsync(ObterUsuarioId(), dto.Marca, dto.Modelo, dto.Ano, dto.ExpiraEm);
        return NoContent();
    }

    /// <summary>
    /// Lista todas as escutas de lançamento ativas do usuário autenticado.
    /// </summary>
    /// <returns>Um <see cref="ActionResult{T}"/> contendo <c>200 OK</c> com a lista de escutas do usuário.</returns>
    [HttpGet("minhas")]
    [ProducesResponseType(typeof(List<EscutaLancamentoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EscutaLancamento>>> ListarMinhas()
        => Ok(await _escutaService.ListarPorUsuarioAsync(ObterUsuarioId()));

    /// <summary>
    /// Remove a inscrição do usuário autenticado em uma escuta de lançamento específica.
    /// </summary>
    /// <param name="escutaId">Id da escuta de lançamento da qual o usuário deseja sair.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após a saída.</returns>
    [HttpDelete("{escutaId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SairDaEscuta(int escutaId)
    {
        await _escutaService.SairAsync(escutaId, ObterUsuarioId());
        return NoContent();
    }
}