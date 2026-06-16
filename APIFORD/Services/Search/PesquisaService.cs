// APIFORD/Services/Search/PesquisaService.cs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Search;
using APIFORD.Model;
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Services.Search;

public class PesquisaService
{
    private readonly HttpClient _httpClient;
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PesquisaService(HttpClient httpClient, FordDbContext context, IMapper mapper)
    {
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
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

        if (payload?.JobId == null)
            throw new ApplicationException("Python não retornou job_id.");

        return payload.JobId;
    }

    // -------------------------------------------------------------------------
    // GET /Pesquisa/jobs/{id}
    // Checa o status do job. Se done, processa e salva o carro no banco.
    // -------------------------------------------------------------------------
    public async Task<JobStatusDTO> ChecarJob(Guid jobId)
    {
        // 1. Busca o job no banco local
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return new JobStatusDTO { Status = "not_found" };

        // 2. Se ainda está rodando, só retorna o status
        if (job.Status is "pending" or "running")
            return new JobStatusDTO { Status = job.Status };

        // 3. Se deu erro, retorna o erro
        if (job.Status == "error")
            return new JobStatusDTO { Status = "error", Error = job.Error };

        // 4. Se done mas o carro já foi salvo antes, só retorna done
        if (job.Status == "done" && job.Result == null)
            return new JobStatusDTO { Status = "done" };

        // 5. Se done e ainda tem result pra processar, salva o carro agora
        if (job.Status == "done" && job.Result != null)
        {
            var readCarroDto = await ProcessarESalvarCarro(job);

            // Limpa o result do job após processar (já está no banco de carros)
            job.Result = null;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new JobStatusDTO { Status = "done", Carro = readCarroDto };
        }

        return new JobStatusDTO { Status = job.Status };
    }

    // -------------------------------------------------------------------------
    // Processa o JSON do Python e salva o carro — mesma lógica de antes
    // -------------------------------------------------------------------------
    private async Task<ReadCarroDTO> ProcessarESalvarCarro(Job job)
    {
        var createCarroDto = JsonSerializer.Deserialize<CreateCarroDTO>(job.Result!, _jsonOptions);
        if (createCarroDto == null)
            throw new ApplicationException("Resultado do job está vazio ou inválido.");

        var carroEntity = _mapper.Map<Carro>(createCarroDto);

        if (createCarroDto.ModosCarro != null && createCarroDto.ModosCarro.Any())
        {
            foreach (var modoNome in createCarroDto.ModosCarro)
            {
                var modoDb = await _context.Modos.FirstOrDefaultAsync(m => m.Tipo == modoNome);
                if (modoDb != null)
                    carroEntity.ModosCarro.Add(new CarroModo { Carro = carroEntity, Modo = modoDb });
            }
        }

        await SincronizarEInjetarIdsDeFontesAsync(carroEntity);

        await _context.Carros.AddAsync(carroEntity);
        await _context.SaveChangesAsync();

        var carroCompleto = await _context.Carros
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)
            .Include(c => c.ModosCarro).ThenInclude(cm => cm.Modo)
            .FirstOrDefaultAsync(c => c.Id == carroEntity.Id);

        if (carroCompleto == null)
            throw new KeyNotFoundException("Veículo processado mas não encontrado no banco.");

        var readCarroDto = _mapper.Map<ReadCarroDTO>(carroCompleto);
        await PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carroCompleto);

        return readCarroDto;
    }

    // -------------------------------------------------------------------------
    // Métodos auxiliares — sem alteração em relação ao original
    // -------------------------------------------------------------------------

    private async Task SincronizarEInjetarIdsDeFontesAsync(Carro carro)
    {
        var nomesFontes = new HashSet<string>();

        foreach (var spec in carro.Especificacoes)
        {
            nomesFontes.UnionWith(spec.Potencia.Fontes.Select(f => f.Fonte));
            fontesIdsColetor(spec.Torque?.Fontes, nomesFontes);
            fontesIdsColetor(spec.PotenciaRpm?.Fontes, nomesFontes);
            fontesIdsColetor(spec.TorqueRpm?.Fontes, nomesFontes);
            fontesIdsColetor(spec.Transmissao?.Fontes, nomesFontes);
            fontesIdsColetor(spec.Tracao?.Fontes, nomesFontes);
        }

        foreach (var cons in carro.Consumos)
        {
            fontesIdsColetor(cons.Cidade?.Fontes, nomesFontes);
            fontesIdsColetor(cons.Estrada?.Fontes, nomesFontes);
        }

        foreach (var dim in carro.Dimensoes)
        {
            fontesIdsColetor(dim.Comprimento?.Fontes, nomesFontes);
            fontesIdsColetor(dim.Largura?.Fontes, nomesFontes);
            fontesIdsColetor(dim.Altura?.Fontes, nomesFontes);
            fontesIdsColetor(dim.EntreEixos?.Fontes, nomesFontes);
        }

        foreach (var pneu in carro.Pneus)
        {
            fontesIdsColetor(pneu.Tipo?.Fontes, nomesFontes);
            fontesIdsColetor(pneu.Aro?.Fontes, nomesFontes);
            fontesIdsColetor(pneu.Largura?.Fontes, nomesFontes);
            fontesIdsColetor(pneu.Perfil?.Fontes, nomesFontes);
        }

        foreach (var extra in carro.Extras)
        {
            fontesIdsColetor(extra.CapacidadeTanque?.Fontes, nomesFontes);
            fontesIdsColetor(extra.TipoCombustivel?.Fontes, nomesFontes);
            fontesIdsColetor(extra.CapacidadeCarga?.Fontes, nomesFontes);
            fontesIdsColetor(extra.CapacidadeReboque?.Fontes, nomesFontes);
        }

        var listaNomesFiltrados = nomesFontes.Where(n => !string.IsNullOrEmpty(n)).ToList();
        if (!listaNomesFiltrados.Any()) return;

        var fontesNoBanco = await _context.Fontes
            .Where(f => listaNomesFiltrados.Contains(f.Nome))
            .ToDictionaryAsync(f => f.Nome, f => f.Id);

        var novasFontesUrls = listaNomesFiltrados.Where(nome => !fontesNoBanco.ContainsKey(nome)).ToList();
        if (novasFontesUrls.Any())
        {
            var novasFontesEntidades = novasFontesUrls.Select(url => new Fonte
            {
                Nome = url,
                Url = url,
                Confiabilidade = 0.5m,
                DataAdicao = DateTime.UtcNow
            }).ToList();

            await _context.Fontes.AddRangeAsync(novasFontesEntidades);
            await _context.SaveChangesAsync();

            foreach (var novaF in novasFontesEntidades)
                fontesNoBanco.Add(novaF.Nome, novaF.Id);
        }

        foreach (var spec in carro.Especificacoes)
        {
            VincularIdLocal(spec.Potencia.Fontes, fontesNoBanco);
            VincularIdLocal(spec.Torque.Fontes, fontesNoBanco);
            VincularIdLocal(spec.PotenciaRpm.Fontes, fontesNoBanco);
            VincularIdLocal(spec.TorqueRpm.Fontes, fontesNoBanco);
            VincularIdLocal(spec.Transmissao.Fontes, fontesNoBanco);
            VincularIdLocal(spec.Tracao.Fontes, fontesNoBanco);
        }

        foreach (var cons in carro.Consumos)
        {
            VincularIdLocal(cons.Cidade.Fontes, fontesNoBanco);
            VincularIdLocal(cons.Estrada.Fontes, fontesNoBanco);
        }

        foreach (var dim in carro.Dimensoes)
        {
            VincularIdLocal(dim.Comprimento.Fontes, fontesNoBanco);
            VincularIdLocal(dim.Largura.Fontes, fontesNoBanco);
            VincularIdLocal(dim.Altura.Fontes, fontesNoBanco);
            VincularIdLocal(dim.EntreEixos.Fontes, fontesNoBanco);
        }

        foreach (var pneu in carro.Pneus)
        {
            VincularIdLocal(pneu.Tipo.Fontes, fontesNoBanco);
            VincularIdLocal(pneu.Aro.Fontes, fontesNoBanco);
            VincularIdLocal(pneu.Largura.Fontes, fontesNoBanco);
            VincularIdLocal(pneu.Perfil.Fontes, fontesNoBanco);
        }

        foreach (var extra in carro.Extras)
        {
            VincularIdLocal(extra.CapacidadeTanque.Fontes, fontesNoBanco);
            VincularIdLocal(extra.TipoCombustivel.Fontes, fontesNoBanco);
            VincularIdLocal(extra.CapacidadeCarga.Fontes, fontesNoBanco);
            VincularIdLocal(extra.CapacidadeReboque.Fontes, fontesNoBanco);
        }
    }

    private string ExtrairDominioPrincipal(string textoFonte)
    {
        if (string.IsNullOrWhiteSpace(textoFonte)) return string.Empty;
        try
        {
            string urlParaLeitura = textoFonte.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? textoFonte
                : $"https://{textoFonte}";

            if (Uri.TryCreate(urlParaLeitura, UriKind.Absolute, out var uri))
                return uri.Host.Replace("www.", "").ToLowerInvariant();
        }
        catch { }

        return textoFonte.ToLowerInvariant();
    }

    private void fontesIdsColetor<T>(List<ItemFonteScraping<T>> fontes, HashSet<string> coletor)
    {
        if (fontes != null)
            coletor.UnionWith(fontes.Select(f => ExtrairDominioPrincipal(f.Fonte)));
    }

    private void VincularIdLocal<T>(List<ItemFonteScraping<T>> fontes, Dictionary<string, int> catalogo)
    {
        if (fontes == null) return;
        foreach (var f in fontes)
        {
            if (string.IsNullOrEmpty(f.Fonte)) continue;
            string dominioLimpo = ExtrairDominioPrincipal(f.Fonte);
            if (catalogo.TryGetValue(dominioLimpo, out int id))
            {
                f.FonteId = id;
                f.Fonte = dominioLimpo;
            }
        }
    }

    private async Task PreencherCatalogoDeFontesNoDtoAsync(ReadCarroDTO dto, Carro carro)
    {
        var fontesIds = new HashSet<int>();

        foreach (var spec in carro.Especificacoes)
        {
            fontesIds.UnionWith(spec.Potencia?.Fontes.Select(p => p.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(spec.Torque?.Fontes.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(spec.PotenciaRpm?.Fontes.Select(pr => pr.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(spec.TorqueRpm?.Fontes.Select(tr => tr.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(spec.Transmissao?.Fontes.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(spec.Tracao?.Fontes.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
        }

        foreach (var cons in carro.Consumos)
        {
            fontesIds.UnionWith(cons.Cidade?.Fontes.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(cons.Estrada?.Fontes.Select(e => e.FonteId) ?? Enumerable.Empty<int>());
        }

        foreach (var dim in carro.Dimensoes)
        {
            fontesIds.UnionWith(dim.Comprimento?.Fontes.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(dim.Largura?.Fontes.Select(l => l.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(dim.Altura?.Fontes.Select(a => a.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(dim.EntreEixos?.Fontes.Select(e => e.FonteId) ?? Enumerable.Empty<int>());
        }

        foreach (var pneu in carro.Pneus)
        {
            fontesIds.UnionWith(pneu.Tipo?.Fontes.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(pneu.Aro?.Fontes.Select(a => a.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(pneu.Largura?.Fontes.Select(l => l.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(pneu.Perfil?.Fontes.Select(p => p.FonteId) ?? Enumerable.Empty<int>());
        }

        foreach (var extra in carro.Extras)
        {
            fontesIds.UnionWith(extra.CapacidadeTanque?.Fontes.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(extra.TipoCombustivel?.Fontes.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(extra.CapacidadeCarga?.Fontes.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
            fontesIds.UnionWith(extra.CapacidadeReboque?.Fontes.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
        }

        if (fontesIds.Count > 0)
        {
            var listaFontesEntidade = await _context.Fontes
                .Where(f => fontesIds.Contains(f.Id) && !f.Excluido)
                .ToListAsync();

            dto.Fontes = _mapper.Map<List<ReadFonteDTO>>(listaFontesEntidade);
        }
    }
}

// DTOs internos para comunicação com o Python
internal record PythonJobResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("job_id")]
    public Guid JobId { get; init; }
}