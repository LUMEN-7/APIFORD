using APIFORD.Util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APIFORD.Data.DTOS.Notifications;

public class CreateNotificationDTO
{

    [Required(ErrorMessage = "O tipo de notificação é obrigatório.\n Tipos validos: \n\t    EMPRESA,\r\n    Carro,\r\n    UPDATE,\r\n    REPORT")]
    public NotificationTypes Tipo { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Titulo { get; set; }

    public string? Subtitulo { get; set; }

    [Required(ErrorMessage = "O conteúdo é obrigatório.")]
    public dynamic Mensagem { get; set; }

    public bool Lido { get; set; } = false;

    public DateTime? DataCriacao { get; set; }
    

}
