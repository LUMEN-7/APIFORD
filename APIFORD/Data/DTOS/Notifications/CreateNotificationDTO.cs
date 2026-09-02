using APIFORD.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APIFORD.Data.DTOS.Notifications;

public class CreateNotificationDTO
{

    public NotificationTypes Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string Mensagem { get; set; } = string.Empty;

    public TipoDestinoNotificacao TipoDestino { get; set; }

    public List<string>? UserIds { get; set; }     
    public int? LinhagemId { get; set; }

    public bool Lido { get; set; } = false;

    public DateTime? DataCriacao { get; set; }
    

}
