namespace APIFORD.Data.DTOS.CarrosDto;

public record DimensaoDTO(
    PropriedadeScrapingDTO<decimal> Comprimento,
    PropriedadeScrapingDTO<decimal> Largura,
    PropriedadeScrapingDTO<decimal> Altura,
    PropriedadeScrapingDTO<decimal> EntreEixos
);