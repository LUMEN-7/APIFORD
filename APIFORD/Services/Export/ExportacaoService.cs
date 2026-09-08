using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Export;
using APIFORD.Model.CarroClasses;
using APIFORD.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Export;

/// <summary>
/// Serviço responsável pela exportação de dados de carros em diferentes formatos de arquivo
/// (ex: CSV, Excel), incluindo a detecção de conflitos entre fontes de dados divergentes
/// e a resolução manual ou automática desses conflitos antes da exportação final.
/// </summary>
public class ExportacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HelperService _helperService;
    private readonly Dictionary<string, IExportadorFormatoService> _exportadores;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="ExportacaoService"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados Ford.</param>
    /// <param name="mapper">Mapeador AutoMapper para conversão entre entidades e DTOs.</param>
    /// <param name="helperService">Serviço auxiliar utilizado para preencher o catálogo de fontes nos DTOs.</param>
    /// <param name="exportadores">
    /// Coleção de exportadores disponíveis (um por formato de arquivo suportado), injetada via DI
    /// e indexada internamente pelo nome do formato (<see cref="IExportadorFormatoService.Formato"/>).
    /// </param>
    public ExportacaoService(FordDbContext context, IMapper mapper, HelperService helperService, IEnumerable<IExportadorFormatoService> exportadores)
    {
        _context = context;
        _mapper = mapper;
        _helperService = helperService;
        _exportadores = exportadores.ToDictionary(e => e.Formato, e => e);
    }

    /// <summary>
    /// Resolve qual carro deve ser utilizado para um item de exportação: se um <c>CarroId</c>
    /// específico foi informado, valida se ele pertence à linhagem indicada; caso contrário,
    /// retorna o carro mais recente (maior Id) da linhagem informada.
    /// </summary>
    /// <param name="item">Item de exportação contendo a linhagem e, opcionalmente, o Id do carro desejado.</param>
    /// <returns>O <see cref="Carro"/> resolvido, ou <c>null</c> caso nenhum carro seja encontrado para a linhagem.</returns>
    /// <exception cref="BadHttpRequestException">
    /// Lançada quando o <c>CarroId</c> informado não pertence à <c>LinhagemId</c> indicada no item.
    /// </exception>
    private async Task<Carro?> ResolverCarroAsync(ItemExportacaoDTO item)
    {
        if (item.CarroId.HasValue)
        {
            var carro = await _context.Carros.FirstOrDefaultAsync(c => c.Id == item.CarroId.Value);
            if (carro != null && carro.LinhagemId != item.LinhagemId)
                throw new BadHttpRequestException($"CarroId {item.CarroId} não pertence à linhagem {item.LinhagemId}.");
            return carro;
        }

        return await _context.Carros
            .Where(c => c.LinhagemId == item.LinhagemId)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Identifica conflitos de dados entre diferentes fontes para os carros informados,
    /// isto é, campos em que mais de um valor distinto foi coletado de fontes diferentes.
    /// Utilizado para permitir que o usuário decida manualmente qual fonte usar antes de exportar.
    /// </summary>
    /// <param name="itens">Lista de itens de exportação (linhagem e/ou carro específico) a serem verificados.</param>
    /// <returns>
    /// Lista de <see cref="ConflitoCampoResponseDTO"/>, um para cada campo com valores divergentes entre fontes,
    /// contendo todas as fontes conflitantes encontradas para aquele campo.
    /// </returns>
    public async Task<List<ConflitoCampoResponseDTO>> ObterConflitosAsync(List<ItemExportacaoDTO> itens)
    {
        var conflitos = new List<ConflitoCampoResponseDTO>();

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
                .Select(g => new ConflitoCampoResponseDTO
                {
                    LinhagemId = item.LinhagemId,
                    CampoCompleto = g.Key,
                    Fontes = g.Select(f => new FonteExportadaDTO { Fonte = f.Fonte, Valor = f.Valor, Confianca = f.Confianca, DataColeta = f.DataColeta }).ToList()
                }));
        }

        return conflitos;
    }

    /// <summary>
    /// Gera o arquivo de exportação dos carros solicitados, no formato especificado, aplicando
    /// as escolhas manuais de fonte (quando em modo manual) para resolver conflitos entre campos
    /// antes de "achatar" os dados dos carros no formato final do arquivo.
    /// </summary>
    /// <param name="request">
    /// Requisição contendo o formato desejado, os itens (carros/linhagens) a exportar, o modo de
    /// resolução de conflitos (manual ou automático), as escolhas manuais de fonte (se aplicável)
    /// e o separador CSV (se aplicável ao formato).
    /// </param>
    /// <returns>
    /// Uma tupla contendo os bytes do arquivo gerado, o <c>ContentType</c> do arquivo e o nome do
    /// arquivo (com timestamp), prontos para retorno via HTTP.
    /// </returns>
    /// <exception cref="BadHttpRequestException">
    /// Lançada quando o formato solicitado não corresponde a nenhum exportador registrado.
    /// </exception>
    public async Task<(byte[] Arquivo, string ContentType, string NomeArquivo)> ExportarAsync(ExportarRequestDTO request)
    {
        if (!_exportadores.TryGetValue(request.Formato.ToLower(), out var exportador))
            throw new BadHttpRequestException($"Formato '{request.Formato}' não suportado. Use: {string.Join(", ", _exportadores.Keys)}");

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