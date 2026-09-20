using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record EspecificacaoDTO(
    [Required] PropriedadeScrapingDTO<int> Potencia,
    [Required] PropriedadeScrapingDTO<int> Torque,
    [Required] PropriedadeScrapingDTO<int> PotenciaRpm,
    [Required] PropriedadeScrapingDTO<int> TorqueRpm,
    [Required] PropriedadeScrapingDTO<string> Transmissao,
    [Required] PropriedadeScrapingDTO<string> Tracao
)
{
    public EspecificacaoDTO() : this(default!, default!, default!, default!, default!, default!) { }
}