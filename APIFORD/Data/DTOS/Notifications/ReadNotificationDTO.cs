using APIFORD.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APIFORD.Data.DTOS.Notifications;

public class ReadNotificationDTO
{
    public NotificationTypes Tipo { get; set; }
    public string Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public object Mensagem { get; set; }
    public bool Lido { get; set; } = true;
    public bool Excluido { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataLeitura { get; set; } = DateTime.Now;

}
