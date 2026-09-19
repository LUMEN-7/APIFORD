using APIFORD.Middleware;
using System.Text.Json;

namespace APIFORD.Services;

public interface IImportadorArquivoService
{
    Task<Dictionary<string, object>> ParseCsvAsync(IFormFile arquivo);
    Task<Dictionary<string, object>> ParseJsonAsync(IFormFile arquivo);
    Task<Dictionary<string, object>> ParseXlsxAsync(IFormFile arquivo);
    Task<Dictionary<string, object>> ParseXmlAsync(IFormFile arquivo);
}

public class ImportadorArquivoService : IImportadorArquivoService
{
    public async Task<Dictionary<string, object>> ParseCsvAsync(IFormFile arquivo)
    {
        using var stream = arquivo.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        if (!await csv.ReadAsync() || !csv.ReadHeader())
            throw new BadRequestException("CSV vazio ou sem cabeçalho.");
        if (!await csv.ReadAsync())
            throw new BadRequestException("CSV não tem nenhuma linha de dados.");

        var dados = new Dictionary<string, object>();
        foreach (var header in csv.HeaderRecord!)
        {
            var valor = csv.GetField(header);
            if (!string.IsNullOrWhiteSpace(valor)) dados[header] = valor;
        }
        return dados;
    }

    public async Task<Dictionary<string, object>> ParseJsonAsync(IFormFile arquivo)
    {
        using var stream = arquivo.OpenReadStream();
        var documento = await JsonDocument.ParseAsync(stream);

        var dados = new Dictionary<string, object>();
        foreach (var prop in documento.RootElement.EnumerateObject())
            dados[prop.Name] = prop.Value; // fica como JsonElement — AplicarValorNaPropriedade já sabe desserializar isso
        return dados;
    }

    public async Task<Dictionary<string, object>> ParseXlsxAsync(IFormFile arquivo)
    {
        using var stream = arquivo.OpenReadStream();
        using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        var planilha = workbook.Worksheets.First();

        var cabecalhos = planilha.Row(1).CellsUsed().Select(c => c.GetString()).ToList();
        var linhaDados = planilha.Row(2); // assume 1 linha de dados = 1 carro — ver ressalva abaixo

        var dados = new Dictionary<string, object>();
        for (int i = 0; i < cabecalhos.Count; i++)
        {
            var valor = linhaDados.Cell(i + 1).GetString();
            if (!string.IsNullOrWhiteSpace(valor)) dados[cabecalhos[i]] = valor;
        }
        return dados;
    }

    public async Task<Dictionary<string, object>> ParseXmlAsync(IFormFile arquivo)
    {
        using var stream = arquivo.OpenReadStream();
        var documento = await System.Xml.Linq.XDocument.LoadAsync(stream, System.Xml.Linq.LoadOptions.None, default);

        var raiz = documento.Root ?? throw new BadRequestException("XML vazio ou mal formado.");
        var dados = new Dictionary<string, object>();
        foreach (var elemento in raiz.Elements())
            if (!string.IsNullOrWhiteSpace(elemento.Value)) dados[elemento.Name.LocalName] = elemento.Value;
        return dados;
    }
}