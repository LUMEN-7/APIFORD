using APIFORD.Data;
using APIFORD.Model.CarroClasses;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace APIFORD.Services.CarServices;

public class ImportacaoCarroService
{
    private readonly FordDbContext _context;

    private static readonly Dictionary<string, Func<Carro, object>> ColecoesAninhadas = new()
    {
        ["Especificacao"] = c => c.Especificacoes,
        ["Consumo"] = c => c.Consumos,
        ["Dimensao"] = c => c.Dimensoes,
        ["Pneu"] = c => c.Pneus,
        ["Extra"] = c => c.Extras,
    };

    private static bool EhTipoInteiro(Type t) =>
    t == typeof(int) || t == typeof(long) || t == typeof(short) ||
    t == typeof(int?) || t == typeof(long?) || t == typeof(short?);

    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        // Consumo
        ["consumo_cidade_km_l"] = "Cidade",
        ["consumo_estrada_km_l"] = "Estrada",

        // Dimensao
        ["altura_mm"] = "Altura",
        ["largura_mm"] = "Largura",
        ["comprimento_mm"] = "Comprimento",
        ["entre_eixos_mm"] = "EntreEixos",

        // Especificacao
        ["potencia_cv"] = "Potencia",
        ["torque_kgfm"] = "Torque",

        // Extra
        ["capacidade_tanque_l"] = "CapacidadeTanque",
        ["capacidade_reboque_kg"] = "CapacidadeReboque",
        ["capacidade_carga_kg"] = "CapacidadeCarga",

        // Pneu
        ["pneu_tipo"] = "Tipo",
        ["pneu_aro"] = "Aro",
        ["pneu_largura_mm"] = "Largura",
        ["pneu_perfil_pct"] = "Perfil",
    };

    private static readonly Dictionary<string, (string Colecao, string Propriedade)> AliasesComColecao = new(StringComparer.OrdinalIgnoreCase)
    {
        ["consumo_cidade_km_l"] = ("Consumo", "Cidade"),
        ["consumo_estrada_km_l"] = ("Consumo", "Estrada"),
        ["altura_mm"] = ("Dimensao", "Altura"),
        ["largura_mm"] = ("Dimensao", "Largura"),
        ["comprimento_mm"] = ("Dimensao", "Comprimento"),
        ["entre_eixos_mm"] = ("Dimensao", "EntreEixos"),
        ["potencia_cv"] = ("Especificacao", "Potencia"),
        ["torque_kgfm"] = ("Especificacao", "Torque"),
        ["capacidade_tanque_l"] = ("Extra", "CapacidadeTanque"),
        ["capacidade_reboque_kg"] = ("Extra", "CapacidadeReboque"),
        ["capacidade_carga_kg"] = ("Extra", "CapacidadeCarga"),
        ["pneu_tipo"] = ("Pneu", "Tipo"),
        ["pneu_aro"] = ("Pneu", "Aro"),
        ["pneu_largura_mm"] = ("Pneu", "Largura"), // <- agora sem ambiguidade
        ["pneu_perfil_pct"] = ("Pneu", "Perfil"),
    };

    public ImportacaoCarroService(FordDbContext context)
    {
        _context = context;
    }

    private static string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

        var semAcento = new string(texto
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        return Regex.Replace(semAcento, "[^a-zA-Z0-9]", "").ToLowerInvariant();
    }

    public List<string> AplicarAlteracoes(Carro carro, Dictionary<string, object> alteracoes, string fonte, int fonteId)
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

    public async Task<int> ResolverLinhagemIdAsync(string marca, string modelo, int ano)
    {
        var existente = await _context.Carros
            .Where(c => c.Marca.Trim().ToLower() == marca.Trim().ToLower()
                     && c.Modelo.Trim().ToLower() == modelo.Trim().ToLower()
                     && c.Ano == ano)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        return existente?.LinhagemId ?? 0;
    }

    private bool AplicarValorNaPropriedade(object alvo, PropertyInfo property, object valorBruto, string fonte, int fonteId)
    {
        bool isEnvelope = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PropriedadeScraping<>);
        var targetType = isEnvelope ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;

        object valorLimpo;
        try
        {
            if (valorBruto is Newtonsoft.Json.Linq.JToken jToken) valorLimpo = jToken.ToObject(targetType);
            else if (valorBruto is System.Text.Json.JsonElement jsonElement) valorLimpo = jsonElement.Deserialize(targetType);
            else if (valorBruto is string textoValor && EhTipoInteiro(targetType))
            {
                // CSV/planilha costuma exportar inteiro como "17.0" — Convert.ChangeType direto rejeita o ponto decimal
                var comoDouble = double.Parse(textoValor, CultureInfo.InvariantCulture);
                valorLimpo = Convert.ChangeType(Math.Round(comoDouble), targetType, CultureInfo.InvariantCulture);
            }
            else valorLimpo = Convert.ChangeType(valorBruto, targetType, CultureInfo.InvariantCulture);

            // Garante que qualquer DateTime vindo de fora sempre chegue como UTC pro Postgres
            if (valorLimpo is DateTime dt && dt.Kind != DateTimeKind.Utc)
            {
                valorLimpo = dt.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                    : dt.ToUniversalTime();
            }
        }
        catch { return false; }

        if (isEnvelope)
        {
            var tipoInterno = property.PropertyType.GetGenericArguments()[0];
            object envelope = property.GetValue(alvo) ?? Activator.CreateInstance(property.PropertyType)!;

            var novaFonteType = typeof(ItemFonteScraping<>).MakeGenericType(tipoInterno);
            object novaFonte = Activator.CreateInstance(novaFonteType)!;

            novaFonteType.GetProperty("Valor")!.SetValue(novaFonte, valorLimpo);

            var confiancaProperty = novaFonteType.GetProperty("Confianca")!;
            var confiancaValue = Convert.ChangeType(1.0m, confiancaProperty.PropertyType);
            confiancaProperty.SetValue(novaFonte, confiancaValue);

            novaFonteType.GetProperty("Fonte")!.SetValue(novaFonte, fonte);
            novaFonteType.GetProperty("FonteId")!.SetValue(novaFonte, fonteId);

            var envelopeType = property.PropertyType;
            var fontesProp = envelopeType.GetProperty("Fontes")!;
            var listaFontes = fontesProp.GetValue(envelope);

            if (listaFontes == null)
            {
                listaFontes = Activator.CreateInstance(typeof(List<>).MakeGenericType(novaFonteType))!;
                fontesProp.SetValue(envelope, listaFontes);
            }

            ((System.Collections.IList)listaFontes).Add(novaFonte);
            envelopeType.GetProperty("Conflito")!.SetValue(envelope, false);
            envelopeType.GetProperty("Confianca")!.SetValue(envelope, confiancaValue);

            property.SetValue(alvo, envelope);
        }
        else
        {
            property.SetValue(alvo, valorLimpo);
        }
        return true;
    }

    private (object alvo, PropertyInfo? property) LocalizarPropriedade(Carro carro, string nomePropriedade)
    {
        // 0) Alias explícito — resolve nomes com prefixo/sufixo (unidade, "consumo_", "pneu_", etc.)
        //    e desambigua colisões como "Largura" existir tanto em Dimensao quanto em Pneu.
        if (AliasesComColecao.TryGetValue(nomePropriedade, out var alias))
        {
            var tipoItemAlias = alias.Colecao switch
            {
                "Especificacao" => typeof(Especificacao),
                "Consumo" => typeof(Consumo),
                "Dimensao" => typeof(Dimensao),
                "Pneu" => typeof(Pneu),
                "Extra" => typeof(Extra),
                _ => null
            };

            if (tipoItemAlias != null)
            {
                var propAlias = tipoItemAlias.GetProperty(alias.Propriedade, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propAlias != null)
                {
                    dynamic colecaoAlias = ColecoesAninhadas[alias.Colecao](carro);
                    if (colecaoAlias.Count == 0)
                    {
                        dynamic novoItemAlias = Activator.CreateInstance(tipoItemAlias)!;
                        colecaoAlias.Add(novoItemAlias);
                    }
                    return (colecaoAlias[0], propAlias);
                }
            }
        }

        // 1) tenta direto em Carro, com normalização (ignora acentos/pontuação/underscore)
        var nomeBuscado = Normalizar(nomePropriedade);

        var direta = typeof(Carro).GetProperties()
            .FirstOrDefault(p => Normalizar(p.Name) == nomeBuscado);
        if (direta != null) return (carro, direta);

        // 2) procura nas 5 coleções aninhadas, também normalizado
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

            var prop = tipoItem?.GetProperties()
                .FirstOrDefault(p => Normalizar(p.Name) == nomeBuscado);
            if (prop == null) continue;

            dynamic colecao = getColecao(carro);
            if (colecao.Count == 0)
            {
                dynamic novoItem = Activator.CreateInstance(tipoItem)!;
                colecao.Add(novoItem);
            }
            return (colecao[0], prop);
        }

        return (carro, null);
    }

}
