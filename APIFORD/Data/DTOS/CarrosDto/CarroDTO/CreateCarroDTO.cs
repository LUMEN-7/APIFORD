using APIFORD.Data.DTOS;
using APIFORD.Data.DTOS.CarrosDto;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.CarrosDto;

public record CreateCarroDTO(
    [ Required(ErrorMessage = "O modelo é obrigatório.")]
    [ StringLength(100)] string Modelo,

    [ Required(ErrorMessage = "A marca é obrigatória.")]
    [ StringLength(100)] string Marca,

    [ Range(1900, 2100, ErrorMessage = "Ano inválido.")] int Ano,

    [ Url(ErrorMessage = "URL de imagem inválida.")] string? ImagemUrl,

    List<EspecificacaoDTO>? Especificacoes = null,
    List<ConsumoDTO>? Consumos = null,
    List<DimensaoDTO>? Dimensoes = null,
    List<PneuDTO>? Pneus = null,
    List<ExtraDTO>? Extras = null,
    PropriedadeScrapingDTO<List<string>>? Modos = null,
    PropriedadeScrapingDTO<string>? Categoria = null
);