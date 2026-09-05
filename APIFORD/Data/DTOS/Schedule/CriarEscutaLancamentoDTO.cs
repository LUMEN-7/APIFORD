namespace APIFORD.Data.DTOS.Schedule;

public class CriarEscutaLancamentoDTO
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Ano { get; set; }
    public DateTime? ExpiraEm { get; set; } // opcional — null = nunca expira sozinha
}