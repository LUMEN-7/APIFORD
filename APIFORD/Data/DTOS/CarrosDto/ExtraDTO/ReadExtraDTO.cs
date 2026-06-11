namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadExtraDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public PropriedadeScrapingDTO<string> CapacidadeTanque { get; set; } = null!;
    public PropriedadeScrapingDTO<string> TipoCombustivel { get; set; } = null!;
    public PropriedadeScrapingDTO<string> CapacidadeCarga { get; set; } = null!;
    public PropriedadeScrapingDTO<string> CapacidadeReboque { get; set; } = null!;
}