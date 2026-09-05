using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Util;
using System.Text;
using System.Text.Json;

namespace APIFORD.Services.Export;

public class ExportadorJsonService : IExportadorFormatoService
{
    public string Formato => "json";
    public string ExtensaoArquivo => "json";
    public string ContentType => "application/json";

    public byte[] Exportar(List<ReadCarroDTO> carros, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(carros, new JsonSerializerOptions { WriteIndented = true }));
}

