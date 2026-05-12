using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using APIFORD.Model.CarroClasses.Intermedians;
using System.Runtime.ConstrainedExecution;
namespace APIFORD.Services.CarroServices;

public class CarroService : BaseService<Carro, CreateCarroDTO, ReadCarroDTO, UpdateCarroDTO, int>
{
    public CarroService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // 1. Sobrescreve para apliCarro os Includes do Carro
    protected override IQueryable<Carro> AddIncludes(IQueryable<Carro> query)
    {
        return query
            .Include(c => c.Especificacaos).ThenInclude(s => s.EspecificacaoFontes).ThenInclude(ss => ss.Fonte)
            .Include(c => c.Consumos).ThenInclude(co => co.ConsumoFontes).ThenInclude(cs => cs.Fonte)
            .Include(c => c.Dimensoes).ThenInclude(d => d.DimensaoFontes).ThenInclude(ds => ds.Fonte)
            .Include(c => c.Pneus).ThenInclude(t => t.PneuFontes).ThenInclude(ts => ts.Fonte)
            .Include(c => c.Extras).ThenInclude(a => a.ExtraFontes).ThenInclude(ans => ans.Fonte)
            .Include(c => c.ModosCarro).ThenInclude(cm => cm.Modo);
    }

    // 2. Sobrescreve o CreateAsync para manter a sua lógica de mapeamento dos modos de direção
    public override async Task<ReadCarroDTO> CreateAsync(CreateCarroDTO dto)
    {
        var Carro = Mapper.Map<Carro>(dto);

        if (dto.ModosCarro != null && dto.ModosCarro.Any())
        {
            foreach (var modeName in dto.ModosCarro)
            {
                var modeEntity = await Context.Modos.FirstOrDefaultAsync(m => m.Tipo == modeName);
                if (modeEntity != null)
                {
                    Carro.ModosCarro.Add(new CarroModo { Carro = Carro, Modo = modeEntity });
                }
            }
        }

        await DbSet.AddAsync(Carro);
        await Context.SaveChangesAsync();
        return Mapper.Map<ReadCarroDTO>(Carro);
    }
}