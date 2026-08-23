namespace APIFORD.Data.DTOS.CarrosDto;

public record ExtraDTO(
    PropriedadeScrapingDTO<decimal> CapacidadeTanque,
    PropriedadeScrapingDTO<string> TipoCombustivel,
    PropriedadeScrapingDTO<decimal> CapacidadeCarga,
    PropriedadeScrapingDTO<decimal> CapacidadeReboque
);