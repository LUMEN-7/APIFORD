using Microsoft.AspNetCore.Identity;

namespace APIFORD.Model.User;

public class User : IdentityUser
{
    // Caso precisa adicionar mais propriedades exclusivas ao usuário, basta coloCarro aqui.
    public ICollection<ModeloSalvo> ModelosSalvos { get; set; } // Relacionamento com Favoritos (Carros Salvos)
    public bool Excluido { get; set; } = false;
    public string? NomeExibicao { get; set; }
    public string? FotoPerfilUrl { get; set; }
    public User() : base()
    {   
    }
}
