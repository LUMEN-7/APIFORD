namespace APIFORD.Data.DTOS.CarrosDto;

public record PneuDTO(
    PropriedadeScrapingDTO<string> Tipo,
    PropriedadeScrapingDTO<int> Aro,
    PropriedadeScrapingDTO<int> Largura,
    PropriedadeScrapingDTO<int> Perfil
);