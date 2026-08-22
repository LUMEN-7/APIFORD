
using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.CarrosDto;

public record CreateCarroDTO(
    string Modelo,
    string Marca,
    int Ano,
    List<EspecificacaoDTO>? Especificacoes = null,
    List<ConsumoDTO>? Consumos = null,
    List<DimensaoDTO>? Dimensoes = null,
    List<PneuDTO>? Pneus = null,
    List<ExtraDTO>? Extras = null,
    PropriedadeScrapingDTO<List<string>>? Modos = null,
    PropriedadeScrapingDTO<string>? Categoria = null

);
