namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateExtraDTO(
    PropriedadeScrapingDTO<string> CapacidadeTanque,
    PropriedadeScrapingDTO<string> TipoCombustivel,
    PropriedadeScrapingDTO<string> CapacidadeCarga,
    PropriedadeScrapingDTO<string> CapacidadeReboque
);