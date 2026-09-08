using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record ConsumoDTO(
    [property: Required] PropriedadeScrapingDTO<decimal> Cidade,
    [property: Required] PropriedadeScrapingDTO<decimal> Estrada
);