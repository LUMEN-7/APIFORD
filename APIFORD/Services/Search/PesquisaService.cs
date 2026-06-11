// ============================================================================
// FILE: APIFORD/Services/Search/PesquisaService.cs
// ============================================================================
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
using APIFORD.Model.CarroClasses;
using APIFORD.Model.CarroClasses.Intermedians;

namespace APIFORD.Services.Search;

public class PesquisaService
{
    private readonly HttpClient _httpClient;
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public PesquisaService(HttpClient httpClient, FordDbContext context, IMapper mapper)
    {
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReadCarroDTO> Busca(BuscaDTO dto)
    {
        var url = "http://127.0.0.1:8000/specs";

        // 1. Consome a API em Python enviando o objeto de busca no corpo da requisição POST
        var opcoesSerializacao = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var response = await _httpClient.PostAsJsonAsync(url, dto);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Falha crítica na orquestração: O microserviço de busca em Python está indisponível.");
        }

        // 2. Deserializa o JSON estruturado diretamente para o DTO de criação do Carro
        var createCarroDto = await response.Content.ReadFromJsonAsync<CreateCarroDTO>(opcoesSerializacao);
        if (createCarroDto == null)
        {
            throw new ApplicationException("A resposta retornada pelo serviço de raspagem de dados está vazia.");
        }

        // 3. Converte o DTO recebido para a Entidade Carro do banco de dados
        var carroEntity = _mapper.Map<Carro>(createCarroDto);

        // 4. Resolve o mapeamento automático dos modos de condução (Tabela Intermediária Relacional)
        if (createCarroDto.ModosCarro != null && createCarroDto.ModosCarro.Any())
        {
            foreach (var modoNome in createCarroDto.ModosCarro)
            {
                var modoDb = await _context.Modos.FirstOrDefaultAsync(m => m.Tipo == modoNome);
                if (modoDb != null)
                {
                    carroEntity.ModosCarro.Add(new CarroModo { Carro = carroEntity, Modo = modoDb });
                }
            }
        }

        // 5. Varre o JSON aninhado recebido do Python, sincroniza as fontes textuais com a tb_fontes e injeta os IDs gerados
        await SincronizarEInjetarIdsDeFontesAsync(carroEntity);

        // 6. Persiste o grafo completo com as tabelas satélites, modos relacionais e colunas JSON no SQL Server
        await _context.Carros.AddAsync(carroEntity);
        await _context.SaveChangesAsync();

        // 7. Carrega o objeto atualizado do banco de dados incluindo os relacionamentos estruturados
        var carroCompleto = await _context.Carros
            .Include(c => c.Especificacoes)
            .Include(c => c.Consumos)
            .Include(c => c.Dimensoes)
            .Include(c => c.Pneus)
            .Include(c => c.Extras)
            .Include(c => c.ModosCarro).ThenInclude(cm => cm.Modo)
            .FirstOrDefaultAsync(c => c.Id == carroEntity.Id);

        if (carroCompleto == null)
        {
            throw new KeyNotFoundException("Erro de persistência: O veículo foi processado mas não pôde ser recuperado do banco de dados.");
        }

        // 8. Mapeia para o ReadCarroDTO padrão de saída
        var readCarroDto = _mapper.Map<ReadCarroDTO>(carroCompleto);

        // 9. Coleta os IDs de fontes gravados localmente para anexar o catálogo global de metadados das fontes no DTO
        await PreencherCatalogoDeFontesNoDtoAsync(readCarroDto, carroCompleto);

        return readCarroDto;
    }

    

    private async Task SincronizarEInjetarIdsDeFontesAsync(Carro carro)
    {
        var nomesFontes = new HashSet<string>();

        // Coleta todos os nomes de fontes de forma deduplicada de cada propriedade técnica
        foreach (var spec in carro.Especificacoes)
        {
            nomesFontes.UnionWith(spec.Potencia.HistoricoFontes.Select(f => f.NomeFontePython));
            fontesIdsColetor(spec.Torque.HistoricoFontes, nomesFontes);
            fontesIdsColetor(spec.PotenciaRpm.HistoricoFontes, nomesFontes);
            fontesIdsColetor(spec.TorqueRpm.HistoricoFontes, nomesFontes);
            fontesIdsColetor(spec.Transmissao.HistoricoFontes, nomesFontes);
            fontesIdsColetor(spec.Tracao.HistoricoFontes, nomesFontes);
        }

        foreach (var cons in carro.Consumos)
        {
            fontesIdsColetor(cons.Cidade.HistoricoFontes, nomesFontes);
            fontesIdsColetor(cons.Estrada.HistoricoFontes, nomesFontes);
        }

        foreach (var dim in carro.Dimensoes)
        {
            fontesIdsColetor(dim.Comprimento.HistoricoFontes, nomesFontes);
            fontesIdsColetor(dim.Largura.HistoricoFontes, nomesFontes);
            fontesIdsColetor(dim.Altura.HistoricoFontes, nomesFontes);
            fontesIdsColetor(dim.EntreEixos.HistoricoFontes, nomesFontes);
        }

        foreach (var pneu in carro.Pneus)
        {
            fontesIdsColetor(pneu.Tipo.HistoricoFontes, nomesFontes);
            fontesIdsColetor(pneu.Aro.HistoricoFontes, nomesFontes);
            fontesIdsColetor(pneu.Largura.HistoricoFontes, nomesFontes);
            fontesIdsColetor(pneu.Perfil.HistoricoFontes, nomesFontes);
        }

        foreach (var extra in carro.Extras)
        {
            fontesIdsColetor(extra.CapacidadeTanque.HistoricoFontes, nomesFontes);
            fontesIdsColetor(extra.TipoCombustivel.HistoricoFontes, nomesFontes);
            fontesIdsColetor(extra.CapacidadeCarga.HistoricoFontes, nomesFontes);
            fontesIdsColetor(extra.CapacidadeReboque.HistoricoFontes, nomesFontes);
        }

        var listaNomesFiltrados = nomesFontes.Where(n => !string.IsNullOrEmpty(n)).ToList();
        if (!listaNomesFiltrados.Any()) return;

        // Verifica quais fontes textuais já existem cadastradas no banco de dados
        var fontesNoBanco = await _context.Fontes
            .Where(f => listaNomesFiltrados.Contains(f.Nome))
            .ToDictionaryAsync(f => f.Nome, f => f.Id);

        // Se houverem fontes inéditas vindas do robô Python, adiciona-as no catálogo global relacional
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
            {
                fontesNoBanco.Add(novaF.Nome, novaF.Id);
            }
        }

        // Realiza o vínculo forçado injetando os IDs corretos de banco nas estruturas JSON locais de histórico
        foreach (var spec in carro.Especificacoes)
        {
            VincularIdLocal(spec.Potencia.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(spec.Torque.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(spec.PotenciaRpm.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(spec.TorqueRpm.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(spec.Transmissao.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(spec.Tracao.HistoricoFontes, fontesNoBanco);
        }

        foreach (var cons in carro.Consumos)
        {
            VincularIdLocal(cons.Cidade.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(cons.Estrada.HistoricoFontes, fontesNoBanco);
        }

        foreach (var dim in carro.Dimensoes)
        {
            VincularIdLocal(dim.Comprimento.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(dim.Largura.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(dim.Altura.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(dim.EntreEixos.HistoricoFontes, fontesNoBanco);
        }

        foreach (var pneu in carro.Pneus)
        {
            VincularIdLocal(pneu.Tipo.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(pneu.Aro.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(pneu.Largura.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(pneu.Perfil.HistoricoFontes, fontesNoBanco);
        }

        foreach (var extra in carro.Extras)
        {
            VincularIdLocal(extra.CapacidadeTanque.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(extra.TipoCombustivel.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(extra.CapacidadeCarga.HistoricoFontes, fontesNoBanco);
            VincularIdLocal(extra.CapacidadeReboque.HistoricoFontes, fontesNoBanco);
        }
    }

    private string ExtrairDominioPrincipal(string textoFonte)
    {
        if (string.IsNullOrWhiteSpace(textoFonte)) return string.Empty;

        try
        {
            // Se a string vier sem "http", o C# não consegue recortar. Adicionamos um falso só para a leitura.
            string urlParaLeitura = textoFonte.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? textoFonte
                : $"https://{textoFonte}";

            if (Uri.TryCreate(urlParaLeitura, UriKind.Absolute, out var uri))
            {
                // O ".Host" pega apenas a raiz do site. O Replace limpa o "www." caso exista.
                return uri.Host.Replace("www.", "").ToLowerInvariant();
            }
        }
        catch
        {
            // Se der algum erro muito bizarro na string, retorna ela mesma por segurança
        }

        return textoFonte.ToLowerInvariant();
    }

    private void fontesIdsColetor<T>(List<ItemFonteScraping<T>> fontes, HashSet<string> coletor)
    {
        if (fontes != null)
        {
            coletor.UnionWith(fontes.Select(f => ExtrairDominioPrincipal(f.NomeFontePython)));
        }
    }

    private void VincularIdLocal<T>(List<ItemFonteScraping<T>> fontes, Dictionary<string, int> catalogo)
    {
        if (fontes == null) return;
        foreach (var f in fontes)
        {
            if (string.IsNullOrEmpty(f.NomeFontePython)) continue;

            // 🛡️ APLICA A BLINDAGEM AQUI TAMBÉM:
            string dominioLimpo = ExtrairDominioPrincipal(f.NomeFontePython);

            if (catalogo.TryGetValue(dominioLimpo, out int id))
            {
                f.FonteId = id;
                // Opcional: Você pode substituir o nome sujo pelo nome limpo direto no objeto para manter o banco padronizado
                f.NomeFontePython = dominioLimpo;
            }
        }
    }

    private async Task PreencherCatalogoDeFontesNoDtoAsync(ReadCarroDTO dto, Carro carro)
    {
        var fontesIds = new HashSet<int>();

        foreach (var spec in carro.Especificacoes)
        {
            fontesIds.UnionWith(spec.Potencia.HistoricoFontes.Select(p => p.FonteId));
            fontesIds.UnionWith(spec.Torque.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(spec.PotenciaRpm.HistoricoFontes.Select(pr => pr.FonteId));
            fontesIds.UnionWith(spec.TorqueRpm.HistoricoFontes.Select(tr => tr.FonteId));
            fontesIds.UnionWith(spec.Transmissao.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(spec.Tracao.HistoricoFontes.Select(t => t.FonteId));
        }

        foreach (var cons in carro.Consumos)
        {
            fontesIds.UnionWith(cons.Cidade.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(cons.Estrada.HistoricoFontes.Select(e => e.FonteId));
        }

        foreach (var dim in carro.Dimensoes)
        {
            fontesIds.UnionWith(dim.Comprimento.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(dim.Largura.HistoricoFontes.Select(l => l.FonteId));
            fontesIds.UnionWith(dim.Altura.HistoricoFontes.Select(a => a.FonteId));
            fontesIds.UnionWith(dim.EntreEixos.HistoricoFontes.Select(e => e.FonteId));
        }

        foreach (var pneu in carro.Pneus)
        {
            fontesIds.UnionWith(pneu.Tipo.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(pneu.Aro.HistoricoFontes.Select(a => a.FonteId));
            fontesIds.UnionWith(pneu.Largura.HistoricoFontes.Select(l => l.FonteId));
            fontesIds.UnionWith(pneu.Perfil.HistoricoFontes.Select(p => p.FonteId));
        }

        foreach (var extra in carro.Extras)
        {
            fontesIds.UnionWith(extra.CapacidadeTanque.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(extra.TipoCombustivel.HistoricoFontes.Select(t => t.FonteId));
            fontesIds.UnionWith(extra.CapacidadeCarga.HistoricoFontes.Select(c => c.FonteId));
            fontesIds.UnionWith(extra.CapacidadeReboque.HistoricoFontes.Select(c => c.FonteId));
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