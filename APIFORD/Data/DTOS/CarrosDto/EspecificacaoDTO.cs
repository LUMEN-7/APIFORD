using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record EspecificacaoDTO(
    [property: Required] PropriedadeScrapingDTO<int> Potencia,
    [property: Required] PropriedadeScrapingDTO<int> Torque,
    [property: Required] PropriedadeScrapingDTO<int> PotenciaRpm,
    [property: Required] PropriedadeScrapingDTO<int> TorqueRpm,
    [property: Required] PropriedadeScrapingDTO<string> Transmissao,
    [property: Required] PropriedadeScrapingDTO<string> Tracao
);