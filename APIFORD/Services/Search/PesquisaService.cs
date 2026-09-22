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

    private static bool EstaFresco(Carro carro)
    {
        var idade = DateTime.UtcNow - carro.DataCriacao;
        var anosDesdeOModelo = DateTime.UtcNow.Year - carro.Ano;

        if (anosDesdeOModelo <= 1) return idade < TimeSpan.FromDays(90);
        if (anosDesdeOModelo <= 3) return idade < TimeSpan.FromDays(180);
        return idade < TimeSpan.FromDays(365);
    }

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
            return await IniciarBusca(dto, userId);

        var chave = NormalizarChaveBusca(dto.Brand, dto.Model, dto.Year);
        var lockHash = SHA256.HashData(Encoding.UTF8.GetBytes(chave));
        var lockKey = BitConverter.ToInt64(lockHash, 0);
        await _context.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock({lockKey})");

        var carroExistente = await _context.Carros
            .Where(c => c.Marca == dto.Brand && c.Modelo == dto.Model && c.Ano == dto.Year)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (carroExistente != null && EstaFresco(carroExistente))
        {
            var jobDeCache = new Job
            {
                Status = "done",
                Payload = JsonSerializer.Serialize(dto),
                Result = JsonSerializer.Serialize(new JobResultDTO { CarroId = carroExistente.Id }),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                CarroId = carroExistente.Id,
                Notificado = true
            };

            await _context.Jobs.AddAsync(jobDeCache);
            await _context.SaveChangesAsync();

            return jobDeCache.Id;
        }

        var limite = DateTime.UtcNow.AddHours(-2);
        var jobsAbertos = await _context.Jobs
            .Where(j =>
                (j.Status == "pending" || j.Status == "running" || j.Status == "processing") &&
                j.CreatedAt >= limite)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        var jobAberto = jobsAbertos.FirstOrDefault(j => ChaveDoPayload(j.Payload) == chave);
        if (jobAberto != null)
            return jobAberto.Id;

        return await IniciarBusca(dto, userId);
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

        if (job.Status != "done" || job.CarroId == null)
            return new JobStatusDTO { Status = job.Status, Error = job.Error };

        var carro = await _carroService.GetByIdAsync(job.CarroId.Value);
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
