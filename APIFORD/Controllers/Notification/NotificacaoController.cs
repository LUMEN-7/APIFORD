using APIFORD.Data.DTOS.Notifications;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Notification;

[ApiController]
[Route("[controller]")]
public class NotificacaoController : BaseController<Model.Notificacao, CreateNotificationDTO, ReadNotificationDTO, UpdateNotificationDTO, int>
{
    private NotificacaoService _notificacaoService;

    public NotificacaoController(NotificacaoService notificacaoService) : base(notificacaoService)
    {
    }

}
