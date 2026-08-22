using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

namespace APIFORD.Data.DTOS.Comparison.Direct;

public class ComparacaoDiretaResponseDTO
{
    public List<ReadCarroDTO> CarrosComparados { get; set; }
    public string ParecerIA { get; set; } // Análise do Python sobre quem ganha em qual quesito
}
