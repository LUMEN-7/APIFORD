namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadConsumoDTO
{
    public int Id { get; set; }
    public int CarroId { get; set; }
    public bool Excluido { get; set; }
    public DateTime DataColeta { get; set; }

    public PropriedadeScrapingDTO<double> Cidade { get; set; } = null!;
    public PropriedadeScrapingDTO<double> Estrada { get; set; } = null!;
}
