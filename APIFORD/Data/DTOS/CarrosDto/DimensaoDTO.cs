using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record DimensaoDTO(
    [Required] PropriedadeScrapingDTO<decimal> Comprimento,
    [Required] PropriedadeScrapingDTO<decimal> Largura,
    [Required] PropriedadeScrapingDTO<decimal> Altura,
    [Required] PropriedadeScrapingDTO<decimal> EntreEixos
);