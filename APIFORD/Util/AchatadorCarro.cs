using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.Export;

namespace APIFORD.Util;

public static class AchatadorCarro
{
    public static ResultadoAchatamento Achatar(int linhagemId, object objeto, Dictionary<string, string> escolhas, string prefixo = "")
    {
        var resultado = new ResultadoAchatamento();
        AchatarRecursivo(linhagemId, objeto, escolhas, prefixo, resultado);
        return resultado;
    }

    private static string NomeColuna(string campoCompleto) =>
    campoCompleto.Contains('.') ? campoCompleto[(campoCompleto.LastIndexOf('.') + 1)..] : campoCompleto;

    private static void AchatarRecursivo(int linhagemId, object objeto, Dictionary<string, string> escolhas, string prefixo, ResultadoAchatamento resultado)
    {
        if (objeto == null) return;

        foreach (var prop in objeto.GetType().GetProperties())
        {
            var valor = prop.GetValue(objeto);
            if (valor == null) continue;

            bool isEnvelope = prop.PropertyType.IsGenericType &&
                               prop.PropertyType.GetGenericTypeDefinition() == typeof(PropriedadeScrapingDTO<>);
            string campoCompleto = $"{prefixo}{prop.Name}";

            if (isEnvelope)
            {
                ProcessarEnvelope(linhagemId, campoCompleto, valor, escolhas, resultado);
            }
            else if (valor is System.Collections.IEnumerable enumeravel && valor is not string)
            {
                foreach (var item in enumeravel)
                    AchatarRecursivo(linhagemId, item, escolhas, $"{campoCompleto}.", resultado);
            }
            else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string) &&
                      prop.PropertyType != typeof(DateTime) && !prop.PropertyType.IsEnum)
            {
                AchatarRecursivo(linhagemId, valor, escolhas, $"{campoCompleto}.", resultado);
            }
            else
            {
                resultado.Valores[NomeColuna(campoCompleto)] = valor.ToString() ?? "";
            }
        }
    }

    private static void ProcessarEnvelope(int linhagemId, string campoCompleto, dynamic envelope, Dictionary<string, string> escolhas, ResultadoAchatamento resultado)
    {
        if (envelope.Fontes == null || envelope.Fontes.Count == 0)
        {
            resultado.Valores[campoCompleto] = "";
            return;
        }

        var fontes = ((IEnumerable<dynamic>)envelope.Fontes).ToList();
        bool houveConflito = fontes.Select(f => f.Valor?.ToString()).Distinct().Count() > 1;

        escolhas.TryGetValue(campoCompleto, out var fonteEscolhidaManual);
        dynamic vencedora = fonteEscolhidaManual != null
            ? fontes.FirstOrDefault(f => f.Fonte == fonteEscolhidaManual) ?? fontes.OrderByDescending(f => (decimal)f.Confianca).First()
            : fontes.OrderByDescending(f => (decimal)f.Confianca).First();

        resultado.Valores[NomeColuna(campoCompleto)] = vencedora.Valor?.ToString() ?? "";


        if (houveConflito)
        {
            resultado.Origens[campoCompleto] = fonteEscolhidaManual != null
                ? $"Escolha manual: {vencedora.Fonte}"
                : $"Automático (maior confiança): {vencedora.Fonte}";
        }

        foreach (var fonte in fontes)
        {
            resultado.FontesDetalhadas.Add(new FonteDetalhadaResponseDTO
            {
                LinhagemId = linhagemId,
                Campo = campoCompleto,
                Fonte = fonte.Fonte,
                Valor = fonte.Valor?.ToString() ?? "",
                Confianca = fonte.Confianca,
                DataColeta = fonte.DataColeta,
                Selecionada = fonte.Fonte == vencedora.Fonte
            });
        }
    }
}