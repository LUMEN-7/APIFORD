namespace APIFORD.Data.DTOS.CarrosDto;

public record ExtraDTO(
    PropriedadeScrapingDTO<double> CapacidadeTanque,
    PropriedadeScrapingDTO<string> TipoCombustivel,
    PropriedadeScrapingDTO<double> CapacidadeCarga,
    PropriedadeScrapingDTO<double> CapacidadeReboque
);