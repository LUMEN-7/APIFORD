namespace APIFORD.Data.DTOS.Export;

public class EscolhaManualDTO
{
    public int LinhagemId { get; set; }
    public string CampoCompleto { get; set; } = string.Empty; // ex: "Especificacoes.Potencia"
    public string FonteEscolhida { get; set; } = string.Empty;
}
