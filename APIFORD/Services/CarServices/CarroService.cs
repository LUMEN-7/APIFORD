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
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)
            .Include(c => c.ModosCarro).ThenInclude(cm => cm.Modo);
    }

    // 2. Sobrescreve o CreateAsync para manter a sua lógica de mapeamento dos modos de direção
    public override async Task<ReadCarroDTO> CreateAsync(CreateCarroDTO dto)
    {
        var carro = Mapper.Map<Carro>(dto);

        if (dto.ModosCarro != null && dto.ModosCarro.Any())
        {
            foreach (var modeName in dto.ModosCarro)
            {
                var modeEntity = await Context.Modos.FirstOrDefaultAsync(m => m.Tipo == modeName);
                if (modeEntity != null)
                {
                    carro.ModosCarro.Add(new CarroModo { Carro = carro, Modo = modeEntity });
                }
            }
        }

        await DbSet.AddAsync(carro);
        await Context.SaveChangesAsync();

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);

        return readCarroDto;
    }

    public override async Task<ReadCarroDTO> GetByIdAsync(int id)
    {
        var query = DbSet.AsQueryable();
        query = AddIncludes(query);

        var carro = await query.FirstOrDefaultAsync(c => c.Id == id);
        if (carro == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado para o ID informado.");
        }

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);

        return readCarroDto;
    }

    private async Task PreencherCatalogoDeFontesNoDtoAsync(ReadCarroDTO dto, Carro carro)
    {
        var fontesIds = new HashSet<int>();

        foreach (var spec in carro.Especificacoes)
        {
            fontesIds.UnionWith(spec.Potencia.HistoricoFontes.Select(p => p.FonteId));
            fontesIds.UnionWith(spec.Torque.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(spec.PotenciaRpm.HistoricoFontes.Select(pr => pr.FonteId));
            fontesIds.UnionWith(spec.TorqueRpm.HistoricoFontes.Select(tr => tr.FonteId));
            fontesIds.UnionWith(spec.Transmissao.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(spec.Tracao.HistoricoFontes.Select(t => t.FonteId));
        }

        foreach (var cons in carro.Consumos)
        {
            fontesIds.UnionWith(cons.Cidade.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(cons.Estrada.HistoricoFontes.Select(e => e.FonteId));
        }

        foreach (var dim in carro.Dimensoes)
        {
            fontesIds.UnionWith(dim.Comprimento.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(dim.Largura.HistoricoFontes.Select(l => l.FonteId));
            fontesIds.UnionWith(dim.Altura.HistoricoFontes.Select(a => a.FonteId));
            fontesIds.UnionWith(dim.EntreEixos.HistoricoFontes.Select(e => e.FonteId));
        }

        foreach (var pneu in carro.Pneus)
        {
            fontesIds.UnionWith(pneu.Tipo.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(pneu.Aro.HistoricoFontes.Select(a => a.FonteId));
            fontesIds.UnionWith(pneu.Largura.HistoricoFontes.Select(l => l.FonteId));
            fontesIds.UnionWith(pneu.Perfil.HistoricoFontes.Select(p => p.FonteId));
        }

        foreach (var extra in carro.Extras)
        {
            fontesIds.UnionWith(extra.CapacidadeTanque.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(extra.TipoCombustivel.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(extra.CapacidadeCarga.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(extra.CapacidadeReboque.HistoricoFontes.Select(c => c.FonteId));
        }

        if (fontesIds.Count > 0)
        {
            var listaFontesEntidade = await Context.Fontes
                .Where(f => fontesIds.Contains(f.Id) && !f.Excluido)
                .ToListAsync();

            dto.Fontes = Mapper.Map<List<ReadFonteDTO>>(listaFontesEntidade);
        }
    }
}