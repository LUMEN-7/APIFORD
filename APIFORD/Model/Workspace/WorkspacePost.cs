using APIFORD.Model.Workspace.enums;

namespace APIFORD.Model.Workspace;

public class WorkspacePost
{
    public int Id { get; set; }
    public string AutorUserId { get; set; } = string.Empty;
    public TipoPost Tipo { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string? ResponsavelUserId { get; set; }
    public StatusRevisao? Status { get; set; }
    public int EquipeId { get; set; }
    public TipoConteudoVinculado? TipoConteudoVinculado { get; set; }
    public int? ConteudoVinculadoId { get; set; }
    public string? ConteudoVinculadoTitulo { get; set; }

    public bool Fixado { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<WorkspaceComentario> Comentarios { get; set; } = new List<WorkspaceComentario>();
    public ICollection<WorkspaceCurtida> Curtidas { get; set; } = new List<WorkspaceCurtida>();
}