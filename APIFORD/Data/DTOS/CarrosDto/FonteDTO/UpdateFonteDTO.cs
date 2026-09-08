using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateFonteDTO
{
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    public string? Nome { get; set; }

    [Url(ErrorMessage = "URL inválida.")]
    public string? Url { get; set; }

    [Range(0.0, 1.0, ErrorMessage = "Confiabilidade deve estar entre 0 e 1.")]
    public decimal? Confiabilidade { get; set; }

    public DateTime? DataAdicao { get; set; }
}