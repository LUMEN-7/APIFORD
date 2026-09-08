namespace APIFORD.Model.Annotation;

public enum TipoBloco { Paragrafo, Titulo, Subtitulo, ItemLista, CardCarro, CardComparacao }


public class Anotacao
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public List<BlocoAnotacao> Blocos { get; set; } = new(); // JSONB, mesmo padrão OwnsMany/.ToJson() do resto da API
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}

