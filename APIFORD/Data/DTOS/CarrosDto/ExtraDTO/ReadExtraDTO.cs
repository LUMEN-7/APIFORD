namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadExtraDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }    // Contexto do Carro
    public int FonteId { get; set; } // Contexto da Fonte
    public string CapacidadeTanque { get; set; }
    public string TipoCombustivel { get; set; }
    public string CapacidadeCarga { get; set; }
    public string CapacidadeReboque { get; set; }
    public bool Excluido { get; set; }

    public DateTime DataColeta { get; set; }
    public DateTime? DataReferencia { get; set; }
}