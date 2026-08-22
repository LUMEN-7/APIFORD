namespace APIFORD.Data.DTOS.CarrosDto;

public record EspecificacaoDTO(
    PropriedadeScrapingDTO<int> Potencia,
    PropriedadeScrapingDTO<int> Torque,
    PropriedadeScrapingDTO<int> PotenciaRpm,
    PropriedadeScrapingDTO<int> TorqueRpm,
    PropriedadeScrapingDTO<string> Transmissao,
    PropriedadeScrapingDTO<string> Tracao
);