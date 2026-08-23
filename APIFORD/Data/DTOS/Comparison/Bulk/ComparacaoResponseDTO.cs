using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class ComparacaoResponseDTO
{
    public ReadCarroDTO CarroBase { get; set; }

    public List<ReadCarroDTO> ConcorrentesEncontrados { get; set; } = new();

    // Guarda as médias calculadas (Ex: "MediaPotencia": 210.5)
    public Dictionary<string, string> MediasDaCategoria { get; set; } = new();

    public Dictionary<string, string> ConclusoesMatematicas { get; set; } = new();

    // O texto final gerado pelo LLM no Python
    public string ParecerIA { get; set; } = string.Empty;
}
