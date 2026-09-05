using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;
using APIFORD.Util;

namespace APIFORD.Services.Export;

public interface IExportadorFormatoService
{
    string Formato { get; }         // chave usada na requisição, ex: "csv"
    string ExtensaoArquivo { get; } // extensão real do arquivo, ex: "zip"
    string ContentType { get; }
    byte[] Exportar(List<ReadCarroDTO> carrosOriginais, List<ResultadoAchatamento> carrosAchatados, OpcoesExportacao opcoes);
}

public class OpcoesExportacao
{
    public string SeparadorCsv { get; set; } = ",";
}