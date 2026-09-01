using APIFORD.Util;
using System.Text.Json;

namespace APIFORD.Model;

public class Notificacao
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public NotificationTypes Type { get; set; }
    public string Titulo { get; set; }
    public string? Subtitle { get; set; } = string.Empty;
    public string Mensagem { get; set; }
    public bool Lida { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Excluido { get; set; } = false;

}
