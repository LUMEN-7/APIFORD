using APIFORD.Model.Enum;

namespace APIFORD.Data.DTOS.Schedule;

public class EscutaLancamentoDTO
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Ano { get; set; }
    public StatusAgendamento Status { get; set; }
    public DateTime? ExpiraEm { get; set; }
}
