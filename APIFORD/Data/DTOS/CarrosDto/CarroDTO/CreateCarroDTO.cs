using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.CarrosDto;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record CreateCarroDTO(
    [property: Required(ErrorMessage = "O modelo é obrigatório.")]
    [property: StringLength(100)] string Modelo,

    [property: Required(ErrorMessage = "A marca é obrigatória.")]
    [property: StringLength(100)] string Marca,

    [property: Range(1900, 2100, ErrorMessage = "Ano inválido.")] int Ano,

    [property: Url(ErrorMessage = "URL de imagem inválida.")] string? ImagemUrl,

    List<EspecificacaoDTO>? Especificacoes = null,
    List<ConsumoDTO>? Consumos = null,
    List<DimensaoDTO>? Dimensoes = null,
    List<PneuDTO>? Pneus = null,
    List<ExtraDTO>? Extras = null,
    PropriedadeScrapingDTO<List<string>>? Modos = null,
    PropriedadeScrapingDTO<string>? Categoria = null
);