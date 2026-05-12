using APIFORD.Util;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Notifications;

public class UpdateNotificationDTO
{
    
    public NotificationTypes? Tipo { get; set; }

    public string? Titulo { get; set; }

    public string? Subtitulo { get; set; }
    public dynamic? Mensagem { get; set; }

    public bool? Lido { get; set; } = false;
    public DateTime? DataCriacao { get; set; }
}
