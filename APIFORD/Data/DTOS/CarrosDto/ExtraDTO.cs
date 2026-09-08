using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record ExtraDTO(
    [property: Required] PropriedadeScrapingDTO<decimal> CapacidadeTanque,
    [property: Required] PropriedadeScrapingDTO<string> TipoCombustivel,
    [property: Required] PropriedadeScrapingDTO<decimal> CapacidadeCarga,
    [property: Required] PropriedadeScrapingDTO<decimal> CapacidadeReboque
);