using APIFORD.Model.Enum;
using APIFORD.Util;
using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS.Schedule;

public class AgendamentoPesquisaDTO
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Ano { get; set; }
    public int? LinhagemId { get; set; }
    [JsonConverter(typeof(DataBrasileiraConverter))]
    public DateTime ProximaExecucao { get; set; }
    public RecorrenciaAgendamento Recorrencia { get; set; }
    public StatusAgendamento Status { get; set; }
    [JsonConverter(typeof(DataBrasileiraConverter))]
    public DateTime? UltimaExecucao { get; set; }
    public string? Notas { get; set; }
}
