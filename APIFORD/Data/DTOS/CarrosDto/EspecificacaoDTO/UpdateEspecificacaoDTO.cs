namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateEspecificacaoDTO
{
    public PropriedadeScrapingDTO<int>? Potencia { get; set; }
    public PropriedadeScrapingDTO<int>? Torque { get; set; }
    public PropriedadeScrapingDTO<int>? PotenciaRpm { get; set; }
    public PropriedadeScrapingDTO<int>? TorqueRpm { get; set; }
    public PropriedadeScrapingDTO<string>? Transmissao { get; set; }
    public PropriedadeScrapingDTO<string>? Tracao { get; set; }
}
