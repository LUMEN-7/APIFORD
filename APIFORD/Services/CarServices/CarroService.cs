using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDto;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
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

    public CarroService(FordDbContext context, IMapper mapper, HelperService helperService, NotificacaoService notificacaoService)
        : base(context, mapper)
    {
        _helperService = helperService;
        _notificacaoService = notificacaoService;
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


    public override async Task<ReadCarroDTO> CreateAsync(CreateCarroDTO dto)
    {
        var carro = Mapper.Map<Carro>(dto);
        await _helperService.SincronizarEInjetarIdsDeFontesAsync(carro);

        await DbSet.AddAsync(carro);
        await Context.SaveChangesAsync();

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);
        await _notificacaoService.NotificarAtualizacaoCarroAsync(carro.Id, carro.Marca, carro.Modelo);

        return readCarroDto;
    }

    public override async Task<ReadCarroDTO> GetByIdAsync(int id)
    {
        var carro = await DbSet.FirstOrDefaultAsync(c => c.Id == id); // Zero Includes!
        if (carro == null) throw new KeyNotFoundException("Veículo não encontrado.");

        var readCarroDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carro);

        return readCarroDto;
    }

    public async Task<ReadCarroDTO> EditarPropriedadesAdminAsync(int carroId, Dictionary<string, object> alteracoes, int adminId)
    {
        var carroOriginal = await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carroId);
        if (carroOriginal == null) throw new KeyNotFoundException("Carro não encontrado.");

        var novaVersao = System.Text.Json.JsonSerializer.Deserialize<Carro>(System.Text.Json.JsonSerializer.Serialize(carroOriginal))!;
        novaVersao.Id = 0;
        novaVersao.LinhagemId = carroOriginal.LinhagemId;
        novaVersao.VersaoAnteriorId = carroOriginal.Id;
        novaVersao.DataCriacao = DateTime.UtcNow;

        foreach (var alteracao in alteracoes)
        {
            var property = typeof(Carro).GetProperty(alteracao.Key, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property == null) continue;

            bool isEnvelope = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PropriedadeScraping<>);
            var targetType = isEnvelope ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;

            object valorLimpo = null;
            try
            {
                if (alteracao.Value is Newtonsoft.Json.Linq.JToken jToken)
                    valorLimpo = jToken.ToObject(targetType);
                else if (alteracao.Value is System.Text.Json.JsonElement jsonElement)
                    valorLimpo = jsonElement.Deserialize(targetType);
                else
                    valorLimpo = Convert.ChangeType(alteracao.Value, targetType);
            }
            catch
            {
                continue;
            }

            if (isEnvelope)
            {
                var tipoInterno = property.PropertyType.GetGenericArguments()[0];
                dynamic envelope = property.GetValue(novaVersao) ?? Activator.CreateInstance(property.PropertyType)!;

                var novaFonteType = typeof(ItemFonteScraping<>).MakeGenericType(tipoInterno);
                dynamic novaFonte = Activator.CreateInstance(novaFonteType)!;

                novaFonte.Valor = valorLimpo;
                novaFonte.Confianca = 1.0m;
                novaFonte.Fonte = "Edição Manual";
                novaFonte.FonteId = adminId;

                if (envelope.Fontes == null)
                {
                    var listaType = typeof(List<>).MakeGenericType(novaFonteType);
                    envelope.Fontes = Activator.CreateInstance(listaType);
                }
                envelope.Fontes.Add((object)novaFonte);
                envelope.Conflito = false;

                property.SetValue(novaVersao, (object)envelope);
            }
            else
            {
                property.SetValue(novaVersao, valorLimpo);
            }
        }

        await DbSet.AddAsync(novaVersao);
        await Context.SaveChangesAsync();

        var readDto = Mapper.Map<ReadCarroDTO>(novaVersao);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, novaVersao);
        await _notificacaoService.NotificarAtualizacaoCarroAsync(novaVersao.LinhagemId, novaVersao.Marca, novaVersao.Modelo);
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
            throw new KeyNotFoundException("Carro não encontrado.");

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

        if (carro == null) return null;
        var dto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(dto, carro);
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
}