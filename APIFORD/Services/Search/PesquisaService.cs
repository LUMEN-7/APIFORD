// APIFORD/Services/Search/PesquisaService.cs
using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Search;
using APIFORD.Middleware;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Services.CarroServices;
using APIFORD.Services.NotificationService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIFORD.Services.Search;

public class PesquisaService
{
    private readonly HttpClient _httpClient;
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly HelperService _helperService;
    private readonly NotificacaoService _notificacaoService;
    private readonly CarroService _carroService;
    private readonly IConfiguration _configuration;

    private string _pythonServiceUrl => _configuration["Python:Url"];


    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PesquisaService(HelperService helperService, IConfiguration configuration, HttpClient httpClient, FordDbContext context, IMapper mapper, NotificacaoService notificacaoService, CarroService carroService)
    {
        _helperService = helperService;
        _configuration = configuration;
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
        _notificacaoService = notificacaoService;
        _carroService = carroService;
    }

    private static bool EstaFresco(Carro carro)
    {
        var idade = DateTime.UtcNow - carro.DataCriacao;
        var anosDesdeOModelo = DateTime.UtcNow.Year - carro.Ano;

        if (anosDesdeOModelo <= 1) return idade < TimeSpan.FromDays(90);
        if (anosDesdeOModelo <= 3) return idade < TimeSpan.FromDays(180);
        return idade < TimeSpan.FromDays(365);
    }

    private static bool FonteVazia<T>(PropriedadeScraping<T>? prop)
    {
        if (prop?.Fontes == null || prop.Fontes.Count == 0)
            return true;

        return prop.Fontes.All(f =>
            f.Valor is null
            || f.Valor is string s && string.IsNullOrWhiteSpace(s)
            || f.Valor is IEnumerable<string> lista && !lista.Any());
    }

    private static double PercentualCamposVazios(Carro c)
    {
        var campos = new List<bool>();

        void Add<T>(PropriedadeScraping<T>? p) => campos.Add(FonteVazia(p));

        // raiz do Carro
        Add(c.Descricao);
        Add(c.Preco);
        Add(c.Modos);
        Add(c.Categoria);
        campos.Add(string.IsNullOrWhiteSpace(c.ImagemUrl));

        // Especificacao — Potencia, Torque, PotenciaRpm, TorqueRpm, Transmissao, Motor, Tracao
        var spec = c.Especificacoes?.FirstOrDefault() ?? new Especificacao();
        Add(spec.Potencia);
        Add(spec.Torque);
        Add(spec.PotenciaRpm);
        Add(spec.TorqueRpm);
        Add(spec.Transmissao);
        Add(spec.Motor);
        Add(spec.Tracao);

        // Consumo — Cidade, Estrada
        var cons = c.Consumos?.FirstOrDefault() ?? new Consumo();
        Add(cons.Cidade);
        Add(cons.Estrada);

        // Dimensao — Comprimento, Largura, Altura, EntreEixos
        var dim = c.Dimensoes?.FirstOrDefault() ?? new Dimensao();
        Add(dim.Comprimento);
        Add(dim.Largura);
        Add(dim.Altura);
        Add(dim.EntreEixos);

        // Pneu — Tipo, Aro, Largura, Perfil
        var pneu = c.Pneus?.FirstOrDefault() ?? new Pneu();
        Add(pneu.Tipo);
        Add(pneu.Aro);
        Add(pneu.Largura);
        Add(pneu.Perfil);

        // Extra — tanque, combustível, carga, reboque, Performance, Seguranca, Conforto, Tecnologia
        var extra = c.Extras?.FirstOrDefault() ?? new Extra();
        Add(extra.CapacidadeTanque);
        Add(extra.TipoCombustivel);
        Add(extra.CapacidadeCarga);
        Add(extra.CapacidadeReboque);
        Add(extra.Performance);
        Add(extra.Seguranca);
        Add(extra.Conforto);
        Add(extra.Tecnologia);

        return campos.Count == 0 ? 1 : campos.Count(v => v) / (double)campos.Count;
    }

    private static bool PrecisaAtualizar(Carro c)
    {
        if (!EstaFresco(c)) return true;
        return PercentualCamposVazios(c) >= 0.30;
    }

    private static string NormalizarChaveBusca(string? marca, string? modelo, int? ano)
    {
        static string Limpar(string? valor) => (valor ?? string.Empty).Trim().ToLowerInvariant();
        return $"{Limpar(marca)}|{Limpar(modelo)}|{ano?.ToString() ?? ""}";
    }

    private static string ChaveDoPayload(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload)) return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            string Pegar(params string[] nomes)
            {
                foreach (var nome in nomes)
                {
                    if (!root.TryGetProperty(nome, out var prop)) continue;
                    return prop.ValueKind == JsonValueKind.Number
                        ? prop.GetRawText()
                        : (prop.GetString() ?? "");
                }
                return "";
            }

            var marca = Pegar("brand", "Brand", "marca");
            var modelo = Pegar("model", "Model", "modelo");
            var anoTexto = Pegar("year", "Year", "ano");
            int? ano = int.TryParse(anoTexto, out var n) && n > 0 ? n : null;
            return NormalizarChaveBusca(marca, modelo, ano);
        }
        catch
        {
            return payload ?? string.Empty;
        }
    }

    // -------------------------------------------------------------------------
    // POST /Pesquisa/busca
    // Envia o pedido ao Python e retorna o job_id imediatamente (~5ms)
    // -------------------------------------------------------------------------
    public async Task<Guid> BuscarOuIniciarAsync(BuscaDTO dto, string userId, bool forcarNovaBusca = false)
    {
        if (forcarNovaBusca)
        {
            var novo = await IniciarBusca(dto, userId);
            await _notificacaoService.RegistrarAcompanhamentoBuscaAsync(userId, novo, dto.Brand ?? "", dto.Model ?? "");
            return novo;
        }

        var chave = NormalizarChaveBusca(dto.Brand, dto.Model, dto.Year);
        var lockHash = SHA256.HashData(Encoding.UTF8.GetBytes(chave));
        var lockKey = BitConverter.ToInt64(lockHash, 0);

        var strategy = _context.Database.CreateExecutionStrategy();

        var jobExistente = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock({lockKey})");

            var marca = (dto.Brand ?? "").Trim().ToLower();
            var modelo = (dto.Model ?? "").Trim().ToLower();

            var recente = await _context.Carros
                .Where(c => !c.Excluido
                    && c.Marca.ToLower() == marca
                    && c.Modelo.ToLower() == modelo
                    && (!dto.Year.HasValue || c.Ano == dto.Year.Value))
                .OrderByDescending(c => c.DataCriacao)
                .FirstOrDefaultAsync();

            if (recente != null && !PrecisaAtualizar(recente))
            {
                var jobDeCache = new Job
                {
                    Status = "done",
                    Payload = JsonSerializer.Serialize(dto),
                    Result = JsonSerializer.Serialize(new JobResultDTO { CarroId = recente.Id }),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId,
                    CarroId = recente.Id,
                    Notificado = false
                };
                await _context.Jobs.AddAsync(jobDeCache);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return jobDeCache.Id;
            }

            var limite = DateTime.UtcNow.AddHours(-2);
            var jobsAbertos = await _context.Jobs
                .Where(j =>
                    (j.Status == "pending" || j.Status == "running" || j.Status == "processing") &&
                    j.CreatedAt >= limite)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var aberto = jobsAbertos.FirstOrDefault(j => ChaveDoPayload(j.Payload) == chave);
            await transaction.CommitAsync();
            return aberto?.Id;
        });

        var jobId = jobExistente ?? await IniciarBusca(dto, userId);
        await _notificacaoService.RegistrarAcompanhamentoBuscaAsync(userId, jobId, dto.Brand ?? "", dto.Model ?? "");
        return jobId;
    }

    public async Task<Guid> IniciarBusca(BuscaDTO dto, string userId)
    {

        var a = JsonContent.Create(dto);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_pythonServiceUrl}/specs")
        {
            Content = a
        };
        request.Headers.Add("X-Api-Key", _configuration["PythonInternalApiKey"]);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var corpo = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Python respondeu {(int)response.StatusCode} {response.StatusCode}: {corpo}");
            throw new ServiceUnavailableException("Microserviço Python indisponível.");
        }

        var payload = await response.Content.ReadFromJsonAsync<PythonJobResponse>(_jsonOptions);

        if (payload == null)
            throw new ExternalServiceException("Python", " não retornou um job_id válido.");

        // O job já deve ter sido criado no banco pelo próprio Python (é de lá que vem o job_id).
        // Aqui só localizamos esse registro e completamos o UserId, que o Python não tem como saber.
        var job = await _context.Jobs.FindAsync(payload.JobId);
        if (job != null)
        {
            job.UserId = userId;
            await _context.SaveChangesAsync();
        }

        return payload.JobId;
    }

    public async Task<Guid> IniciarBusca(BuscaDTO dto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_pythonServiceUrl}/specs")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Add("X-Api-Key", _configuration["PythonInternalApiKey"]); // ⚠️ nome do header/config — confirma com o Python

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var corpo = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Python respondeu {(int)response.StatusCode} {response.StatusCode}: {corpo}");
            throw new ServiceUnavailableException("Microserviço Python indisponível.");
        }

        var payload = await response.Content.ReadFromJsonAsync<PythonJobResponse>(_jsonOptions);

        if (payload == null)
            throw new ExternalServiceException("Python", " não retornou um job_id válido.");

        // O job já deve ter sido criado no banco pelo próprio Python (é de lá que vem o job_id).
        // Aqui só localizamos esse registro e completamos o UserId, que o Python não tem como saber.
        var job = await _context.Jobs.FindAsync(payload.JobId);
        return payload.JobId;
    }

    // -------------------------------------------------------------------------
    // GET /Pesquisa/jobs/{id}
    // Checa o status do job. Se done, processa e salva o carro no banco.
    // -------------------------------------------------------------------------
    public async Task<JobStatusDTO> ChecarJob(Guid jobId, string userId)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job == null) throw new NotFoundException($"Job {jobId} não encontrado.");

        if (job.Status != "done")
            return new JobStatusDTO { Status = job.Status, Error = job.Error };

        ReadCarroDTO carro;
        if (job.CarroId == null)
        {
            if (string.IsNullOrWhiteSpace(job.Result))
                return new JobStatusDTO { Status = job.Status, Error = job.Error };

            carro = await ProcessarJobPendenteAsync(job);
            await _context.SaveChangesAsync();
        }
        else
        {
            carro = await _carroService.GetByIdAsync(job.CarroId.Value);
        }

        if (!job.Notificado)
        {
            job.Notificado = true;
            await _context.SaveChangesAsync();
            await _notificacaoService.NotificarBuscaConcluidaDoJobAsync(job.Id, carro);
        }

        return new JobStatusDTO { Status = "done", Carro = carro };
    }

    public async Task<ReadCarroDTO> ProcessarJobPendenteAsync(Job job)
    {
        var readDto = await ProcessarESalvarCarro(job);
        job.CarroId = readDto.Id;
        job.Result = null;
        job.UpdatedAt = DateTime.UtcNow;
        return readDto;
    }

    // -------------------------------------------------------------------------
    // Processamento do C# - Refatorado para o padrão JSONB
    // -------------------------------------------------------------------------
    private async Task<ReadCarroDTO> ProcessarESalvarCarro(Job job)
    {
        var resultado = JsonSerializer.Deserialize<JobResultDTO>(job.Result!, _jsonOptions);
        if (resultado == null || resultado.CarroId <= 0)
            throw new ExternalServiceException("Python", "Resultado do job não trouxe um CarroId válido.");

        var carro = await _context.Carros.FirstOrDefaultAsync(c => c.Id == resultado.CarroId);
        if (carro == null)
            throw new NotFoundException($"Python reportou CarroId {resultado.CarroId}, mas ele não foi encontrado no banco.");

        var readDto = _mapper.Map<ReadCarroDTO>(carro);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, carro);
        return readDto;
    }
}

internal record PythonJobResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("job_id")]
    public Guid JobId { get; init; }
}

internal record JobResultDTO
{
    public int CarroId { get; init; }
}
