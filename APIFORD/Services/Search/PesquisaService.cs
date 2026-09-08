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

    private static bool EstaFresco(Carro carro)
    {
        var idade = DateTime.UtcNow - carro.DataCriacao;
        var anosDesdeOModelo = DateTime.UtcNow.Year - carro.Ano;

        if (anosDesdeOModelo <= 1) return idade < TimeSpan.FromDays(90);   // ano atual/recente: ainda muda preço com frequência, ~3 meses
        if (anosDesdeOModelo <= 3) return idade < TimeSpan.FromDays(180);  // já estabilizado, ~6 meses
        return idade < TimeSpan.FromDays(365);                            // carro antigo: dificilmente muda, ~12 meses
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PesquisaService(HelperService helperService, HttpClient httpClient, FordDbContext context, IMapper mapper, NotificacaoService notificacaoService, CarroService carroService)
    {
        _helperService = helperService;
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
        _notificacaoService = notificacaoService;
        _carroService = carroService;
    }

    // -------------------------------------------------------------------------
    // POST /Pesquisa/busca
    // Envia o pedido ao Python e retorna o job_id imediatamente (~5ms)
    // -------------------------------------------------------------------------
    public async Task<Guid> BuscarOuIniciarAsync(BuscaDTO dto, bool forcarNovaBusca = false)
    {
        if (!forcarNovaBusca)
        {
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
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Jobs.AddAsync(jobDeCache);
                await _context.SaveChangesAsync();

                return jobDeCache.Id;
            }
        }

        return await IniciarBusca(dto);
    }

    public async Task<Guid> IniciarBusca(BuscaDTO dto)
    {
        var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/specs", dto);

        if (!response.IsSuccessStatusCode)
            throw new ServiceUnavailableException("Microserviço Python indisponível.");

        var payload = await response.Content.ReadFromJsonAsync<PythonJobResponse>(_jsonOptions);

        if (payload == null)
            throw new ExternalServiceException("Python", " não retornou um job_id válido.");

        return payload.JobId;
    }

    // -------------------------------------------------------------------------
    // GET /Pesquisa/jobs/{id}
    // Checa o status do job. Se done, processa e salva o carro no banco.
    // -------------------------------------------------------------------------
    public async Task<JobStatusDTO> ChecarJob(Guid jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            throw new NotFoundException($"Job {jobId} não encontrado.");

        if (job.Status is "pending" or "running" or "error")
            return new JobStatusDTO { Status = job.Status, Error = job.Error };

        // Se done mas o carro já foi processado (Result limpo anteriormente)
        if (job.Status == "done" && string.IsNullOrWhiteSpace(job.Result))
            return new JobStatusDTO { Status = "done" };

        // Processa o resultado apenas na primeira vez que identifica como done
        if (job.Status == "done" && !string.IsNullOrWhiteSpace(job.Result))
        {
            var readCarroDto = await ProcessarESalvarCarro(job);

            // Limpa o payload gigante de resultado para poupar espaço no banco
            job.Result = null;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new JobStatusDTO { Status = "done", Carro = readCarroDto };
        }

        return new JobStatusDTO { Status = job.Status };
    }

    // -------------------------------------------------------------------------
    // Processamento do C# - Refatorado para o padrão JSONB
    // -------------------------------------------------------------------------
   private async Task<ReadCarroDTO> ProcessarESalvarCarro(Job job)
{
    var resultado = JsonSerializer.Deserialize<JobResultDTO>(job.Result!, _jsonOptions);
    if (resultado == null || resultado.CarroId <= 0)
        throw new ExternalServiceException("Python","Resultado do job não trouxe um CarroId válido.");

    var carro = await _context.Carros.FirstOrDefaultAsync(c => c.Id == resultado.CarroId);
    if (carro == null)
        throw new NotFoundException($"Python reportou CarroId {resultado.CarroId}, mas ele não foi encontrado no banco.");

    var readDto = _mapper.Map<ReadCarroDTO>(carro);
    await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readDto, carro);
    return readDto;
}
}

// DTO interno refatorado para int
internal record PythonJobResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("job_id")]
    public Guid JobId { get; init; }
}

internal record JobResultDTO
{
    public int CarroId { get; init; }
}