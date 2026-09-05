using System.ComponentModel.DataAnnotations.Schema;

namespace APIFORD.Model.User;

public class ComparacaoSalva
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // O ID do usuário logado (pode ser int ou string dependendo do seu Auth)
    public string Titulo { get; set; } = string.Empty; // Ex: "Mustang vs Porsche" ou "Análise de SUVs"
    public string Tipo { get; set; } = string.Empty; // "Direta" ou "Grupo"
    public DateTime DataSalvamento { get; set; } = DateTime.UtcNow;

    
    [Column(TypeName = "jsonb")]
    public string RequestPayload { get; set; } = string.Empty;
}
