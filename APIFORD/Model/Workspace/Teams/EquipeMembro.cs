namespace APIFORD.Model.Workspace.Teams;

public class EquipeMembro
{
    public int Id { get; set; }
    public int EquipeId { get; set; }
    public Equipe Equipe { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public PapelEquipe Papel { get; set; } = PapelEquipe.Membro; // Administrador can add/remove members
    public DateTime EntrouEm { get; set; } = DateTime.UtcNow;
}