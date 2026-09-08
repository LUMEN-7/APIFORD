using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Comparison.Bulk;

/// <summary>Request (aninhado) — uma regra de filtro, ex: Potencia > 300.</summary>
public class FiltroComparacaoDTO
{
    [Required(ErrorMessage = "O atributo é obrigatório.")]
    public string Atributo { get; set; } = string.Empty; // Ex: "Potencia", "Torque", "ConsumoCidade"

    [Required(ErrorMessage = "O operador é obrigatório.")]
    [RegularExpression("^(>|<|>=|<=|==)$", ErrorMessage = "Operador inválido. Use >, <, >=, <= ou ==.")]
    public string Operador { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor é obrigatório.")]
    public string Valor { get; set; }
}