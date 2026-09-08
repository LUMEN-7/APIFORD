using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Comparison.Direct;

/// <summary>Request — compara  2 ou + veículos, ponta a ponta.</summary>
public class ComparacaoDiretaRequestDTO
{
    [Required(ErrorMessage = "Informe os veículos a comparar.")]
    [MinLength(2, ErrorMessage = "A comparação direta exige pelo menos 2 veículos.")]
    public List<int> CarrosIds { get; set; } = new();
}