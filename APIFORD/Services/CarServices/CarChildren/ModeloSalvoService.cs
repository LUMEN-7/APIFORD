using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.CarroServices.CarroChildren;

public class ModeloSalvoService : BaseService<ModeloSalvo, CreateModeloSalvoDTO, ReadModeloSalvoDTO, UpdateModeloSalvoDTO, int>
{
    // Injetamos o HelperService para reaproveitar a extração de fontes
    private readonly HelperService _helperService;
    private readonly FordDbContext _context;

    public ModeloSalvoService(FordDbContext context, IMapper mapper, HelperService helperService, FordDbContext dbContext)
        : base(context, mapper)
    {
        _helperService = helperService;
        _context = dbContext;
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

        var linhagens = favorites.Select(f => f.Carro.LinhagemId).ToList();
        var carrosAtuais = await _context.Carros
            .Where(c => linhagens.Contains(c.LinhagemId))
            .GroupBy(c => c.LinhagemId)
            .Select(g => g.OrderByDescending(c => c.Id).First())
            .ToListAsync();


        var readDto = Mapper.Map<ReadModeloSalvoDTO>(favorites);
        readDto.FavoriteCarros = Mapper.Map<List<ReadCarroDTO>>(carrosAtuais);
        
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