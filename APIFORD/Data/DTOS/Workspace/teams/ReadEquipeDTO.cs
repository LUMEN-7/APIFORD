using APIFORD.Model.Workspace.Teams;

namespace APIFORD.Data.DTOS.Workspace.teams;

public class ReadEquipeDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CodigoConvite { get; set; } = string.Empty;
    public int TotalMembros { get; set; }
    public PapelEquipe MeuPapel { get; set; }
}