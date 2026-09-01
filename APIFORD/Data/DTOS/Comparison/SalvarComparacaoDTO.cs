namespace APIFORD.Data.DTOS.Comparison;

public class SalvarComparacaoDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string RequestPayload { get; set; } = string.Empty; // O front-end envia o JSON do request como string
}