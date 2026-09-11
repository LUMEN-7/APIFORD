namespace APIFORD.Data.DTOS.User;

public class UsuarioResumoDTO
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? FotoPerfilUrl { get; set; }
}