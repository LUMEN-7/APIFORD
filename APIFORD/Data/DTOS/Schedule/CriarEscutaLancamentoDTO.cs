using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Schedule;

public class CriarEscutaLancamentoDTO : IValidatableObject
{
    [Required(ErrorMessage = "A marca é obrigatória.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    public string Modelo { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Ano inválido.")]
    public int? Ano { get; set; }

    public DateTime? ExpiraEm { get; set; } // opcional — null = nunca expira sozinha

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiraEm.HasValue && ExpiraEm.Value <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "A data de expiração deve ser no futuro.",
                new[] { nameof(ExpiraEm) });
        }
    }
}