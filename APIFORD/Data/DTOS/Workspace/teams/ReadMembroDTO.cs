using APIFORD.Model.Workspace.Teams;

namespace APIFORD.Data.DTOS.Workspace.teams;

public class ReadMembroDTO
{
    public string UserId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Iniciais { get; set; } = string.Empty;
    public PapelEquipe Papel { get; set; }
}