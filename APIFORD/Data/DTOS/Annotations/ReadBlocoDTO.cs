using APIFORD.Data.DTOS.Annotations.Cards;
using APIFORD.Model.Annotation;

namespace APIFORD.Data.DTOS.Annotations;

public class ReadBlocoDTO
{
    public string Id { get; set; } = string.Empty;
    public TipoBloco Tipo { get; set; }
    public string? Texto { get; set; }
    public CardCarroPreviewDTO? CardCarro { get; set; }
    public CardComparacaoPreviewDTO? CardComparacao { get; set; }
}