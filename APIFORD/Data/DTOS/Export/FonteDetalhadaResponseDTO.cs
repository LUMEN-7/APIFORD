namespace APIFORD.Data.DTOS.Export;

public class FonteDetalhadaResponseDTO
{
    public int LinhagemId { get; set; }
    public string Campo { get; set; } = "";
    public string Fonte { get; set; } = "";
    public string Valor { get; set; } = "";
    public double Confianca { get; set; }
    public DateTime DataColeta { get; set; }
    public bool Selecionada { get; set; }
}
