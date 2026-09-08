namespace APIFORD.Model.Annotation;

public class BlocoAnotacao
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N"); // pro front reordenar/editar bloco individual
    public TipoBloco Tipo { get; set; }
    public string? Texto { get; set; }                    // usado por Paragrafo/Titulo/Subtitulo/ItemLista
    public int? LinhagemIdReferenciado { get; set; }       // usado só por CardCarro
    public int? ComparacaoIdReferenciada { get; set; }     // usado só por CardComparacao
    public int Ordem { get; set; }
}