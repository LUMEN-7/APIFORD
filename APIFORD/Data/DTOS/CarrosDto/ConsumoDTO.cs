namespace APIFORD.Data.DTOS.CarrosDto;

public record ConsumoDTO(
    PropriedadeScrapingDTO<double> Cidade,
    PropriedadeScrapingDTO<double> Estrada
);