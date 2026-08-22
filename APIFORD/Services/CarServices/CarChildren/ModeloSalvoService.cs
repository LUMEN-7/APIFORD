using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class ModeloSalvoService : BaseService<ModeloSalvo, CreateModeloSalvoDTO, ReadModeloSalvoDTO, UpdateModeloSalvoDTO, int>
{
    // Injetamos o HelperService para reaproveitar a extração de fontes
    private readonly HelperService _helperService;

    public ModeloSalvoService(FordDbContext context, IMapper mapper, HelperService helperService)
        : base(context, mapper)
    {
        _helperService = helperService;
    }

    public async Task<ReadModeloSalvoDTO> GetUserFavoritesAsync(string userId)
    {
        var favorites = await DbSet
            .Where(ms => ms.UserId == userId)
            .Include(ms => ms.Carro) // Fazemos o Include APENAS da tabela Carro. O JSONB vem junto de graça!
            .ToListAsync();

        if (!favorites.Any())
        {
            return new ReadModeloSalvoDTO { UserId = userId, FavoriteCarros = new List<ReadCarroDTO>() };
        }

        var readDto = Mapper.Map<ReadModeloSalvoDTO>(favorites);

        foreach (var fav in favorites)
        {
            var carroDto = readDto.FavoriteCarros.FirstOrDefault(c => c.Id == fav.CarroId);
            if (carroDto != null && fav.Carro != null)
            {
                // Reutilizamos a lógica centralizada em vez de repetir 60 linhas de código!
                await _helperService.PreencherCatalogoDeFontesNoDtoAsync(carroDto, fav.Carro);
            }
        }

        return readDto;
    }
}