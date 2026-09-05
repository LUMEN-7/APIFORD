using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Util;
using System.Text;
using System.Text.Json;

namespace APIFORD.Services.Export;

public class ExportadorJsonLinesService : IExportadorFormatoService
{
    public string Formato => "jsonl";
    public string ExtensaoArquivo => "jsonl";
    public string ContentType => "application/x-ndjson";

    public byte[] Exportar(List<ReadCarroDTO> carros, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes)
    {
        var sb = new StringBuilder();
        foreach (var carro in carros)
            sb.AppendLine(JsonSerializer.Serialize(carro));
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
