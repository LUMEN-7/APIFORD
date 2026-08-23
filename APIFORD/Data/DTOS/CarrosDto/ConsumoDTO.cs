namespace APIFORD.Data.DTOS.CarrosDto;

public record ConsumoDTO(
    PropriedadeScrapingDTO<decimal> Cidade,
    PropriedadeScrapingDTO<decimal> Estrada
);