using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Data.DTOS.Comparison.Direct;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Comparison;

public class ComparacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public ComparacaoService(FordDbContext context, IMapper mapper, HttpClient httpClient)
    {
        _context = context;
        _mapper = mapper;
        _httpClient = httpClient;
    }

    // =======================================================================
    // MÉTODO 1: COMPARAÇÃO EM GRUPO (Busca Concorrentes por Tolerância)
    // =======================================================================
    public async Task<ComparacaoResponseDTO?> GerarComparacaoEmGrupoAsync(ComparacaoRequestDTO dto)
    {
        // Busca o Carro Base de forma ultra-rápida (Sem Includes, o JSONB já traz tudo!)
        var carroBase = await _context.Carros.FirstOrDefaultAsync(c => c.Id == dto.CarroBaseId);
        if (carroBase == null) return null;

        var query = _context.Carros.AsQueryable();
        query = query.Where(c => c.Id != carroBase.Id);

        if (!string.IsNullOrEmpty(dto.Categoria))
            query = query.Where(c => c.Categoria.Fontes.Any(f => f.Valor == dto.Categoria));

        // MÁGICA DO POSTGRESQL (Busca DENTRO do JSONB em milissegundos)
        if (dto.Criterios.Contains("Potencia"))
        {
            // Extração segura do valor base em memória
            decimal potenciaBase = carroBase.Especificacoes?.FirstOrDefault()?.Potencia?.Fontes?.FirstOrDefault()?.Valor ?? 0m;

            if (potenciaBase > 0)
            {
                decimal margem = potenciaBase * (dto.ToleranciaPercentual / 100m);
                decimal min = potenciaBase - margem;
                decimal max = potenciaBase + margem;

                // O EF Core traduz isso para a busca no GIN Index do Postgres!
                query = query.Where(c => c.Especificacoes.Any(e =>
                    e.Potencia != null &&
                    e.Potencia.Fontes.Any(f => f.Valor >= min && f.Valor <= max)
                ));
            }
        }

        // Executa a Query e traz os concorrentes matematicamente validados
        var concorrentes = await query.Take(10).ToListAsync();
        var medias = new Dictionary<string, decimal>();

        // Prepara a resposta (Futuramente o texto do LLM entrará aqui)
        return new ComparacaoResponseDTO
        {
            CarroBase = _mapper.Map<ReadCarroDTO>(carroBase),
            ConcorrentesEncontrados = _mapper.Map<List<ReadCarroDTO>>(concorrentes),
            MediasDaCategoria = medias,
            ParecerIA = "LLM não conectado: Esta é uma análise simulada pelo C#."
        };
    }

    // =======================================================================
    // MÉTODO 2: COMPARAÇÃO DIRETA DE 'N' MODELOS (Selecionados a dedo)
    // =======================================================================
    public async Task<ComparacaoDiretaResponseDTO?> GerarComparacaoDiretaAsync(ComparacaoDiretaRequestDTO dto)
    {
        // Validação básica: precisamos de pelo menos 2 carros para haver comparação
        if (dto.CarrosIds == null || dto.CarrosIds.Count < 2)
        {
            throw new ArgumentException("É necessário informar pelo menos 2 carros para a comparação.");
        }

        // O EF Core traduz o .Contains() para um SELECT IN (id1, id2, id3...)
        // O PostgreSQL devolve todos os JSONBs solicitados num piscar de olhos
        var carros = await _context.Carros
            .Where(c => dto.CarrosIds.Contains(c.Id))
            .ToListAsync();

        // Validação opcional: verifica se todos os IDs enviados realmente existem no banco
        if (carros.Count == 0) return null;

        // ====================================================================
        // PREPARAÇÃO PARA O PYTHON (Integração Futura)
        // ====================================================================
        // Aqui nós enviaremos a lista inteira de carros para a API Python,
        // que usará dicionários flexíveis para analisar e gerar o texto final.

        var nomesCarros = string.Join(", ", carros.Select(c => $"{c.Marca} {c.Modelo}"));
        string textoIA = $"Análise Competitiva entre: {nomesCarros}. O Python definirá o vencedor geral em breve!";

        return new ComparacaoDiretaResponseDTO
        {
            CarrosComparados = _mapper.Map<List<ReadCarroDTO>>(carros),
            ParecerIA = textoIA
        };
    }

}
