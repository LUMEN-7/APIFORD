namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadPneuDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public PropriedadeScrapingDTO<string> Tipo { get; set; } = null!;
    public PropriedadeScrapingDTO<int> Aro { get; set; } = null!;
    public PropriedadeScrapingDTO<int> Largura { get; set; } = null!;
    public PropriedadeScrapingDTO<int> Perfil { get; set; } = null!;
}