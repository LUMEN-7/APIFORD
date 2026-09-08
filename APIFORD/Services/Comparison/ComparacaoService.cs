using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Comparison;
using APIFORD.Data.DTOS.Comparison.Bulk;
using APIFORD.Data.DTOS.Comparison.Direct;
using APIFORD.Middleware;
using APIFORD.Model.CarroClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APIFORD.Services.Comparison;

/// <summary>
/// Serviço responsável pelas regras de negócio de comparação entre carros,
/// contemplando dois cenários principais: comparação em grupo (BI de mercado, um carro
/// contra uma categoria/concorrentes) e comparação direta (embate 1x1 entre carros específicos).
/// Todos os cálculos estatísticos e de filtragem são feitos em C#/SQL, sem uso de LLM externo.
/// </summary>
public class ComparacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HelperService _helperService;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="ComparacaoService"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados Ford.</param>
    /// <param name="mapper">Mapeador AutoMapper para conversão entre entidades e DTOs.</param>
    /// <param name="helperService">Serviço auxiliar utilizado para preencher o catálogo de fontes nos DTOs de resposta.</param>
    public ComparacaoService(FordDbContext context, IMapper mapper, HelperService helperService)
    {
        _context = context;
        _mapper = mapper;
        _helperService = helperService;
    }

    // =======================================================================
    // MÉTODO 1: COMPARAÇÃO EM GRUPO (BI DE MERCADO)
    // =======================================================================
    /// <summary>
    /// Compara um carro base com os demais carros da mesma categoria (ou que atendam aos
    /// filtros avançados informados), calculando médias de mercado para atributos numéricos,
    /// modas para atributos categóricos e tendências para listas (ex: modos de condução).
    /// </summary>
    /// <param name="dto">
    /// Requisição contendo o Id do carro base, a categoria (opcional) e a lista de filtros
    /// avançados a serem aplicados na seleção dos concorrentes (limitado a 15 concorrentes).
    /// </param>
    /// <returns>
    /// Um <see cref="ComparacaoResponseDTO"/> com o carro base, os concorrentes encontrados,
    /// as médias/tendências da categoria e as conclusões (vantagem/desvantagem) do carro base
    /// em relação ao mercado.
    /// </returns>
    /// <exception cref="NotFoundException">Lançada quando o carro base informado não é encontrado.
    /// </exception>
    public async Task<ComparacaoResponseDTO?> GerarComparacaoEmGrupoAsync(ComparacaoRequestDTO dto)
    {
        var carroBase = await _context.Carros.FirstOrDefaultAsync(c => c.Id == dto.CarroBaseId);
        if (carroBase == null) throw new NotFoundException("Carro base não encontrado.");

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
        if (response == null) throw new NotFoundException("Carro base não encontrado.");
        return response;
    }

    // =======================================================================
    // MÉTODO 2: COMPARAÇÃO DIRETA (EMBATE 1x1)
    // =======================================================================
    /// <summary>
    /// Realiza uma comparação direta (embate) entre dois ou mais carros específicos,
    /// confrontando seus atributos numéricos (ex: potência, torque, consumo) e categóricos
    /// (ex: transmissão, tração, combustível) para determinar vencedores e diferenças.
    /// </summary>
    /// <param name="dto">Requisição contendo a lista de Ids dos carros a serem comparados (mínimo 2).</param>
    /// <returns>
    /// Um <see cref="ComparacaoDiretaResponseDTO"/> com os carros comparados e as conclusões
    /// de cada embate por atributo.
    /// </returns>
    /// <exception cref="BadRequestException">Lançada quando menos de 2 Ids de carros são informados.</exception>
    /// <exception cref="NotFoundException">Lançada quando nenhum carro é encontrado para os Ids informados.</exception>
    public async Task<ComparacaoDiretaResponseDTO?> GerarComparacaoDiretaAsync(ComparacaoDiretaRequestDTO dto)
    {
        if (dto.CarrosIds == null || dto.CarrosIds.Count < 2)
            throw new BadRequestException("É necessário informar pelo menos 2 carros.");

        var carros = await _context.Carros.Where(c => dto.CarrosIds.Contains(c.Id)).ToListAsync();
        if (carros.Count == 0) throw new NotFoundException("Nenhum carro encontrado para comparação.");

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
        if (response == null) throw new NotFoundException("Nenhum carro encontrado para comparação.");
        return response;
    }

    // =======================================================================
    // MOTORES DE ANÁLISE GENÉRICA (NÚMEROS, TEXTOS E LISTAS)
    // =======================================================================

    // --- GRUPOS ---
    /// <summary>
    /// Compara um valor numérico do carro base com a média do mesmo atributo em um grupo de
    /// carros, registrando a média calculada e uma conclusão indicando vantagem ou desvantagem
    /// percentual do carro base em relação à média.
    /// </summary>
    /// <param name="baseCar">Carro base a ser avaliado.</param>
    /// <param name="todos">Lista contendo o carro base e seus concorrentes.</param>
    /// <param name="medias">Dicionário de saída onde a média calculada é registrada.</param>
    /// <param name="conc">Dicionário de saída onde a conclusão (vantagem/desvantagem) é registrada.</param>
    /// <param name="nome">Nome do atributo (usado como chave nos dicionários e na mensagem).</param>
    /// <param name="un">Unidade de medida do atributo (ex: "cv", "km/l").</param>
    /// <param name="maiorMelhor"><c>true</c> se um valor maior representa vantagem; <c>false</c> caso contrário.</param>
    /// <param name="seletor">Função que extrai o valor numérico do atributo a partir de um <see cref="Carro"/>.</param>
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
 
    /// <summary>
    /// Compara um valor categórico (texto) do carro base com a moda estatística (valor mais
    /// frequente) do mesmo atributo em um grupo de carros, indicando se o carro base segue
    /// ou diverge do padrão de mercado.
    /// </summary>
    /// <param name="baseCar">Carro base a ser avaliado.</param>
    /// <param name="todos">Lista contendo o carro base e seus concorrentes.</param>
    /// <param name="medias">Dicionário de saída onde a moda (valor mais comum) é registrada.</param>
    /// <param name="conc">Dicionário de saída onde a conclusão (padrão/diferente) é registrada.</param>
    /// <param name="nome">Nome do atributo (usado como chave nos dicionários).</param>
    /// <param name="seletor">Função que extrai o valor categórico do atributo a partir de um <see cref="Carro"/>.</param>
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

    /// <summary>
    /// Compara uma lista de valores categóricos (textos) do carro base com a tendência estatística (os 3 valores mais
    /// frequentes) do mesmo atributo em um grupo de carros, indicando se o carro base segue
    /// ou diverge do padrão de mercado.
    /// </summary>
    /// <param name="baseCar">Carro base a ser avaliado.</param>
    /// <param name="todos">Lista contendo o carro base e seus concorrentes.</param>
    /// <param name="medias">Dicionário de saída onde a tendência (3 valores mais comuns) é registrada.</param>
    /// <param name="conc">Dicionário de saída onde a conclusão (alinhado/exclusivo) é registrada.</param>
    /// <param name="nome">Nome do atributo (usado como chave nos dicionários).</param>
    /// <param name="seletor">Função que extrai a lista de valores categóricos do atributo a partir de um <see cref="Carro"/>.</param>
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
    /// <summary>
    /// Realiza o embate numérico de um atributo entre múltiplos carros, determinando o vencedor
    /// (maior ou menor valor, conforme o atributo) e a vantagem percentual sobre o pior colocado.
    /// </summary>
    /// <param name="carros">Lista de carros participantes do embate.</param>
    /// <param name="conc">Dicionário de saída onde a conclusão do embate é registrada.</param>
    /// <param name="nome">Nome do atributo (usado como chave no dicionário).</param>
    /// <param name="un">Unidade de medida do atributo.</param>
    /// <param name="maiorMelhor"><c>true</c> se o maior valor vence; <c>false</c> se o menor valor vence.</param>
    /// <param name="seletor">Função que extrai o valor numérico do atributo a partir de um <see cref="Carro"/>.</param>
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

    /// <summary>
    /// Realiza o embate categórico de um atributo entre múltiplos carros, determinando se todos os carros
    /// compartilham o mesmo valor ou se há diferenças significativas.
    /// </summary>
    /// <param name="carros">Lista de carros participantes do embate.</param>
    /// <param name="conc">Dicionário de saída onde a conclusão do embate é registrada.</param>
    /// <param name="nome">Nome do atributo (usado como chave no dicionário).</param>
    /// <param name="seletor">Função que extrai o valor categórico do atributo a partir de um <see cref="Carro"/>.</param>
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
    
    /// <summary>
    /// Aplica um filtro avançado à consulta de carros, direcionando-o para o motor de expressões
    /// adequado conforme o atributo seja uma propriedade aninhada (ex: Especificações, Consumos)
    /// ou uma propriedade raiz do carro (ex: Categoria, Modos).
    /// </summary>
    /// <param name="query">Consulta de carros à qual o filtro será aplicado.</param>
    /// <param name="f">Filtro contendo o atributo, operador e valor a serem aplicados.</param>
    /// <returns>A consulta com o filtro aplicado, ou a consulta original caso o filtro seja inválido/incompatível.</returns>
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
    
    /// <summary>
    /// Constrói dinamicamente, via Expression Trees, uma expressão de filtro para atributos
    /// aninhados dentro de coleções do carro (ex: <c>Especificacoes[].Potencia.Fontes[].Valor</c>),
    /// permitindo comparações numéricas (&gt;, &lt;, &gt;=, &lt;=, ==, !=) que são traduzidas para SQL.
    /// </summary>
    /// <param name="coluna">Nome da coleção de navegação do carro (ex: "Especificacoes", "Consumos").</param>
    /// <param name="atributo">Nome do atributo dentro do item da coleção (ex: "Potencia").</param>
    /// <param name="operador">Operador de comparação a ser aplicado (ex: "&gt;", "==").</param>
    /// <param name="valor">Valor a ser comparado com o atributo.</param>
    /// <returns>Uma expressão lambda <see cref="Expression{TDelegate}"/> utilizável em cláusulas <c>Where</c> do EF Core.</returns>
    /// <exception cref="BadRequestException">Lançada quando o operador informado é inválido.</exception>
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
            _ => throw new BadRequestException($"Operador '{operador}' inválido")
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
    
    /// <summary>
    /// Constrói dinamicamente, via Expression Trees, uma expressão de filtro para atributos
    /// que ficam diretamente na raiz do carro (ex: <c>Categoria.Fontes[].Valor</c>,
    /// <c>Modos.Fontes[].Valor</c>), suportando comparações de igualdade/diferença e o
    /// operador <c>Contains</c> para listas de strings (ex: modos de condução).
    /// </summary>
    /// <param name="atributo">Nome da propriedade raiz do carro (ex: "Categoria", "Modos").</param>
    /// <param name="operador">Operador de comparação a ser aplicado (ex: "==", "!=", "contains").</param>
    /// <param name="valor">Valor a ser comparado com o atributo.</param>
    /// <returns>Uma expressão lambda <see cref="Expression{TDelegate}"/> utilizável em cláusulas <c>Where</c> do EF Core.</returns>
    /// <exception cref="BadRequestException">Lançada quando o operador informado é inválido para propriedades raiz.</exception>
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
                _ => throw new BadRequestException($"Operador '{operador}' inválido para raiz")
            };
        }

        var fonteLambda = Expression.Lambda(comparacao, fonteParam);
        var anyFontes = Expression.Call(typeof(Enumerable), "Any", new[] { fonteItemType }, fontesProp, fonteLambda);

        return Expression.Lambda<Func<Carro, bool>>(anyFontes, carroParam);
    }
}