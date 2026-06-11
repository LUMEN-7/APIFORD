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
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public ModeloSalvoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<ReadModeloSalvoDTO> GetUserFavoritesAsync(string userId)
    {
        var favorites = await _context.ModeloSalvos
            .Where(ms => ms.UserId == userId)
            .Include(ms => ms.Carro).ThenInclude(c => c.Especificacoes)
            .Include(ms => ms.Carro).ThenInclude(c => c.Consumos)
            .Include(ms => ms.Carro).ThenInclude(c => c.Dimensoes)
            .Include(ms => ms.Carro).ThenInclude(c => c.Pneus)
            .Include(ms => ms.Carro).ThenInclude(c => c.Extras)
            .Include(ms => ms.Carro).ThenInclude(c => c.ModosCarro).ThenInclude(cm => cm.Modo)
            .ToListAsync();

        if (!favorites.Any())
        {
            return new ReadModeloSalvoDTO { UserId = userId, FavoriteCarros = new List<ReadCarroDTO>() };
        }

        var readDto = _mapper.Map<ReadModeloSalvoDTO>(favorites);

        foreach (var fav in favorites)
        {
            var carroDto = readDto.FavoriteCarros.FirstOrDefault(c => c.Id == fav.CarroId);
            if (carroDto != null && fav.Carro != null)
            {
                await PreencherCatalogoDeFontesNoDtoAsync(carroDto, fav.Carro);
            }
        }

        return readDto;
    }

    private async Task PreencherCatalogoDeFontesNoDtoAsync(ReadCarroDTO dto, Carro carro)
    {
        var fontesIds = new HashSet<int>();

        if (carro.Especificacoes != null)
        {
            foreach (var spec in carro.Especificacoes)
            {
                fontesIds.UnionWith(spec.Potencia.HistoricoFontes.Select(p => p.FonteId));
                fontesIds.UnionWith(spec.Torque.HistoricoFontes.Select(t => t.FonteId));
                fontesIds.UnionWith(spec.PotenciaRpm.HistoricoFontes.Select(pr => pr.FonteId));
                fontesIds.UnionWith(spec.TorqueRpm.HistoricoFontes.Select(tr => tr.FonteId));
                fontesIds.UnionWith(spec.Transmissao.HistoricoFontes.Select(t => t.FonteId));
                fontesIds.UnionWith(spec.Tracao.HistoricoFontes.Select(t => t.FonteId));
            }
        }

        if (carro.Consumos != null)
        {
            foreach (var cons in carro.Consumos)
            {
                fontesIds.UnionWith(cons.Cidade.HistoricoFontes.Select(c => c.FonteId));
                fontesIds.UnionWith(cons.Estrada.HistoricoFontes.Select(e => e.FonteId));
            }
        }

        if (carro.Dimensoes != null)
        {
            foreach (var dim in carro.Dimensoes)
            {
                fontesIds.UnionWith(dim.Comprimento.HistoricoFontes.Select(c => c.FonteId));
                fontesIds.UnionWith(dim.Largura.HistoricoFontes.Select(l => l.FonteId));
                fontesIds.UnionWith(dim.Altura.HistoricoFontes.Select(a => a.FonteId));
                fontesIds.UnionWith(dim.EntreEixos.HistoricoFontes.Select(e => e.FonteId));
            }
        }

        if (carro.Pneus != null)
        {
            foreach (var pneu in carro.Pneus)
            {
                fontesIds.UnionWith(pneu.Tipo.HistoricoFontes.Select(t => t.FonteId));
                fontesIds.UnionWith(pneu.Aro.HistoricoFontes.Select(a => a.FonteId));
                fontesIds.UnionWith(pneu.Largura.HistoricoFontes.Select(l => l.FonteId));
                fontesIds.UnionWith(pneu.Perfil.HistoricoFontes.Select(p => p.FonteId));
            }
        }

        if (carro.Extras != null)
        {
            foreach (var extra in carro.Extras)
            {
                fontesIds.UnionWith(extra.CapacidadeTanque.HistoricoFontes.Select(c => c.FonteId));
                fontesIds.UnionWith(extra.TipoCombustivel.HistoricoFontes.Select(t => t.FonteId));
                fontesIds.UnionWith(extra.CapacidadeCarga.HistoricoFontes.Select(c => c.FonteId));
                fontesIds.UnionWith(extra.CapacidadeReboque.HistoricoFontes.Select(c => c.FonteId));
            }
        }

        if (fontesIds.Count > 0)
        {
            var listaFontesEntidade = await _context.Fontes
                .Where(f => fontesIds.Contains(f.Id) && !f.Excluido)
                .ToListAsync();

            dto.Fontes = _mapper.Map<List<ReadFonteDTO>>(listaFontesEntidade);
        }
    }
}