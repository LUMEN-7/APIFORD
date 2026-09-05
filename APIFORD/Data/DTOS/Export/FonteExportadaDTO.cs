namespace APIFORD.Data.DTOS.Export;

public class FonteExportadaDTO
{
    public string Fonte { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public double Confianca { get; set; }
    public DateTime DataColeta { get; set; }
}