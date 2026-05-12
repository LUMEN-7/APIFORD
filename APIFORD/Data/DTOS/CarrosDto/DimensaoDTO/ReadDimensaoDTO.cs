namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadDimensaoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }    // Contexto do Carro
    public int FonteId { get; set; } // Contexto da Fonte
    public decimal Length { get; set; }
    public decimal Largura { get; set; }
    public decimal Altura { get; set; }
    public decimal EntreEixos { get; set; }
    public bool Excluido { get; set; }
    public DateTime DataColeta { get; set; }
    public DateTime? DataReferencia { get; set; }
}
