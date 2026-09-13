using APIFORD.Data.DTOS.Schedule;
using APIFORD.Middleware;
using APIFORD.Model.Schedule;
using APIFORD.Model.User;
using APIFORD.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.Schedule;

/// <summary>
/// Controller responsável por expor os endpoints de agendamento de pesquisas de carros,
/// permitindo que o usuário autenticado crie, liste e cancele buscas programadas
/// (únicas, semanais ou mensais) por marca/modelo/ano.
/// </summary>
[ApiController]
[Route("[controller]")]
//[Authorize]
public class AgendamentoPesquisaController : ControllerBase
{
    private readonly AgendamentoPesquisaService _agendamentoService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="AgendamentoPesquisaController"/>.
    /// </summary>
    /// <param name="agendamentoService">Serviço responsável pelas regras de negócio de agendamento de pesquisas.</param>
    public AgendamentoPesquisaController(AgendamentoPesquisaService agendamentoService)
        => _agendamentoService = agendamentoService;

    /// <summary>
    /// Obtém o Id do usuário autenticado a partir das claims da requisição atual.
    /// </summary>
    /// <returns>O Id do usuário autenticado, ou uma string vazia caso a claim não esteja presente.</returns>
    protected string ObterUsuarioId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedException("Não foi possível identificar o usuário autenticado.");
        return id;
    }

    /// <summary>
    /// Cria um novo agendamento de pesquisa para o usuário autenticado.
    /// </summary>
    /// <param name="dto">Dados do agendamento a ser criado, incluindo marca, modelo, ano, data agendada e recorrência.</param>
    /// <returns>Um <see cref="ActionResult{T}"/> contendo <c>200 OK</c> com o agendamento criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoPesquisaDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgendamentoPesquisa>> Criar([FromBody] CriarAgendamentoDTO dto)
        => Ok(await _agendamentoService.CriarAsync(ObterUsuarioId(), dto));

    /// <summary>
    /// Lista todos os agendamentos de pesquisa ativos (não cancelados) do usuário autenticado.
    /// </summary>
    /// <returns>Um <see cref="ActionResult{T}"/> contendo <c>200 OK</c> com a lista de agendamentos do usuário.</returns>
    [HttpGet("meus")]
    [ProducesResponseType(typeof(List<AgendamentoPesquisaDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AgendamentoPesquisa>>> ListarMeus()
        => Ok(await _agendamentoService.ListarPorUsuarioAsync(ObterUsuarioId()));

    /// <summary>
    /// Cancela um agendamento de pesquisa específico do usuário autenticado.
    /// </summary>
    /// <param name="id">Id do agendamento a ser cancelado.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo <c>204 No Content</c> após o cancelamento.</returns>
    /// <response code="404">Agendamento não encontrado para o usuário informado.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _agendamentoService.CancelarAsync(id, ObterUsuarioId());
        return NoContent();
    }
}