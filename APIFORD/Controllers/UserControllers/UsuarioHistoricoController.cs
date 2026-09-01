using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Model;
using APIFORD.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFORD.Controllers.UserControllers;

[ApiController]
[Route("user")]
[Authorize] // Garante que apenas usuários logados acessem essas rotas
public class UsuarioHistoricoController : ControllerBase
{
    private readonly UsuarioHistoricoService _service;

    public UsuarioHistoricoController(UsuarioHistoricoService service)
    {
        _service = service;
    }

    // Helper privado para pegar o ID do Token JWT sem repetir código
    private string ObterUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    // =======================================================================
    // MODELOS
    // =======================================================================

    [HttpGet("modelos")]
    public async Task<IActionResult> ListarModelos()
    {
        var result = await _service.ListarModeloSalvosAsync(ObterUsuarioId());
        return Ok(result);
    }

    [HttpPost("modelos")]
    public async Task<IActionResult> SalvarModelo([FromBody] CreateModeloSalvoDTO dto)
    {
        var sucesso = await _service.SalvarModeloAsync(ObterUsuarioId(), dto);
        if (!sucesso) return BadRequest("Modelo já está salvo ou não encontrado.");

        return Ok("Modelo favoritado com sucesso.");
    }

    [HttpDelete("modelos/{carroId}")]
    public async Task<IActionResult> RemoverModelo(int carroId)
    {
        var sucesso = await _service.RemoverModeloAsync(ObterUsuarioId(), carroId);
        if (!sucesso) return NotFound("Modelo não encontrado nos favoritos.");

        return NoContent(); // Padrão REST 204 para deleção com sucesso
    }

    // =======================================================================
    // COMPARAÇÕES
    // =======================================================================

    [HttpGet("comparacoes")]
    public async Task<IActionResult> ListarComparacoes()
    {
        var result = await _service.ListarComparacoesSalvasAsync(ObterUsuarioId());
        return Ok(result);
    }

    [HttpPost("comparacoes")]
    public async Task<IActionResult> SalvarComparacao([FromBody] SalvarComparacaoDTO dto)
    {
        var result = await _service.SalvarComparacaoAsync(ObterUsuarioId(), dto);
        return Created("", result); // Retorna 201 Created com o DTO recém-criado
    }

    [HttpDelete("comparacoes/{comparacaoId}")]
    public async Task<IActionResult> RemoverComparacao(int comparacaoId)
    {
        var sucesso = await _service.RemoverComparacaoAsync(ObterUsuarioId(), comparacaoId);
        if (!sucesso) return NotFound("Comparação não encontrada no histórico.");

        return NoContent();
    }
}