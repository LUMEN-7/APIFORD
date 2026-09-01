// APIFORD/Services/Search/PesquisaService.cs
using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Search;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
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

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PesquisaService(HelperService helperService, HttpClient httpClient, FordDbContext context, IMapper mapper, NotificacaoService notificacaoService)
    {
        _helperService = helperService;
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
        _notificacaoService = notificacaoService;
    }

    // -------------------------------------------------------------------------
    // POST /Pesquisa/busca
    // Envia o pedido ao Python e retorna o job_id imediatamente (~5ms)
    // -------------------------------------------------------------------------
    public async Task<Guid> IniciarBusca(BuscaDTO dto)
    {
        var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/specs", dto);

        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Microserviço Python indisponível.");

        var payload = await response.Content.ReadFromJsonAsync<PythonJobResponse>(_jsonOptions);

        if (payload == null)
            throw new ApplicationException("Python não retornou um job_id válido.");

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
            return new JobStatusDTO { Status = "not_found" };

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
        var createCarroDto = JsonSerializer.Deserialize<CreateCarroDTO>(job.Result!, _jsonOptions);
        if (createCarroDto == null)
            throw new ApplicationException("Resultado do job está vazio ou inválido.");

        var carroEntity = _mapper.Map<Carro>(createCarroDto);

        // Toda a iteração de Context.Modos foi removida. O C# confia no JSON!

        await _helperService.SincronizarEInjetarIdsDeFontesAsync(carroEntity);

        await _context.Carros.AddAsync(carroEntity);
        await _context.SaveChangesAsync();

        // Busca o carro final sem precisar de nenhum JOIN/Include
        var carroCompleto = await _context.Carros
            .FirstOrDefaultAsync(c => c.Id == carroEntity.Id);

        if (carroCompleto == null)
            throw new KeyNotFoundException("Veículo processado mas não encontrado no banco.");

        var readCarroDto = _mapper.Map<ReadCarroDTO>(carroCompleto);
        await _helperService.PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carroCompleto);
        await _notificacaoService.NotificarAtualizacaoCarroAsync(carroCompleto.Id, carroCompleto.Marca, carroCompleto.Modelo);
        return readCarroDto;
    }
}

// DTO interno refatorado para int
internal record PythonJobResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("job_id")]
    public Guid JobId { get; init; }
}