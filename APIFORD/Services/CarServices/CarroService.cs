using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.NotificationService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        var carro = await DbSet.FirstOrDefaultAsync(c => c.Id == carroId);
        if (carro == null) throw new KeyNotFoundException("Carro não encontrado.");

        foreach (var alteracao in alteracoes)
        {
            var property = typeof(Carro).GetProperty(alteracao.Key, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property == null) continue;

            // 1. Descobrimos se a propriedade é um Envelope (PropriedadeScraping) ou uma propriedade normal (string, int)
            bool isEnvelope = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PropriedadeScraping<>);

            // 2. Descobrimos qual é o tipo final que precisamos (ex: 'string' para o Modelo, 'List<string>' para os Modos)
            var targetType = isEnvelope ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;

            object valorLimpo = null;

            try
            {
                // ====================================================================
                // O MOTOR DE CONVERSÃO UNIVERSAL (Lida com qualquer formato que o front mandar)
                // ====================================================================

                // Cenário A: O ASP.NET usou Newtonsoft.Json (JToken, JObject, JValue)
                if (alteracao.Value is Newtonsoft.Json.Linq.JToken jToken)
                {
                    valorLimpo = jToken.ToObject(targetType);
                }
                // Cenário B: O ASP.NET usou System.Text.Json (JsonElement)
                else if (alteracao.Value is System.Text.Json.JsonElement jsonElement)
                {
                    valorLimpo = jsonElement.Deserialize(targetType);
                }
                // Cenário C: Veio como tipo primitivo puro (ex: a própria string "Mustang Mach-E91")
                else
                {
                    valorLimpo = Convert.ChangeType(alteracao.Value, targetType);
                }
            }
            catch
            {
                // Se houver qualquer falha bizarra de conversão num campo, ele ignora e salva o resto!
                continue;
            }

            // ====================================================================
            // APLICAÇÃO DO VALOR
            // ====================================================================
            if (isEnvelope)
            {
                var tipoInterno = property.PropertyType.GetGenericArguments()[0];
                dynamic envelope = property.GetValue(carro) ?? Activator.CreateInstance(property.PropertyType)!;

                // Em vez de 'envelope.Valor' (que não existe), nós criamos uma FONTE DO ADMIN!
                var novaFonteType = typeof(ItemFonteScraping<>).MakeGenericType(tipoInterno);
                dynamic novaFonte = Activator.CreateInstance(novaFonteType)!;

                novaFonte.Valor = valorLimpo;
                novaFonte.Confianca = 1.0m; // Confiança máxima
                novaFonte.Fonte = "Edição Manual";
                novaFonte.FonteId = adminId;

                // Adicionamos a fonte ao envelope
                if (envelope.Fontes == null)
                {
                    var listaType = typeof(List<>).MakeGenericType(novaFonteType);
                    envelope.Fontes = Activator.CreateInstance(listaType);
                }
                envelope.Fontes.Add((object)novaFonte);
                envelope.Conflito = false; // Como o admin editou, resolvemos qualquer conflito

                property.SetValue(carro, (object)envelope);
            }
            else
            {
                // Propriedades comuns (Modelo, Ano)
                property.SetValue(carro, valorLimpo);
            }
        }

        // Salva e atualiza!
        await Context.SaveChangesAsync();
        var readDto = Mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, carro);
        await _notificacaoService.NotificarAtualizacaoCarroAsync(carro.Id, carro.Marca, carro.Modelo);
        return readDto;
    }

    public async Task<(ReadCarroDTO Carro, bool HouveMudanca)> AtualizarCarroComNovosDadosAsync(int carroExistenteId, Carro carroNovo)
    {
        var carroExistente = await DbSet
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)
            .FirstOrDefaultAsync(c => c.Id == carroExistenteId);

        if (carroExistente == null)
            throw new KeyNotFoundException("Carro não encontrado.");

        bool houveMudanca = _helperService.MesclarDadosDeScraping(carroExistente, carroNovo);

        if (houveMudanca)
        {
            await Context.SaveChangesAsync();
            await _notificacaoService.NotificarAtualizacaoCarroAsync(carroExistente.Id, carroExistente.Marca, carroExistente.Modelo);
        }

        var readDto = Mapper.Map<ReadCarroDTO>(carroExistente);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, carroExistente);

        return (readDto, houveMudanca);
    }
}