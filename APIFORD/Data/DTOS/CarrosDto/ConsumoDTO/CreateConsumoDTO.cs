namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateConsumoDTO(
    PropriedadeScrapingDTO<string> Cidade,
    PropriedadeScrapingDTO<string> Estrada
);