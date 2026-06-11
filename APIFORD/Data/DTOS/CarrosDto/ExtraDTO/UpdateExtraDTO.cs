namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateExtraDTO
{
    public PropriedadeScrapingDTO<string>? CapacidadeTanque { get; set; }
    public PropriedadeScrapingDTO<string>? TipoCombustivel { get; set; }
    public PropriedadeScrapingDTO<string>? CapacidadeCarga { get; set; }
    public PropriedadeScrapingDTO<string>? CapacidadeReboque { get; set; }
}
