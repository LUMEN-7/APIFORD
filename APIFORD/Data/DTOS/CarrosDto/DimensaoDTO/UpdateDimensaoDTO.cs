namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateDimensaoDTO
{
    public PropriedadeScrapingDTO<decimal>? Comprimento { get; set; }
    public PropriedadeScrapingDTO<decimal>? Largura { get; set; }
    public PropriedadeScrapingDTO<decimal>? Altura { get; set; }
    public PropriedadeScrapingDTO<decimal>? EntreEixos { get; set; }
}
