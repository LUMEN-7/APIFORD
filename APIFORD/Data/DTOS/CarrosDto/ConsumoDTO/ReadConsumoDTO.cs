namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadConsumoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }    // Contexto do Carro
    public int FonteId { get; set; } // Contexto da Fonte
    public string Cidade { get; set; } = string.Empty;
    public string Estrada { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime DataColeta { get; set; }
    public DateTime? DataReferencia { get; set; }
}
