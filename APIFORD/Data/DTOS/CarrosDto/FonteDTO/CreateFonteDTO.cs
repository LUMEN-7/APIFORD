using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateFonteDTO(
    [property: Required(ErrorMessage = "O nome da fonte é obrigatório.")]
    [property: StringLength(200)] string Nome,

    [property: Required(ErrorMessage = "A URL é obrigatória.")]
    [property: Url(ErrorMessage = "URL inválida.")] string Url,

    [property: Range(0.0, 1.0, ErrorMessage = "Confiabilidade deve estar entre 0 e 1.")] decimal Confiabilidade,

    DateTime DataAdicao
);