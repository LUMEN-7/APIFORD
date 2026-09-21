using APIFORD.Model.Workspace.enums;

namespace APIFORD.Model.Workspace;

public class AtividadeWorkspace
{
    public int Id { get; set; }
    public int EquipeId { get; set; }
    public TipoAtividade Tipo { get; set; }
    public int? PostId { get; set; }

    public string AtorUserId { get; set; } = string.Empty;      // quem executou a ação
    public string? AlvoUserId { get; set; }                     // quem foi atribuído (só p/ Atribuicao)
    public string? StatusNovo { get; set; }                     // só p/ StatusAlterado

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
