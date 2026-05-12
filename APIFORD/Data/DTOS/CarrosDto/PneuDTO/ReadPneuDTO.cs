namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadPneuDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }    // Contexto do Carro
    public int FonteId { get; set; } // Contexto da Fonte
    public string Tipo { get; set; } = string.Empty; // Front/Rear
    public int Aro { get; set; }
    public int Largura { get; set; }
    public int AspectRatio { get; set; }
    public bool Excluido { get; set; }

    public DateTime DataColeta { get; set; }
    public DateTime? DataReferencia { get; set; }
}
