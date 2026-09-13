namespace APIFORD.Data.DTOS.Annotations;

public class ReadAnotacaoDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public List<ReadBlocoDTO> Blocos { get; set; } = new();
    public DateTime AtualizadoEm { get; set; }

    public DateTime CriadoEm { get; set; }
}