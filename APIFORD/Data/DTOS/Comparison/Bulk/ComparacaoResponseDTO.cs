using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class ComparacaoResponseDTO
{
    public ReadCarroDTO CarroBase { get; set; }

    public List<ReadCarroDTO> ConcorrentesEncontrados { get; set; } = new();

    // Guarda as médias calculadas (Ex: "MediaPotencia": 210.5)
    public Dictionary<string, decimal> MediasDaCategoria { get; set; } = new();

    // O texto final gerado pelo LLM no Python
    public string ParecerIA { get; set; }
}
