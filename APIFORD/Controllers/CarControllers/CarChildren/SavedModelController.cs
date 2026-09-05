using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.User;
using APIFORD.Services.CarroServices.CarroChildren;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.CarroControllers.CarroChildren;

[ApiController]
[Route("[controller]")]
public class SavedModelController : BaseController<ModeloSalvo, CreateModeloSalvoDTO, ReadModeloSalvoDTO, UpdateModeloSalvoDTO, int>
{
    private ModeloSalvoService _savedModelService;

    public SavedModelController(ModeloSalvoService savedModelService) : base(savedModelService)
    {
        _savedModelService = savedModelService;
    }

    /// <summary>
    /// Recupera a lista de modelos de Carros favoritados por um usuário específico.
    /// </summary>
    /// <param name="userId">A chave pAroária (ID) do usuário.</param>
    /// <returns>Um objeto contendo o ID do usuário e a lista de Carros favoritados.</returns>
    /// <response code="200">Lista de favoritos retornada com sucesso.</response>
    /// <response code="401">Usuário não autenticado ou token inválido.</response>
    /// <response code="404">Nenhum registro de favorito encontrado para o usuário informado.</response>
    [HttpGet("favorites/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserFavorites([FromRoute] string userId)
    {
        var favorites = await _savedModelService.GetUserFavoritesAsync(userId);
        return Ok(favorites);
    }
}
