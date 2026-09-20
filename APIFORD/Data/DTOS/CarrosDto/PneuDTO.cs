using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record PneuDTO(
    [ Required] PropriedadeScrapingDTO<string> Tipo,
    [ Required] PropriedadeScrapingDTO<int> Aro,
    [ Required] PropriedadeScrapingDTO<int> Largura,
    [ Required] PropriedadeScrapingDTO<int> Perfil
)
{
    public PneuDTO() : this(default!, default!, default!, default!) { }
};