namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadDimensaoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public PropriedadeScrapingDTO<decimal> Comprimento { get; set; } = null!;
    public PropriedadeScrapingDTO<decimal> Largura { get; set; } = null!;
    public PropriedadeScrapingDTO<decimal> Altura { get; set; } = null!;
    public PropriedadeScrapingDTO<decimal> EntreEixos { get; set; } = null!;
}
