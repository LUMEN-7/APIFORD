namespace APIFORD.Data.DTOS.Comparison.Bulk;

public class FiltroComparacaoDTO
{
    public string Atributo { get; set; } = string.Empty; // Ex: "Potencia", "Torque", "ConsumoCidade"
    public string Operador { get; set; } = string.Empty; // Ex: ">", "<", ">=", "<=", "=="
    public string Valor { get; set; }
}
