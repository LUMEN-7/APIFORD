using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateFonteDTO(
    [ Required(ErrorMessage = "O nome da fonte é obrigatório.")]
    [ StringLength(200)] string Nome,

    [ Required(ErrorMessage = "A URL é obrigatória.")]
    [ Url(ErrorMessage = "URL inválida.")] string Url,

    [ Range(0.0, 1.0, ErrorMessage = "Confiabilidade deve estar entre 0 e 1.")] decimal Confiabilidade,

    DateTime DataAdicao
);