using APIFORD.Util;
using System.ComponentModel.DataAnnotations;

namespace APIFORD.Data.DTOS.Notifications;

/// <summary>
/// Request — payload de criação de uma notificação (broadcast). O servidor decide o estado
/// inicial (Lida = false, DataCriacao = agora); não são campos que o cliente controla.
/// </summary>
public class CreateNotificationDTO : IValidatableObject
{
    [EnumDataType(typeof(NotificationTypes), ErrorMessage = "Tipo de notificação inválido.")]
    public NotificationTypes Tipo { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "O subtítulo deve ter no máximo 200 caracteres.")]
    public string? Subtitulo { get; set; }

    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(2000, ErrorMessage = "A mensagem deve ter no máximo 2000 caracteres.")]
    public string Mensagem { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoDestinoNotificacao), ErrorMessage = "Tipo de destino inválido.")]
    public TipoDestinoNotificacao TipoDestino { get; set; }

    public List<string>? UserIds { get; set; }   // obrigatório só quando TipoDestino == UsuariosEspecificos
    public int? LinhagemId { get; set; }          // obrigatório só quando TipoDestino == FavoritantesDeCarro

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TipoDestino == TipoDestinoNotificacao.UsuariosEspecificos && (UserIds == null || UserIds.Count == 0))
            yield return new ValidationResult("Informe ao menos um UserId quando o destino é UsuariosEspecificos.", new[] { nameof(UserIds) });

        if (TipoDestino == TipoDestinoNotificacao.FavoritantesDeCarro && LinhagemId == null)
            yield return new ValidationResult("Informe o LinhagemId quando o destino é FavoritantesDeCarro.", new[] { nameof(LinhagemId) });
    }
}