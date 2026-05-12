namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadEspecificacaoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }    // Contexto do Carro
    public int FonteId { get; set; } // Contexto da Fonte
    public string Potencia { get; set; }
    public string Torque { get; set; }
    public string PotenciaRpm { get; set; }
    public string TorqueRpm { get; set; }
    public string Transmissao { get; set; }
    public string Tracao { get; set; }

    public bool Excluido { get; set; }

    public DateTime DataColeta { get; set; }
    public DateTime? DataReferencia { get; set; }
}
