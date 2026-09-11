using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record ExtraDTO(
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeTanque,
    [Required] PropriedadeScrapingDTO<string> TipoCombustivel,
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeCarga,
    [Required] PropriedadeScrapingDTO<decimal> CapacidadeReboque
);