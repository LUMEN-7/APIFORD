using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Util;
using ClosedXML.Excel;

namespace APIFORD.Services.Export;

public class ExportadorXlsxService : IExportadorFormatoService // precisa do pacote NuGet ClosedXML
{
    public string Formato => "xlsx";
    public string ExtensaoArquivo => "xlsx";
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public byte[] Exportar(List<ReadCarroDTO> carrosOriginais, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes)
    {
        var linhas = carrosAchatados.Select(c => c.Valores).ToList();
        var colunas = linhas.SelectMany(l => l.Keys).Distinct().ToList();

        using var workbook = new XLWorkbook();

        var planilhaCarros = workbook.Worksheets.Add("Carros");
        for (int i = 0; i < colunas.Count; i++) planilhaCarros.Cell(1, i + 1).Value = colunas[i];
        planilhaCarros.Row(1).Style.Font.Bold = true;
        for (int l = 0; l < linhas.Count; l++)
            for (int c = 0; c < colunas.Count; c++)
                planilhaCarros.Cell(l + 2, c + 1).Value = linhas[l].GetValueOrDefault(colunas[c], "");
        planilhaCarros.Columns().AdjustToContents();

        var fontesDetalhadas = carrosAchatados.SelectMany(c => c.FontesDetalhadas).ToList();
        var planilhaFontes = workbook.Worksheets.Add("Fontes");
        string[] cabecalho = { "LinhagemId", "Campo", "Fonte", "Valor", "Confiança", "Data Coleta", "Selecionada" };
        for (int i = 0; i < cabecalho.Length; i++) planilhaFontes.Cell(1, i + 1).Value = cabecalho[i];
        planilhaFontes.Row(1).Style.Font.Bold = true;
        for (int i = 0; i < fontesDetalhadas.Count; i++)
        {
            var f = fontesDetalhadas[i];
            planilhaFontes.Cell(i + 2, 1).Value = f.LinhagemId;
            planilhaFontes.Cell(i + 2, 2).Value = f.Campo;
            planilhaFontes.Cell(i + 2, 3).Value = f.Fonte;
            planilhaFontes.Cell(i + 2, 4).Value = f.Valor;
            planilhaFontes.Cell(i + 2, 5).Value = f.Confianca;
            planilhaFontes.Cell(i + 2, 6).Value = f.DataColeta;
            planilhaFontes.Cell(i + 2, 7).Value = f.Selecionada ? "Sim" : "Não";
        }
        planilhaFontes.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}