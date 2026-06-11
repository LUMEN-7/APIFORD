namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateExtraDTO
{
    public PropriedadeScrapingDTO<double>? CapacidadeTanque { get; set; }
    public PropriedadeScrapingDTO<string>? TipoCombustivel { get; set; }
    public PropriedadeScrapingDTO<double>? CapacidadeCarga { get; set; }
    public PropriedadeScrapingDTO<double>? CapacidadeReboque { get; set; }
}
