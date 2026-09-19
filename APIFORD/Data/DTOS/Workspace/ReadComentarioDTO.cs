namespace APIFORD.Data.DTOS.Workspace;

public class ReadComentarioDTO
{
    public int Id { get; set; }
    public AutorResumoDTO Autor { get; set; } = null!;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}