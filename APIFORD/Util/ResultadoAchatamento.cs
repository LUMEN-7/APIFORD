using APIFORD.Data.DTOS.Export;

namespace APIFORD.Util;

public class ResultadoAchatamento
{
    public Dictionary<string, string> Valores { get; set; } = new();
    public Dictionary<string, string> Origens { get; set; } = new(); // só os campos que tiveram conflito
    public List<FonteDetalhadaDTO> FontesDetalhadas { get; set; } = new();
}
