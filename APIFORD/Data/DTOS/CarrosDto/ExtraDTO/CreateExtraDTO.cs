namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateExtraDTO(
    PropriedadeScrapingDTO<double> CapacidadeTanque,
    PropriedadeScrapingDTO<string> TipoCombustivel,
    PropriedadeScrapingDTO<double> CapacidadeCarga,
    PropriedadeScrapingDTO<double> CapacidadeReboque
);