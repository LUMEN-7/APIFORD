using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Util;
using System.Xml.Linq;

namespace APIFORD.Services.Export;

public class ExportadorXmlService : IExportadorFormatoService
{
    public string Formato => "xml";
    public string ExtensaoArquivo => "xml";
    public string ContentType => "application/xml";

    public byte[] Exportar(List<ReadCarroDTO> carrosOriginais, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(carrosOriginais);
        using var doc = System.Text.Json.JsonDocument.Parse(json);

        var raiz = new XElement("Carros");
        foreach (var elemento in doc.RootElement.EnumerateArray())
            raiz.Add(JsonParaXml(elemento, "Carro"));

        using var stream = new MemoryStream();
        new XDocument(raiz).Save(stream);
        return stream.ToArray();
    }

    private static XElement JsonParaXml(System.Text.Json.JsonElement elemento, string nome)
    {
        nome = NormalizarNomeXml(nome);

        switch (elemento.ValueKind)
        {
            case System.Text.Json.JsonValueKind.Object:
                var objEl = new XElement(nome);
                foreach (var prop in elemento.EnumerateObject())
                    objEl.Add(JsonParaXml(prop.Value, prop.Name));
                return objEl;

            case System.Text.Json.JsonValueKind.Array:
                var arrEl = new XElement(nome);
                foreach (var item in elemento.EnumerateArray())
                    arrEl.Add(JsonParaXml(item, "Item"));
                return arrEl;

            case System.Text.Json.JsonValueKind.Null:
                return new XElement(nome);

            default:
                return new XElement(nome, elemento.ToString());
        }
    }

    // Nome de elemento XML não pode começar com número
    private static string NormalizarNomeXml(string nome)
    {
        if (string.IsNullOrEmpty(nome)) return "Campo";
        if (char.IsDigit(nome[0])) nome = "_" + nome;
        return nome;
    }
}