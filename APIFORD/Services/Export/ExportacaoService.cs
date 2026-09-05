using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Export;
using APIFORD.Model.CarroClasses;
using APIFORD.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Export;

public class ExportacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HelperService _helperService;
    private readonly Dictionary<string, IExportadorFormatoService> _exportadores;

    public ExportacaoService(FordDbContext context, IMapper mapper, HelperService helperService, IEnumerable<IExportadorFormatoService> exportadores)
    {
        _context = context;
        _mapper = mapper;
        _helperService = helperService;
        _exportadores = exportadores.ToDictionary(e => e.Formato, e => e);
    }

    private async Task<Carro?> ResolverCarroAsync(ItemExportacaoDTO item)
    {
        if (item.CarroId.HasValue)
        {
            var carro = await _context.Carros.FirstOrDefaultAsync(c => c.Id == item.CarroId.Value);
            if (carro != null && carro.LinhagemId != item.LinhagemId)
                throw new ArgumentException($"CarroId {item.CarroId} não pertence à linhagem {item.LinhagemId}.");
            return carro;
        }

        return await _context.Carros
            .Where(c => c.LinhagemId == item.LinhagemId)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ConflitoCampoDTO>> ObterConflitosAsync(List<ItemExportacaoDTO> itens)
    {
        var conflitos = new List<ConflitoCampoDTO>();

        foreach (var item in itens)
        {
            var carro = await ResolverCarroAsync(item);
            if (carro == null) continue;

            var dto = _mapper.Map<ReadCarroDTO>(carro);
            await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);
            var achatado = AchatadorCarro.Achatar(item.LinhagemId, dto, new Dictionary<string, string>());

            conflitos.AddRange(achatado.FontesDetalhadas
                .GroupBy(f => f.Campo)
                .Where(g => g.Select(f => f.Valor).Distinct().Count() > 1)
                .Select(g => new ConflitoCampoDTO
                {
                    LinhagemId = item.LinhagemId,
                    CampoCompleto = g.Key,
                    Fontes = g.Select(f => new FonteExportadaDTO { Fonte = f.Fonte, Valor = f.Valor, Confianca = f.Confianca, DataColeta = f.DataColeta }).ToList()
                }));
        }

        return conflitos;
    }

    public async Task<(byte[] Arquivo, string ContentType, string NomeArquivo)> ExportarAsync(ExportarRequestDTO request)
    {
        if (!_exportadores.TryGetValue(request.Formato.ToLower(), out var exportador))
            throw new ArgumentException($"Formato '{request.Formato}' não suportado. Use: {string.Join(", ", _exportadores.Keys)}");

        var escolhas = (request.Modo == ModoResolucaoFonte.Manual ? request.Escolhas : null) ?? new List<EscolhaManualDTO>();
        var carrosOriginais = new List<ReadCarroDTO>();
        var carrosAchatados = new List<ResultadoAchatamento>();

        foreach (var item in request.Itens)
        {
            var carro = await ResolverCarroAsync(item);
            if (carro == null) continue;

            var dto = _mapper.Map<ReadCarroDTO>(carro);
            await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);
            carrosOriginais.Add(dto);

            var escolhasDoCarro = escolhas.Where(e => e.LinhagemId == item.LinhagemId).ToDictionary(e => e.CampoCompleto, e => e.FonteEscolhida);
            carrosAchatados.Add(AchatadorCarro.Achatar(item.LinhagemId, dto, escolhasDoCarro));
        }

        var opcoes = new OpcoesExportacao { SeparadorCsv = request.Separador };
        var arquivo = exportador.Exportar(carrosOriginais, carrosAchatados, opcoes);

        return (arquivo, exportador.ContentType, $"carros_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{exportador.ExtensaoArquivo}");
    }
}