namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadCarroDTO
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set; }
    public bool Excluido { get; set; }

    // Listas para suportar a orquestração 1:N
    public List<ReadEspecificacaoDTO> Especificacoes { get; set; } = new();
    public List<ReadConsumoDTO> Consumos { get; set; } = new();
    public List<ReadPneuDTO> Pneus { get; set; } = new();
    public List<ReadDimensaoDTO> Dimensoes { get; set; } = new();
    public List<ReadExtraDTO> Extras { get; set; } = new();
    public List<ReadFonteDTO> Fontes { get; set; } = new();
    public List<string> ModosCarro { get; set; } = new();
}

