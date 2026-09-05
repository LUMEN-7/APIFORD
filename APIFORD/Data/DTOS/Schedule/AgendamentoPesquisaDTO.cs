using APIFORD.Model.Enum;

namespace APIFORD.Data.DTOS.Schedule;

public class AgendamentoPesquisaDTO
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Ano { get; set; }
    public DateTime ProximaExecucao { get; set; }
    public RecorrenciaAgendamento Recorrencia { get; set; }
    public StatusAgendamento Status { get; set; }
    public DateTime? UltimaExecucao { get; set; }
}
