using APIFORD.Model.Enum;

namespace APIFORD.Model.Schedule;

public class AgendamentoPesquisa
{
    public int Id { get; set; }
    public string UserId { get; set; }

    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int? Ano { get; set; }
    public string? Notas { get; set; }
    public int? LinhagemId { get; set; }

    public DateTime ProximaExecucao { get; set; }
    public RecorrenciaAgendamento Recorrencia { get; set; } = RecorrenciaAgendamento.Unica;

    public StatusAgendamento Status { get; set; } = StatusAgendamento.Ativo;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? UltimaExecucao { get; set; }
}
