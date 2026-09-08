using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record PneuDTO(
    [property: Required] PropriedadeScrapingDTO<string> Tipo,
    [property: Required] PropriedadeScrapingDTO<int> Aro,
    [property: Required] PropriedadeScrapingDTO<int> Largura,
    [property: Required] PropriedadeScrapingDTO<int> Perfil
);