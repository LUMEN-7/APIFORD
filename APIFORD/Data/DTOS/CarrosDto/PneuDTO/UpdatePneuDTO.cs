namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdatePneuDTO
{
    public PropriedadeScrapingDTO<string>? Tipo { get; set; }
    public PropriedadeScrapingDTO<int>? Aro { get; set; }
    public PropriedadeScrapingDTO<int>? Largura { get; set; }
    public PropriedadeScrapingDTO<int>? Perfil { get; set; }
}
