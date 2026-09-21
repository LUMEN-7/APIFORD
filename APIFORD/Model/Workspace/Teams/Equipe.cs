namespace APIFORD.Model.Workspace.Teams;

public class Equipe
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CriadorUserId { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CodigoConvite { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<EquipeMembro> Membros { get; set; } = new List<EquipeMembro>();
}