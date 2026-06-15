namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadExtraDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public PropriedadeScrapingDTO<double> CapacidadeTanque { get; set; } = null!;
    public PropriedadeScrapingDTO<string> TipoCombustivel { get; set; } = null!;
    public PropriedadeScrapingDTO<double> CapacidadeCarga { get; set; } = null!;
    public PropriedadeScrapingDTO<double> CapacidadeReboque { get; set; } = null!;
}