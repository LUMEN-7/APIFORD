using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.Comparison.Direct;

public class ComparacaoDiretaResponseDTO
{
    public List<ReadCarroDTO> CarrosComparados { get; set; } = new();

    public Dictionary<string, string> ConclusoesMatematicas { get; set; } = new();
    public string ParecerIA { get; set; } = string.Empty; // Análise do Python sobre quem ganha em qual quesito
}
