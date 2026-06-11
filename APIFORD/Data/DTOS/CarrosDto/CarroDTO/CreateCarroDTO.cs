
using APIFORD.Data.DTOS.CarrosDTO.CarroDTO;

public record CreateCarroDTO(
    string Modelo,
    string Marca,
    int Ano,
    List<CreateEspecificacaoDTO>? Especificacoes = null,
    List<CreateConsumoDTO>? Consumos = null,
    List<CreateDimensaoDTO>? Dimensoes = null,
    List<CreatePneuDTO>? Pneus = null,
    List<CreateExtraDTO>? Extras = null,
    List<string>? ModosCarro = null


);
