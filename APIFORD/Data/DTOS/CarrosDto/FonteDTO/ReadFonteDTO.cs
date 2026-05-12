namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadFonteDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Confiabilidade { get; set; }
    public bool Excluido { get; set; }
    public DateTime DataAdicao { get; set; }
}


