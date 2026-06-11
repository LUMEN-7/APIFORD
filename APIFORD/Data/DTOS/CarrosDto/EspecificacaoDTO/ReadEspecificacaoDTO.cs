namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadEspecificacaoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public DateTime DataColeta { get; set; }

    public PropriedadeScrapingDTO<int> Potencia { get; set; } = null!;
    public PropriedadeScrapingDTO<int> Torque { get; set; } = null!;
    public PropriedadeScrapingDTO<int> PotenciaRpm { get; set; } = null!;
    public PropriedadeScrapingDTO<int> TorqueRpm { get; set; } = null!;
    public PropriedadeScrapingDTO<string> Transmissao { get; set; } = null!;
    public PropriedadeScrapingDTO<string> Tracao { get; set; } = null!;
}
