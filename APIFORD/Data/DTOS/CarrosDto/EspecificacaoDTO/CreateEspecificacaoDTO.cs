namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateEspecificacaoDTO(
    PropriedadeScrapingDTO<int> Potencia,
    PropriedadeScrapingDTO<int> Torque,
    PropriedadeScrapingDTO<int> PotenciaRpm,
    PropriedadeScrapingDTO<int> TorqueRpm,
    PropriedadeScrapingDTO<string> Transmissao,
    PropriedadeScrapingDTO<string> Tracao
);