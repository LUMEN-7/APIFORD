namespace APIFORD.Model.Workspace;

public class WorkspaceComentario
{
    public int Id { get; set; }
    public int WorkspacePostId { get; set; }
    public WorkspacePost WorkspacePost { get; set; } = null!;
    public string AutorUserId { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}