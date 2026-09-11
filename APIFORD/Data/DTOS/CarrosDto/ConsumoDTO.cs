using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record ConsumoDTO(
    [Required] PropriedadeScrapingDTO<decimal> Cidade,
    [Required] PropriedadeScrapingDTO<decimal> Estrada
);