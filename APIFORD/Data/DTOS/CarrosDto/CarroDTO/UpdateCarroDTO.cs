using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateCarroDTO
{
    [StringLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres.")]
    public string? Modelo { get; set; }

    [StringLength(100, ErrorMessage = "A marca deve ter no máximo 100 caracteres.")]
    public string? Marca { get; set; }

    [Range(1900, 2100, ErrorMessage = "Ano inválido.")]
    public int? Ano { get; set; }
}