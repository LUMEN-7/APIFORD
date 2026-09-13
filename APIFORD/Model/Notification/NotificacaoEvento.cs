using APIFORD.Util;
using System.Text.Json;

namespace APIFORD.Model.Notification;

public class NotificacaoEvento
{
    public int Id { get; set; }
    public NotificationTypes Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public bool Excluido { get; set; } = false;
    // Model/NotificacaoEvento.cs — adiciona a property
    public int? LinhagemIdReferenciado { get; set; }

    // Navegação
    public ICollection<NotificacaoUsuario> Destinatarios { get; set; } = new List<NotificacaoUsuario>();
}
