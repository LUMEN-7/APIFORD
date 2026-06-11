namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class UpdateFonteDTO
{
    public string? Nome { get; set; }
    public string? Url { get; set; } // Adicionado
    public decimal? Confiabilidade { get; set; }

    public DateTime? DataAdicao { get; set; }
}
