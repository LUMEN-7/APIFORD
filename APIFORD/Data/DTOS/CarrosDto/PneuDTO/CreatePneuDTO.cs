namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreatePneuDTO(
    PropriedadeScrapingDTO<string> Tipo,
    PropriedadeScrapingDTO<int> Aro,
    PropriedadeScrapingDTO<int> Largura,
    PropriedadeScrapingDTO<int> Perfil
);