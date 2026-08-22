using APIFORD.Data;
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Model.CarroClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace APIFORD.Services;

public class HelperService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;

    public HelperService(FordDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private (string Nome, string UrlBase) ExtrairInfoFonte(string textoFonte)
    {
        if (string.IsNullOrWhiteSpace(textoFonte)) return ("Desconhecido", "desconhecido");
        try
        {
            string urlParaLeitura = textoFonte.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? textoFonte
                : $"https://{textoFonte}";

            if (Uri.TryCreate(urlParaLeitura, UriKind.Absolute, out var uri))
            {
                string host = uri.Host.Replace("www.", "").ToLowerInvariant();
                string nome = host.Split('.')[0];
                nome = char.ToUpper(nome[0]) + nome.Substring(1);

                return (nome, host);
            }
        }
        catch { }

        var textoLimpo = textoFonte.ToLowerInvariant();
        var partes = textoLimpo.Split('.');
        var nomeFallback = partes[0].Length > 0 ? char.ToUpper(partes[0][0]) + partes[0].Substring(1) : "Fonte";
        return (nomeFallback, textoLimpo);
    }

    private void ColetarFontesComConfianca<T>(List<ItemFonteScraping<T>>? fontes, Dictionary<string, (string Nome, List<double> Confiancas)> agregador)
    {
        if (fontes == null) return;
        foreach (var f in fontes)
        {
            if (string.IsNullOrWhiteSpace(f.Fonte)) continue;

            var (nome, urlBase) = ExtrairInfoFonte(f.Fonte);

            if (!agregador.ContainsKey(urlBase))
                agregador[urlBase] = (nome, new List<double>());

            agregador[urlBase].Confiancas.Add(f.Confianca);
        }
    }

    private void VincularIdLocal<T>(List<ItemFonteScraping<T>>? fontes, Dictionary<string, int> catalogo)
    {
        if (fontes == null) return;
        foreach (var f in fontes)
        {
            if (string.IsNullOrWhiteSpace(f.Fonte)) continue;
            var (_, urlBase) = ExtrairInfoFonte(f.Fonte);

            if (catalogo.TryGetValue(urlBase, out int id))
            {
                f.FonteId = id;
                f.Fonte = urlBase;
            }
        }
    }

    public async Task SincronizarEInjetarIdsDeFontesAsync(Carro carro)
    {
        // Dicionário agregador: Chave = urlBase | Valor = (Nome formatado, Lista de confianças)
        var agregadorFontes = new Dictionary<string, (string Nome, List<double> Confiancas)>();
        
        if (carro.Categoria != null)
            ColetarFontesComConfianca(carro.Categoria.Fontes, agregadorFontes);

        if (carro.Modos != null)
            ColetarFontesComConfianca(carro.Modos.Fontes, agregadorFontes);

        if (carro.Especificacoes != null)
        {
            foreach (var spec in carro.Especificacoes)
            {
                ColetarFontesComConfianca(spec.Potencia?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(spec.Torque?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(spec.PotenciaRpm?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(spec.TorqueRpm?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(spec.Transmissao?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(spec.Tracao?.Fontes, agregadorFontes);
            }
        }

        if (carro.Consumos != null)
        {
            foreach (var cons in carro.Consumos)
            {
                ColetarFontesComConfianca(cons.Cidade?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(cons.Estrada?.Fontes, agregadorFontes);
            }
        }

        if (carro.Dimensoes != null)
        {
            foreach (var dim in carro.Dimensoes)
            {
                ColetarFontesComConfianca(dim.Comprimento?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(dim.Largura?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(dim.Altura?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(dim.EntreEixos?.Fontes, agregadorFontes);
            }
        }

        if (carro.Pneus != null)
        {
            foreach (var pneu in carro.Pneus)
            {
                ColetarFontesComConfianca(pneu.Tipo?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(pneu.Aro?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(pneu.Largura?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(pneu.Perfil?.Fontes, agregadorFontes);
            }
        }

        if (carro.Extras != null)
        {
            foreach (var extra in carro.Extras)
            {
                ColetarFontesComConfianca(extra.CapacidadeTanque?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(extra.TipoCombustivel?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(extra.CapacidadeCarga?.Fontes, agregadorFontes);
                ColetarFontesComConfianca(extra.CapacidadeReboque?.Fontes, agregadorFontes);
            }
        }

        if (!agregadorFontes.Any()) return;

        var urlsBase = agregadorFontes.Keys.ToList();
        var fontesNoBanco = await _context.Fontes
            .Where(f => urlsBase.Contains(f.Url))
            .ToDictionaryAsync(f => f.Url, f => f.Id);

        var novasFontesEntidades = new List<Fonte>();

        foreach (var entry in agregadorFontes)
        {
            var urlBase = entry.Key;
            if (!fontesNoBanco.ContainsKey(urlBase))
            {
                var nome = entry.Value.Nome;
                var confiancas = entry.Value.Confiancas;
                decimal mediaCalculada = confiancas.Any() ? (decimal)confiancas.Average() : 0.5m;

                novasFontesEntidades.Add(new Fonte
                {
                    Nome = nome,                     // "Icarros"
                    Url = urlBase,                   // "icarros.com.br"
                    Confiabilidade = mediaCalculada, // Média real calculada
                    DataAdicao = DateTime.UtcNow
                });
            }
        }

        if (novasFontesEntidades.Any())
        {
            await _context.Fontes.AddRangeAsync(novasFontesEntidades);
            await _context.SaveChangesAsync();

            foreach (var novaF in novasFontesEntidades)
                fontesNoBanco[novaF.Url] = novaF.Id;
        }
        if (carro.Categoria != null)
            VincularIdLocal(carro.Categoria.Fontes, fontesNoBanco);

        if (carro.Modos != null)
            VincularIdLocal(carro.Modos.Fontes, fontesNoBanco);

        if (carro.Especificacoes != null)
        {
            foreach (var spec in carro.Especificacoes)
            {
                VincularIdLocal(spec.Potencia?.Fontes, fontesNoBanco);
                VincularIdLocal(spec.Torque?.Fontes, fontesNoBanco);
                VincularIdLocal(spec.PotenciaRpm?.Fontes, fontesNoBanco);
                VincularIdLocal(spec.TorqueRpm?.Fontes, fontesNoBanco);
                VincularIdLocal(spec.Transmissao?.Fontes, fontesNoBanco);
                VincularIdLocal(spec.Tracao?.Fontes, fontesNoBanco);
            }
        }

        if (carro.Consumos != null)
        {
            foreach (var cons in carro.Consumos)
            {
                VincularIdLocal(cons.Cidade?.Fontes, fontesNoBanco);
                VincularIdLocal(cons.Estrada?.Fontes, fontesNoBanco);
            }
        }

        if (carro.Dimensoes != null)
        {
            foreach (var dim in carro.Dimensoes)
            {
                VincularIdLocal(dim.Comprimento?.Fontes, fontesNoBanco);
                VincularIdLocal(dim.Largura?.Fontes, fontesNoBanco);
                VincularIdLocal(dim.Altura?.Fontes, fontesNoBanco);
                VincularIdLocal(dim.EntreEixos?.Fontes, fontesNoBanco);
            }
        }

        if (carro.Pneus != null)
        {
            foreach (var pneu in carro.Pneus)
            {
                VincularIdLocal(pneu.Tipo?.Fontes, fontesNoBanco);
                VincularIdLocal(pneu.Aro?.Fontes, fontesNoBanco);
                VincularIdLocal(pneu.Largura?.Fontes, fontesNoBanco);
                VincularIdLocal(pneu.Perfil?.Fontes, fontesNoBanco);
            }
        }

        if (carro.Extras != null)
        {
            foreach (var extra in carro.Extras)
            {
                VincularIdLocal(extra.CapacidadeTanque?.Fontes, fontesNoBanco);
                VincularIdLocal(extra.TipoCombustivel?.Fontes, fontesNoBanco);
                VincularIdLocal(extra.CapacidadeCarga?.Fontes, fontesNoBanco);
                VincularIdLocal(extra.CapacidadeReboque?.Fontes, fontesNoBanco);
            }
        }
    }

    public async Task PreencherCatalogoDeFontesNoDtoAsync(List<ReadCarroDTO> dtos, List<Carro> carros)
    {
        var todosFontesIds = new HashSet<int>();

        // 1. Usamos o motor privado para coletar todos os IDs da lista sem repetir código
        foreach (var carro in carros)
        {
            todosFontesIds.UnionWith(ExtrairIdsDeFontesDeUmCarro(carro));
        }

        if (todosFontesIds.Count == 0) return;

        // 2. Busca no banco de dados
        var fontesNoBanco = await _context.Fontes
            .Where(f => todosFontesIds.Contains(f.Id) && !f.Excluido)
            .ToListAsync();

        var fontesDtoGlobais = _mapper.Map<List<ReadFonteDTO>>(fontesNoBanco);

        // 3. Distribui usando novamente o motor privado para saber quem é dono do quê
        for (int i = 0; i < dtos.Count; i++)
        {
            var idsDesteCarro = ExtrairIdsDeFontesDeUmCarro(carros[i]);
            dtos[i].Fontes = fontesDtoGlobais.Where(f => idsDesteCarro.Contains(f.Id)).ToList();
        }
    }

    public async Task PreencherCatalogoDeFontesNoDtoAsync(ReadCarroDTO dto, Carro carro)
    {
        // Envelopamos o item único em uma lista e mandamos para a função principal. 
        // O C# não vai ao banco duas vezes e você reaproveita TUDO!
        await PreencherCatalogoDeFontesNoDtoAsync(new List<ReadCarroDTO> { dto }, new List<Carro> { carro });
    }

    private HashSet<int> ExtrairIdsDeFontesDeUmCarro(Carro carro)
    {
        var fontesIds = new HashSet<int>();

        if (carro.Categoria?.Fontes != null)
            fontesIds.UnionWith(carro.Categoria.Fontes.Select(f => f.FonteId));

        if (carro.Modos?.Fontes != null)
            fontesIds.UnionWith(carro.Modos.Fontes.Select(f => f.FonteId));

        if (carro.Especificacoes != null)
        {
            foreach (var spec in carro.Especificacoes)
            {
                fontesIds.UnionWith(spec.Potencia?.Fontes?.Select(p => p.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(spec.Torque?.Fontes?.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(spec.PotenciaRpm?.Fontes?.Select(pr => pr.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(spec.TorqueRpm?.Fontes?.Select(tr => tr.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(spec.Transmissao?.Fontes?.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(spec.Tracao?.Fontes?.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
            }
        }

        if (carro.Consumos != null)
        {
            foreach (var cons in carro.Consumos)
            {
                fontesIds.UnionWith(cons.Cidade?.Fontes?.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(cons.Estrada?.Fontes?.Select(e => e.FonteId) ?? Enumerable.Empty<int>());
            }
        }

        if (carro.Dimensoes != null)
        {
            foreach (var dim in carro.Dimensoes)
            {
                fontesIds.UnionWith(dim.Comprimento?.Fontes?.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(dim.Largura?.Fontes?.Select(l => l.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(dim.Altura?.Fontes?.Select(a => a.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(dim.EntreEixos?.Fontes?.Select(e => e.FonteId) ?? Enumerable.Empty<int>());
            }
        }

        if (carro.Pneus != null)
        {
            foreach (var pneu in carro.Pneus)
            {
                fontesIds.UnionWith(pneu.Tipo?.Fontes?.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(pneu.Aro?.Fontes?.Select(a => a.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(pneu.Largura?.Fontes?.Select(l => l.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(pneu.Perfil?.Fontes?.Select(p => p.FonteId) ?? Enumerable.Empty<int>());
            }
        }

        if (carro.Extras != null)
        {
            foreach (var extra in carro.Extras)
            {
                fontesIds.UnionWith(extra.CapacidadeTanque?.Fontes?.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(extra.TipoCombustivel?.Fontes?.Select(t => t.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(extra.CapacidadeCarga?.Fontes?.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
                fontesIds.UnionWith(extra.CapacidadeReboque?.Fontes?.Select(c => c.FonteId) ?? Enumerable.Empty<int>());
            }
        }

        return fontesIds;
    }
}
