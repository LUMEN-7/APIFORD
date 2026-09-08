using APIFORD.Model.Enum;
using APIFORD.Util;
using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS.Schedule;

public class CriarAgendamentoDTO
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Ano { get; set; }
    [JsonConverter(typeof(DataBrasileiraConverter))]
    public DateTime DataAgendada { get; set; }
    public RecorrenciaAgendamento Recorrencia { get; set; } = RecorrenciaAgendamento.Unica;
}