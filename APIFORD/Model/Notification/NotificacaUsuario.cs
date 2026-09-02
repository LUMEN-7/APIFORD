using System.ComponentModel.DataAnnotations.Schema;

namespace APIFORD.Model.Notification;

public class NotificacaoUsuario
{
    public int Id { get; set; } // <- este é o Id que o front usa pra marcar como lida
    public int NotificacaoEventoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool Lida { get; set; } = false;
    public DateTime? DataLeitura { get; set; }

    [ForeignKey(nameof(NotificacaoEventoId))]
    public NotificacaoEvento Evento { get; set; } = null!;
}
