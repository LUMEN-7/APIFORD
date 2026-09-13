namespace APIFORD.Data.DTOS.Comparison;

public class ReadComparacaoSalvaDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataSalvamento { get; set; }

    // O payload que o React vai usar para refazer a pesquisa
    public string RequestPayload { get; set; } = string.Empty;
}

