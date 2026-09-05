using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Data.DTOS.Export;
using APIFORD.Util;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace APIFORD.Services.Export;

public class ExportadorCsvService : IExportadorFormatoService
{
    public string Formato => "csv";
    public string ExtensaoArquivo => "zip";
    public string ContentType => "application/zip";

    public byte[] Exportar(List<ReadCarroDTO> carrosOriginais, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes)
    {
        var separador = opcoes.SeparadorCsv;
        using var memStream = new MemoryStream();
        using (var zip = new ZipArchive(memStream, ZipArchiveMode.Create, true))
        {
            var entradaCarros = zip.CreateEntry("carros.csv");
            using (var writer = new StreamWriter(entradaCarros.Open(), new UTF8Encoding(true))) // BOM ajuda o Excel a reconhecer acentuação
                writer.Write(MontarCsvCarros(carrosAchatados, separador));

            var entradaFontes = zip.CreateEntry("fontes.csv");
            using (var writer = new StreamWriter(entradaFontes.Open(), new UTF8Encoding(true)))
                writer.Write(MontarCsvFontes(carrosAchatados.SelectMany(c => c.FontesDetalhadas).ToList(), separador));
        }
        return memStream.ToArray();
    }

    private string MontarCsvCarros(List<ResultadoAchatamento> carros, string separador)
    {
        var colunasValores = carros.SelectMany(c => c.Valores.Keys).Distinct().ToList();
        var colunasOrigem = carros.SelectMany(c => c.Origens.Keys).Distinct().ToList();

        var sb = new StringBuilder();
        var cabecalho = colunasValores.Select(c => Escapar(c, separador))
            .Concat(colunasOrigem.Select(c => Escapar($"{c} (Origem)", separador)));
        sb.AppendLine(string.Join(separador, cabecalho));

        foreach (var carro in carros)
        {
            var linha = colunasValores.Select(c => Escapar(carro.Valores.GetValueOrDefault(c, ""), separador))
                .Concat(colunasOrigem.Select(c => Escapar(carro.Origens.GetValueOrDefault(c, ""), separador)));
            sb.AppendLine(string.Join(separador, linha));
        }
        return sb.ToString();
    }

    private string MontarCsvFontes(List<FonteDetalhadaDTO> fontes, string separador)
    {
        var culturaDecimal = separador == ";" ? new CultureInfo("pt-BR") : CultureInfo.InvariantCulture;

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(separador, new[] { "LinhagemId", "Campo", "Fonte", "Valor", "Confianca", "DataColeta", "Selecionada" }));
        foreach (var f in fontes)
        {
            var linha = new[]
            {
            f.LinhagemId.ToString(),
            Escapar(f.Campo, separador),
            Escapar(f.Fonte, separador),
            Escapar(f.Valor, separador),
            f.Confianca.ToString(culturaDecimal),
            f.DataColeta.ToString("O"),
            f.Selecionada.ToString()
        };
            sb.AppendLine(string.Join(separador, linha));
        }
        return sb.ToString();
    }

    private string Escapar(string valor, string separador) =>
        valor.Contains(separador) || valor.Contains('"') || valor.Contains('\n')
            ? $"\"{valor.Replace("\"", "\"\"")}\""
            : valor;
}
