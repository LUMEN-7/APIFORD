using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto.SavedModel;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace APIFORD.Services.CarroServices.CarroChildren;

public class ModeloSalvoService : BaseService<ModeloSalvo, CreateModeloSalvoDTO, ReadModeloSalvoDTO, UpdateModeloSalvoDTO, int>
{
    public ModeloSalvoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    { 
    }

    protected override IQueryable<ModeloSalvo> AddIncludes(IQueryable<ModeloSalvo> query)
    {
        return query
        // 1. Carrorega o Carro, suas Especificações e Fontes
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.Especificacaos)
        .ThenInclude(s => s.EspecificacaoFontes)
        .ThenInclude(ss => ss.Fonte)

        // 2. Carrorega os Consumos do Carro e suas Fontes
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.Consumos)
        .ThenInclude(co => co.ConsumoFontes)
        .ThenInclude(cs => cs.Fonte)

        // 3. Carrorega as Dimensões do Carro e suas Fontes
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.Dimensoes)
        .ThenInclude(d => d.DimensaoFontes)
        .ThenInclude(ds => ds.Fonte)

        // 4. Carrorega os Pneus do Carro e suas Fontes
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.Pneus)
        .ThenInclude(t => t.PneuFontes)
        .ThenInclude(ts => ts.Fonte)

        // 5. Carrorega os Extras do Carro e suas Fontes
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.Extras)
        .ThenInclude(a => a.ExtraFontes)
        .ThenInclude(af => af.Fonte)

        // 6. Carrorega os Modos de Condução do Carro
        .Include(sm => sm.Carro)
        .ThenInclude(c => c.ModosCarro)
        .ThenInclude(cm => cm.Modo);
    }
    public async Task<ReadModeloSalvoDTO> GetUserFavoritesAsync(string userId)
    {
        var savedModels = await Context.ModeloSalvos
            .Include(sm => sm.Carro)
            // Inclua os ThenInclude se quiser os detalhes do Carro (Consumo, Pneu, etc.)
            .Where(sm => sm.UserId == userId)
            .ToListAsync();

        if (!savedModels.Any())
            return new ReadModeloSalvoDTO { UserId = userId };

        return Mapper.Map<ReadModeloSalvoDTO>(savedModels);
    }


}
