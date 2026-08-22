using APIFORD.Data.DTOS.CarrosDto;

namespace APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public class ReadCarroDTO
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set; }
    public bool Excluido { get; set; }

    public List<EspecificacaoDTO> Especificacoes { get; set; } = new();
    public List<ConsumoDTO> Consumos { get; set; } = new();
    public List<PneuDTO> Pneus { get; set; } = new();
    public List<DimensaoDTO> Dimensoes { get; set; } = new();
    public List<ExtraDTO> Extras { get; set; } = new();
    public PropriedadeScrapingDTO<List<string>> Modos { get; set; } = new();
    public PropriedadeScrapingDTO<string> Categoria { get; set; } = new();

    public List<ReadFonteDTO> Fontes { get; set; } = new();
}

