namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateConsumoDTO(
    PropriedadeScrapingDTO<double> Cidade,
    PropriedadeScrapingDTO<double> Estrada
);