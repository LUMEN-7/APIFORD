namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateDimensaoDTO(
    PropriedadeScrapingDTO<decimal> Comprimento,
    PropriedadeScrapingDTO<decimal> Largura,
    PropriedadeScrapingDTO<decimal> Altura,
    PropriedadeScrapingDTO<decimal> EntreEixos
);