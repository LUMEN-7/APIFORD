namespace APIFORD.Data.DTOS.Export;

public class ConflitoCampoDTO
{
    public int LinhagemId { get; set; }
    public string CampoCompleto { get; set; } = string.Empty;
    public List<FonteExportadaDTO> Fontes { get; set; } = new();
}
