using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record DimensaoDTO(
    [property: Required] PropriedadeScrapingDTO<decimal> Comprimento,
    [property: Required] PropriedadeScrapingDTO<decimal> Largura,
    [property: Required] PropriedadeScrapingDTO<decimal> Altura,
    [property: Required] PropriedadeScrapingDTO<decimal> EntreEixos
);