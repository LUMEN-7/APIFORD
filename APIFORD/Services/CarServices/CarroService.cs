using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Middleware;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.NotificationService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace APIFORD.Services.CarroServices;

public class CarroService : BaseService<Carro, CreateCarroDTO, ReadCarroDTO, UpdateCarroDTO, int>
{
    private readonly HelperService _helperService;
    private readonly NotificacaoService _notificacaoService;
    private readonly IImportadorArquivoService _importadorService;

    public CarroService(FordDbContext context, IMapper mapper, HelperService helperService,
        NotificacaoService notificacaoService, IImportadorArquivoService importadorService)
        : base(context, mapper)
    {
        _helperService = helperService;
        _notificacaoService = notificacaoService;
        _importadorService = importadorService;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    protected override async Task PostMappingListAsync(List<ReadCarroDTO> dtos, List<Carro> entities)
    {
        // Aqui nós chamamos a versão do método que aceita Listas!
        // Ele vai varrer todos os carros, ir ao banco uma vez só, e distribuir as fontes.
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dtos, entities);
    }

    private static readonly Dictionary<string, Func<Carro, object>> ColecoesAninhadas = new()
    {
        ["Especificacao"] = c => c.Especificacoes,
        ["Consumo"] = c => c.Consumos,
        ["Dimensao"] = c => c.Dimensoes,
        ["Pneu"] = c => c.Pneus,
        ["Extra"] = c => c.Extras,
    };


    private bool AplicarValorNaPropriedade(object alvo, PropertyInfo property, object valorBruto, string fonte, string? fonteId)
    {
        bool isEnvelope = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PropriedadeScraping<>);
        var targetType = isEnvelope ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;

        object valorLimpo;
        try
        {
            if (valorBruto is Newtonsoft.Json.Linq.JToken jToken) valorLimpo = jToken.ToObject(targetType);
            else if (valorBruto is System.Text.Json.JsonElement jsonElement) valorLimpo = jsonElement.Deserialize(targetType);
            else valorLimpo = Convert.ChangeType(valorBruto, targetType);
        }
        catch { return false; } // valor incompatível com o tipo da propriedade — ignora, igual ao comportamento original

        if (isEnvelope)
        {
            var tipoInterno = property.PropertyType.GetGenericArguments()[0];
            dynamic envelope = property.GetValue(alvo) ?? Activator.CreateInstance(property.PropertyType)!;

            var novaFonteType = typeof(ItemFonteScraping<>).MakeGenericType(tipoInterno);
            dynamic novaFonte = Activator.CreateInstance(novaFonteType)!;
            novaFonte.Valor = valorLimpo;
            novaFonte.Confianca = 1.0m;
            novaFonte.Fonte = fonte;
            novaFonte.FonteId = fonteId;

            if (envelope.Fontes == null)
                envelope.Fontes = Activator.CreateInstance(typeof(List<>).MakeGenericType(novaFonteType));
            envelope.Fontes.Add((object)novaFonte);
            envelope.Conflito = false;

            property.SetValue(alvo, (object)envelope);
        }
        else
        {
            property.SetValue(alvo, valorLimpo);
        }
        return true;
    }

    private List<string> AplicarAlteracoes(Carro carro, Dictionary<string, object> alteracoes, string fonte, string? fonteId)
    {
        var naoAplicados = new List<string>();
        foreach (var alteracao in alteracoes)
        {
            var (alvo, property) = LocalizarPropriedade(carro, alteracao.Key);
            if (property == null) { naoAplicados.Add(alteracao.Key); continue; }

            if (!AplicarValorNaPropriedade(alvo, property, alteracao.Value, fonte, fonteId))
                naoAplicados.Add(alteracao.Key);
        }
        return naoAplicados;
    }

    private (object alvo, PropertyInfo? property) LocalizarPropriedade(Carro carro, string nomePropriedade)
    {
        // 1) tenta direto em Carro (Modelo, Marca, Ano, ImagemUrl, Categoria, Modos)
        var direta = typeof(Carro).GetProperty(nomePropriedade, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (direta != null) return (carro, direta);

        // 2) procura nas 5 coleções aninhadas (Potencia -> Especificacao, Cidade -> Consumo, etc.)
        foreach (var (tipoNome, getColecao) in ColecoesAninhadas)
        {
            var tipoItem = tipoNome switch
            {
                "Especificacao" => typeof(Especificacao),
                "Consumo" => typeof(Consumo),
                "Dimensao" => typeof(Dimensao),
                "Pneu" => typeof(Pneu),
                "Extra" => typeof(Extra),
                _ => null
            };
            var prop = tipoItem?.GetProperty(nomePropriedade, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) continue;

            dynamic colecao = getColecao(carro);
            if (colecao.Count == 0)
            {
                dynamic novoItem = Activator.CreateInstance(tipoItem)!;
                colecao.Add(novoItem);
            }
            return (colecao[0], prop); // assume 1 item ativo por carro/versão — ver ressalva abaixo
        }

        return (carro, null);
    }

    private async Task<int> ResolverLinhagemIdAsync(string marca, string modelo, int ano)
    {
        var existente = await DbSet
            .Where(c => c.Marca.Trim().ToLower() == marca.Trim().ToLower()
                     && c.Modelo.Trim().ToLower() == modelo.Trim().ToLower()
                     && c.Ano == ano)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        return existente?.LinhagemId ?? 0; // 0 = sinal temporário de "carro novo"; vira o Id real logo abaixo
    }

    public async Task<List<ReadCarroDTO>> ListarMaisRecentesAsync(int pagina, int tamanhoPagina)
    {
        // 1. Subquery simples: só agregação (Max), sem Includes — isso o EF traduz sem problema
        var idsMaisRecentesQuery = DbSet
            .GroupBy(c => c.LinhagemId)
            .Select(g => g.Max(c => c.Id));

        // 2. Query principal: já com Includes, filtrando pelos Ids resolvidos acima,
        //    ordenando e paginando normalmente
        var carros = await DbSet
            .Where(c => idsMaisRecentesQuery.Contains(c.Id))
            .OrderBy(c => c.Marca).ThenBy(c => c.Modelo)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        var dtos = new List<ReadCarroDTO>();
        foreach (var carro in carros)
        {
            var dto = Mapper.Map<ReadCarroDTO>(carro);
            await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);
            dtos.Add(dto);
        }
        return dtos;
    }

    public override async Task<ReadCarroDTO> CreateAsync(CreateCarroDTO dto)
    {
        var carro = Mapper.Map<Carro>(dto);
        await _helperService.SincronizarEInjetarIdsDeFontesAsync(carro);

        await DbSet.AddAsync(carro);
        await Context.SaveChangesAsync();

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);

        return readCarroDto;
    }

    public override async Task<ReadCarroDTO> GetByIdAsync(int id)
    {
        var carro = await DbSet.FirstOrDefaultAsync(c => c.Id == id); // Zero Includes!
        if (carro == null) throw new NotFoundException("Veículo não encontrado.");

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);

        return readCarroDto;
    }

    public async Task<ReadCarroDTO> EditarPropriedadesAdminAsync(int carroId, Dictionary<string, object> alteracoes, string adminId)
    {
        if (alteracoes == null || alteracoes.Count == 0)
            throw new BadRequestException("Nenhuma alteração enviada no payload.");

        var carroOriginal = await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carroId);
        if (carroOriginal == null) throw new NotFoundException("Carro não encontrado.");

        var novaVersao = System.Text.Json.JsonSerializer.Deserialize<Carro>(System.Text.Json.JsonSerializer.Serialize(carroOriginal))!;
        novaVersao.Id = 0;
        novaVersao.LinhagemId = carroOriginal.LinhagemId;
        novaVersao.VersaoAnteriorId = carroOriginal.Id;
        novaVersao.DataCriacao = DateTime.UtcNow;

        AplicarAlteracoes(novaVersao, alteracoes, "Edição Manual", adminId);

        await DbSet.AddAsync(novaVersao);
        await Context.SaveChangesAsync();

        var readDto = Mapper.Map<ReadCarroDTO>(novaVersao);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, novaVersao);
        await _notificacaoService.NotificarAtualizacaoCarroAsync(novaVersao.LinhagemId, novaVersao.Marca, novaVersao.Modelo);
        return readDto;
    }


    public async Task<ReadCarroDTO> ImportarArquivoAsync(IFormFile arquivo, string userId)
    {
        if (arquivo == null || arquivo.Length == 0)
            throw new BadRequestException("Nenhum arquivo enviado.");

        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        Dictionary<string, object> dados = extensao switch
        {
            ".csv" => await _importadorService.ParseCsvAsync(arquivo),
            ".json" => await _importadorService.ParseJsonAsync(arquivo),
            ".xlsx" => await _importadorService.ParseXlsxAsync(arquivo),
            ".xml" => await _importadorService.ParseXmlAsync(arquivo),
            _ => throw new BadRequestException($"Formato '{extensao}' não suportado. Use CSV, JSON, XLSX ou XML.")
        };

        if (!dados.TryGetValue("marca", out var marcaObj) || !dados.TryGetValue("modelo", out var modeloObj) || !dados.TryGetValue("ano", out var anoObj))
            throw new BadRequestException("O arquivo precisa conter, no mínimo, Marca, Modelo e Ano.");

        var marca = marcaObj.ToString()!;
        var modelo = modeloObj.ToString()!;
        var ano = Convert.ToInt32(anoObj);

        var carro = new Carro { Marca = marca, Modelo = modelo, Ano = ano, DataCriacao = DateTime.UtcNow };
        carro.LinhagemId = await ResolverLinhagemIdAsync(marca, modelo, ano);

        var resto = dados.Where(kv => kv.Key is not ("marca" or "modelo" or "ano")).ToDictionary(kv => kv.Key, kv => kv.Value);
        var naoAplicados = AplicarAlteracoes(carro, resto, "Importação de Arquivo", userId);

        await DbSet.AddAsync(carro);
        await Context.SaveChangesAsync(); // gera o Id

        if (carro.LinhagemId == 0)
        {
            carro.LinhagemId = carro.Id; // primeira versão dessa linhagem
            await Context.SaveChangesAsync();
        }


        var readDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, carro);
        return readDto;
    }

    public async Task<(ReadCarroDTO Carro, bool HouveMudanca)> AtualizarCarroComNovosDadosAsync(int carroExistenteId, Carro carroComDadosNovos)
    {
        var versaoAtual = await DbSet
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)
            .AsNoTracking() // a versão atual é imutável, não pode ser alterada aqui
            .FirstOrDefaultAsync(c => c.Id == carroExistenteId);

        if (versaoAtual == null)
            throw new NotFoundException("Carro não encontrado.");

        // Clona a versão atual (via serialização, evita precisar de um profile novo no AutoMapper)
        var novaVersao = JsonSerializer.Deserialize<Carro>(JsonSerializer.Serialize(versaoAtual))!;
        novaVersao.Id = 0; // força o EF a tratar como inserção nova
        novaVersao.LinhagemId = versaoAtual.LinhagemId;
        novaVersao.VersaoAnteriorId = versaoAtual.Id;
        novaVersao.DataCriacao = DateTime.UtcNow;

        // Mesmo motor de diff de antes — só muda o que fazemos com o resultado
        bool houveMudanca = _helperService.MesclarDadosDeScraping(novaVersao, carroComDadosNovos);

        if (!houveMudanca)
        {
            var readDtoSemMudanca = Mapper.Map<ReadCarroDTO>(versaoAtual);
            return (readDtoSemMudanca, false);
        }

        await DbSet.AddAsync(novaVersao);
        await Context.SaveChangesAsync();

        await _notificacaoService.NotificarAtualizacaoCarroAsync(novaVersao.LinhagemId, novaVersao.Marca, novaVersao.Modelo);

        var readDto = Mapper.Map<ReadCarroDTO>(novaVersao);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, novaVersao);

        return (readDto, true);
    }

    public async Task<ReadCarroDTO?> ObterVersaoMaisRecenteAsync(int linhagemId)
    {
        var carro = await DbSet
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)// mesmos Includes de sempre
            .Where(c => c.LinhagemId == linhagemId)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (carro == null) throw new NotFoundException("Veículo não encontrado.");
        
        var dto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);

        if (dto == null) throw new InternalServerErrorException("Veículo não encontrado.");
        return dto;
    }

    public async Task<List<VersaoResumoDTO>> ListarVersoesAsync(int linhagemId)
    {
        var versoes = await DbSet
            .Where(c => c.LinhagemId == linhagemId)
            .OrderByDescending(c => c.Id)
            .Select(c => new VersaoResumoDTO { CarroId = c.Id, DataCriacao = c.DataCriacao })
            .ToListAsync();

        if (versoes.Any()) versoes[0].EhVersaoAtual = true;
        return versoes;
    }

    public async Task<ReadCarroDTO?> ObterPorIdExatoAsync(int carroId)
    {
        var carro = await DbSet.FirstOrDefaultAsync(c => c.Id == carroId);
        if (carro == null) return null;

        var dto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);
        return dto;
    }

    public async Task<object?> GetImagemByIdAsync(int carroId)
        => await DbSet.Where(c => c.Id == carroId).Select(c => c.ImagemUrl).FirstOrDefaultAsync();
    
}