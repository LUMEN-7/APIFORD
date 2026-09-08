using APIFORD.Model.Enum;
using APIFORD.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS.Schedule;

public class CriarAgendamentoDTO : IValidatableObject
{
    [Required(ErrorMessage = "A marca é obrigatória.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    public string Modelo { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Ano inválido.")]
    public int? Ano { get; set; }

    [JsonConverter(typeof(DataBrasileiraConverter))]
    public DateTime DataAgendada { get; set; }

    [EnumDataType(typeof(RecorrenciaAgendamento), ErrorMessage = "Recorrência inválida.")]
    public RecorrenciaAgendamento Recorrencia { get; set; } = RecorrenciaAgendamento.Unica;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataAgendada <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "A data agendada deve ser no futuro.",
                new[] { nameof(DataAgendada) });
        }
    }
}