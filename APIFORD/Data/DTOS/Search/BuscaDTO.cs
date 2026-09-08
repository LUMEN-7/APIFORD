using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIFORD.Data.DTOS.Search;

public class BuscaDTO
{
    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [StringLength(100)]
    public string? Model { get; set; }

    [Required(ErrorMessage = "A marca é obrigatória.")]
    [StringLength(100)]
    public string? Brand { get; set; }

    public int? Year { get; set; }
    //public List<string> Urls { get; set; } = new();

}
