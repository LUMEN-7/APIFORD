using APIFORD.Util;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Notifications;

/// <summary>
/// Request — payload de edição administrativa de uma notificação existente.
/// Update parcial: só os campos enviados são alterados.
/// </summary>
public class UpdateNotificationDTO
{
    [EnumDataType(typeof(NotificationTypes), ErrorMessage = "Tipo de notificação inválido.")]
    public NotificationTypes? Tipo { get; set; }

    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string? Titulo { get; set; }

    [StringLength(200, ErrorMessage = "O subtítulo deve ter no máximo 200 caracteres.")]
    public string? Subtitulo { get; set; }

    [StringLength(2000, ErrorMessage = "A mensagem deve ter no máximo 2000 caracteres.")]
    public string? Mensagem { get; set; } // era dynamic? — trocado por não ser bindável/validável

    public bool? Lido { get; set; } // sem default — precisa ficar null quando omitido

    public DateTime? DataCriacao { get; set; }
}