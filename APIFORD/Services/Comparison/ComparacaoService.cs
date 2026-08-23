using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Data.DTOS.Comparison.Direct;
using APIFORD.Model.CarroClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APIFORD.Services.Comparison;

public class ComparacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HelperService _helperService;

    public ComparacaoService(FordDbContext context, IMapper mapper, HelperService helperService)
    {
        _context = context;
        _mapper = mapper;
        _helperService = helperService;
    }

    // =======================================================================
    // MÉTODO 1: COMPARAÇÃO EM GRUPO (BI DE MERCADO)
    // =======================================================================
    public async Task<ComparacaoResponseDTO?> GerarComparacaoEmGrupoAsync(ComparacaoRequestDTO dto)
    {
        var carroBase = await _context.Carros.FirstOrDefaultAsync(c => c.Id == dto.CarroBaseId);
        if (carroBase == null) return null;

        var query = _context.Carros.AsQueryable().Where(c => c.Id != carroBase.Id && !c.Excluido);

        // O usuário pode filtrar tanto por Categoria quanto pelos Filtros Avançados
        if (!string.IsNullOrEmpty(dto.Categoria))
            query = query.Where(c => c.Categoria.Fontes.Any(f => f.Valor == dto.Categoria));

        foreach (var filtro in dto.Filtros)
        {
            query = AplicarFiltroNoBanco(query, filtro);
        }

        var concorrentes = await query.Take(15).ToListAsync();

        var medias = new Dictionary<string, string>();
        var conclusoes = new Dictionary<string, string>();

        if (concorrentes.Any())
        {
            var todos = new List<Carro> { carroBase };
            todos.AddRange(concorrentes);

            // 1. EXTRAI E COMPARA OS NÚMEROS (Médias)
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Potência", "cv", true, c => c.Especificacoes?.FirstOrDefault()?.Potencia?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Torque", "kgfm", true, c => c.Especificacoes?.FirstOrDefault()?.Torque?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Potência RPM", "rpm", false, c => c.Especificacoes?.FirstOrDefault()?.PotenciaRpm?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Torque RPM", "rpm", false, c => c.Especificacoes?.FirstOrDefault()?.TorqueRpm?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Consumo Cidade", "km/l", true, c => c.Consumos?.FirstOrDefault()?.Cidade?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Consumo Estrada", "km/l", true, c => c.Consumos?.FirstOrDefault()?.Estrada?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Comprimento", "m", true, c => c.Dimensoes?.FirstOrDefault()?.Comprimento?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Entre-Eixos", "m", true, c => c.Dimensoes?.FirstOrDefault()?.EntreEixos?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Capacidade Tanque", "L", true, c => c.Extras?.FirstOrDefault()?.CapacidadeTanque?.Fontes?.FirstOrDefault()?.Valor);
            CompararNumComGrupo(carroBase, todos, medias, conclusoes, "Capacidade Carga", "kg", true, c => c.Extras?.FirstOrDefault()?.CapacidadeCarga?.Fontes?.FirstOrDefault()?.Valor);

            // 2. EXTRAI E COMPARA OS TEXTOS (Moda Estatística)
            CompararCatComGrupo(carroBase, todos, medias, conclusoes, "Transmissão", c => c.Especificacoes?.FirstOrDefault()?.Transmissao?.Fontes?.FirstOrDefault()?.Valor);
            CompararCatComGrupo(carroBase, todos, medias, conclusoes, "Tração", c => c.Especificacoes?.FirstOrDefault()?.Tracao?.Fontes?.FirstOrDefault()?.Valor);
            CompararCatComGrupo(carroBase, todos, medias, conclusoes, "Tipo Combustível", c => c.Extras?.FirstOrDefault()?.TipoCombustivel?.Fontes?.FirstOrDefault()?.Valor);

            // 3. EXTRAI E COMPARA AS LISTAS (Modos de Condução)
            CompararListaCatComGrupo(carroBase, todos, medias, conclusoes, "Modos de Condução", c => c.Modos?.Fontes?.FirstOrDefault()?.Valor);
        }

        var response = new ComparacaoResponseDTO
        {
            CarroBase = _mapper.Map<ReadCarroDTO>(carroBase),
            ConcorrentesEncontrados = _mapper.Map<List<ReadCarroDTO>>(concorrentes),
            MediasDaCategoria = medias,
            ConclusoesMatematicas = conclusoes,
            ParecerIA = "LLM não conectado: Análise de BI via C# concluída."
        };

        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(new List<ReadCarroDTO> { response.CarroBase }, new List<Carro> { carroBase });
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(response.ConcorrentesEncontrados, concorrentes);

        return response;
    }

    // =======================================================================
    // MÉTODO 2: COMPARAÇÃO DIRETA (EMBATE 1x1)
    // =======================================================================
    public async Task<ComparacaoDiretaResponseDTO?> GerarComparacaoDiretaAsync(ComparacaoDiretaRequestDTO dto)
    {
        if (dto.CarrosIds == null || dto.CarrosIds.Count < 2)
            throw new ArgumentException("É necessário informar pelo menos 2 carros.");

        var carros = await _context.Carros.Where(c => dto.CarrosIds.Contains(c.Id)).ToListAsync();
        if (carros.Count == 0) return null;

        var conclusoes = new Dictionary<string, string>();

        // 1. EMBATES NÚMERICOS
        EmbateNum(carros, conclusoes, "Potência", "cv", true, c => c.Especificacoes?.FirstOrDefault()?.Potencia?.Fontes?.FirstOrDefault()?.Valor);
        EmbateNum(carros, conclusoes, "Torque", "kgfm", true, c => c.Especificacoes?.FirstOrDefault()?.Torque?.Fontes?.FirstOrDefault()?.Valor);
        EmbateNum(carros, conclusoes, "Consumo Cidade", "km/l", true, c => c.Consumos?.FirstOrDefault()?.Cidade?.Fontes?.FirstOrDefault()?.Valor);
        EmbateNum(carros, conclusoes, "Entre-Eixos", "m", true, c => c.Dimensoes?.FirstOrDefault()?.EntreEixos?.Fontes?.FirstOrDefault()?.Valor);
        EmbateNum(carros, conclusoes, "Capacidade Reboque", "kg", true, c => c.Extras?.FirstOrDefault()?.CapacidadeReboque?.Fontes?.FirstOrDefault()?.Valor);

        // 2. EMBATES CATEGÓRICOS
        EmbateCat(carros, conclusoes, "Transmissão", c => c.Especificacoes?.FirstOrDefault()?.Transmissao?.Fontes?.FirstOrDefault()?.Valor);
        EmbateCat(carros, conclusoes, "Tração", c => c.Especificacoes?.FirstOrDefault()?.Tracao?.Fontes?.FirstOrDefault()?.Valor);
        EmbateCat(carros, conclusoes, "Tipo Combustível", c => c.Extras?.FirstOrDefault()?.TipoCombustivel?.Fontes?.FirstOrDefault()?.Valor);

        var response = new ComparacaoDiretaResponseDTO
        {
            CarrosComparados = _mapper.Map<List<ReadCarroDTO>>(carros),
            ConclusoesMatematicas = conclusoes,
            ParecerIA = "LLM não conectado: Embate Direto via C# concluído."
        };

        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(response.CarrosComparados, carros);
        return response;
    }

    // =======================================================================
    // MOTORES DE ANÁLISE GENÉRICA (NÚMEROS, TEXTOS E LISTAS)
    // =======================================================================

    // --- GRUPOS ---
    private void CompararNumComGrupo(Carro baseCar, List<Carro> todos, Dictionary<string, string> medias, Dictionary<string, string> conc, string nome, string un, bool maiorMelhor, Func<Carro, decimal?> seletor)
    {
        var vals = todos.Select(c => seletor(c) ?? 0).Where(v => v > 0).ToList();
        if (!vals.Any()) return;

        decimal media = Math.Round(vals.Average(), 2);
        medias[nome] = $"{media}{un}";

        decimal vBase = seletor(baseCar) ?? 0m;
        if (vBase == 0) return;

        decimal dif = ((vBase - media) / media) * 100m;
        bool acima = vBase > media;
        bool vantagem = maiorMelhor ? acima : !acima;

        conc[nome] = $"[{(vantagem ? "VANTAGEM" : "DESVANTAGEM")}] {vBase}{un} ({Math.Round(Math.Abs(dif), 1)}% {(acima ? "acima" : "abaixo")} da média de {media}{un})";
    }

    private void CompararCatComGrupo(Carro baseCar, List<Carro> todos, Dictionary<string, string> medias, Dictionary<string, string> conc, string nome, Func<Carro, string?> seletor)
    {
        var vals = todos.Select(c => seletor(c)).Where(v => !string.IsNullOrEmpty(v)).ToList();
        if (!vals.Any()) return;

        // Acha a string que mais repete (A Moda)
        string moda = vals.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key!;
        medias[nome] = moda;

        string vBase = seletor(baseCar) ?? "";
        if (string.IsNullOrEmpty(vBase)) return;

        conc[nome] = vBase.Equals(moda, StringComparison.OrdinalIgnoreCase)
            ? $"[PADRÃO] Iguala a tendência do mercado ({moda})"
            : $"[DIFERENTE] {vBase} (O mercado prefere {moda})";
    }

    private void CompararListaCatComGrupo(Carro baseCar, List<Carro> todos, Dictionary<string, string> medias, Dictionary<string, string> conc, string nome, Func<Carro, List<string>?> seletor)
    {
        var todosItens = todos.SelectMany(c => seletor(c) ?? new List<string>()).Where(v => !string.IsNullOrEmpty(v)).ToList();
        if (!todosItens.Any()) return;

        // Pega os 3 modos mais comuns do mercado
        var top3 = todosItens.GroupBy(v => v).OrderByDescending(g => g.Count()).Take(3).Select(g => g.Key).ToList();
        string tendencia = string.Join(", ", top3);
        medias[nome] = tendencia;

        var vBase = seletor(baseCar) ?? new List<string>();
        if (!vBase.Any()) return;

        int intersecao = vBase.Count(v => top3.Contains(v));
        conc[nome] = intersecao > 0
            ? $"[ALINHADO] Possui {intersecao} dos modos favoritos da categoria. (Tendência: {tendencia})"
            : $"[EXCLUSIVO] Possui modos diferentes do padrão da categoria. (Tendência: {tendencia})";
    }

    // --- DIRETAS ---
    private void EmbateNum(List<Carro> carros, Dictionary<string, string> conc, string nome, string un, bool maiorMelhor, Func<Carro, decimal?> seletor)
    {
        var rank = carros.Select(c => new { Carro = $"{c.Marca} {c.Modelo}", Valor = seletor(c) ?? 0 }).Where(x => x.Valor > 0).ToList();
        if (rank.Count < 2) return;

        rank = maiorMelhor ? rank.OrderByDescending(x => x.Valor).ToList() : rank.OrderBy(x => x.Valor).ToList();
        var prim = rank.First();
        var ult = rank.Last();

        if (prim.Valor == ult.Valor) { conc[nome] = "Empate técnico."; return; }
        decimal dif = ((prim.Valor - ult.Valor) / ult.Valor) * 100m;
        conc[nome] = $"{prim.Carro} vence com {prim.Valor}{un} (Vantagem de {Math.Round(Math.Abs(dif), 1)}%)";
    }

    private void EmbateCat(List<Carro> carros, Dictionary<string, string> conc, string nome, Func<Carro, string?> seletor)
    {
        var vals = carros.Select(c => new { Carro = $"{c.Marca} {c.Modelo}", Valor = seletor(c) ?? "" }).Where(x => !string.IsNullOrEmpty(x.Valor)).ToList();
        if (vals.Count < 2) return;

        bool todosIguais = vals.All(v => v.Valor.Equals(vals.First().Valor, StringComparison.OrdinalIgnoreCase));
        if (todosIguais)
        {
            conc[nome] = $"Empate. Ambos usam {vals.First().Valor}.";
        }
        else
        {
            var diffs = string.Join(" vs ", vals.Select(v => $"{v.Carro} ({v.Valor})"));
            conc[nome] = $"Diferem: {diffs}";
        }
    }

    // =======================================================================
    // FILTROS SQL 
    // =======================================================================
    private IQueryable<Carro> AplicarFiltroNoBanco(IQueryable<Carro> query, FiltroComparacaoDTO f)
    {
        string atributo = f.Atributo; // Ex: "Potencia" ou "Modos"

        // 1. Mapeamento de onde o atributo mora dentro do Carro
        var mapaColunas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Potencia", "Especificacoes" }, { "Torque", "Especificacoes" }, { "Transmissao", "Especificacoes" }, { "Tracao", "Especificacoes" },
            { "ConsumoCidade", "Consumos" }, { "ConsumoEstrada", "Consumos" },
            { "EntreEixos", "Dimensoes" }, { "Comprimento", "Dimensoes" }, { "Largura", "Dimensoes" }, { "Altura", "Dimensoes" },
            { "CapacidadeTanque", "Extras" }, { "TipoCombustivel", "Extras" }, { "CapacidadeCarga", "Extras" }
        };

        // Verifica se é uma propriedade aninhada (Cenário A) ou Raiz (Cenário B)
        bool ehAninhado = mapaColunas.TryGetValue(atributo, out string coluna);

        try
        {
            if (ehAninhado) return query.Where(ConstruirExpressaoAninhada(coluna, atributo, f.Operador, f.Valor));

            else return query.Where(ConstruirExpressaoRaiz(atributo, f.Operador, f.Valor));

        }
        catch
        {
            // Se o usuário mandar um atributo bizarro ou tipo incompatível, ignora o filtro e não quebra a API
            return query;
        }
    }

    // =======================================================================
    // O SEU MOTOR DE EXPRESSION TREES (Cenário A: Listas)
    // =======================================================================
    private Expression<Func<Carro, bool>> ConstruirExpressaoAninhada(string coluna, string atributo, string operador, object valor)
    {
        var carroParam = Expression.Parameter(typeof(Carro), "c");
        var colunaProp = Expression.Property(carroParam, coluna);              // c.Especificacoes
        var itemType = colunaProp.Type.GetGenericArguments()[0];

        var itemParam = Expression.Parameter(itemType, "e");
        var atributoProp = Expression.Property(itemParam, atributo);            // e.Potencia
        var fontesProp = Expression.Property(atributoProp, "Fontes");           // e.Potencia.Fontes
        var fonteItemType = fontesProp.Type.GetGenericArguments()[0];

        var fonteParam = Expression.Parameter(fonteItemType, "x");
        var valorProp = Expression.Property(fonteParam, "Valor");               // x.Valor

        var tipoAlvo = Nullable.GetUnderlyingType(valorProp.Type) ?? valorProp.Type;
        var constante = Expression.Constant(Convert.ChangeType(valor, tipoAlvo), valorProp.Type);

        Expression comparacao = operador switch
        {
            ">" => Expression.GreaterThan(valorProp, constante),
            "<" => Expression.LessThan(valorProp, constante),
            ">=" => Expression.GreaterThanOrEqual(valorProp, constante),
            "<=" => Expression.LessThanOrEqual(valorProp, constante),
            "==" => Expression.Equal(valorProp, constante),
            "!=" => Expression.NotEqual(valorProp, constante),
            _ => throw new NotSupportedException($"Operador '{operador}' inválido")
        };

        var fonteLambda = Expression.Lambda(comparacao, fonteParam);
        var anyFontes = Expression.Call(typeof(Enumerable), "Any", new[] { fonteItemType }, fontesProp, fonteLambda);
        var itemLambda = Expression.Lambda(anyFontes, itemParam);
        var anyColuna = Expression.Call(typeof(Enumerable), "Any", new[] { itemType }, colunaProp, itemLambda);

        return Expression.Lambda<Func<Carro, bool>>(anyColuna, carroParam);
    }

    // =======================================================================
    // MOTOR PARA PROPRIEDADES RAIZ (Cenário B: Categoria, Modos)
    // =======================================================================
    private Expression<Func<Carro, bool>> ConstruirExpressaoRaiz(string atributo, string operador, object valor)
    {
        var carroParam = Expression.Parameter(typeof(Carro), "c");
        var atributoProp = Expression.Property(carroParam, atributo);           // c.Categoria
        var fontesProp = Expression.Property(atributoProp, "Fontes");           // c.Categoria.Fontes
        var fonteItemType = fontesProp.Type.GetGenericArguments()[0];

        var fonteParam = Expression.Parameter(fonteItemType, "x");
        var valorProp = Expression.Property(fonteParam, "Valor");               // x.Valor

        var tipoAlvo = Nullable.GetUnderlyingType(valorProp.Type) ?? valorProp.Type;
        var constante = Expression.Constant(Convert.ChangeType(valor, tipoAlvo), valorProp.Type);

        // Adicionei suporte ao '.Contains()' para o array de 'Modos'
        Expression comparacao;
        if (operador.ToLower() == "contains" && valorProp.Type == typeof(List<string>))
        {
            var containsMethod = typeof(List<string>).GetMethod("Contains");
            comparacao = Expression.Call(valorProp, containsMethod!, constante);
        }
        else
        {
            comparacao = operador switch
            {
                "==" => Expression.Equal(valorProp, constante),
                "!=" => Expression.NotEqual(valorProp, constante),
                _ => throw new NotSupportedException($"Operador '{operador}' inválido para raiz")
            };
        }

        var fonteLambda = Expression.Lambda(comparacao, fonteParam);
        var anyFontes = Expression.Call(typeof(Enumerable), "Any", new[] { fonteItemType }, fontesProp, fonteLambda);

        return Expression.Lambda<Func<Carro, bool>>(anyFontes, carroParam);
    }
}