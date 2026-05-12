using APIFORD.Data.DTOS.Notifications;
using APIFORD.Services.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace APIFORD.Controllers.Notification;

[ApiController]
[Route("[controller]")]
public class NotificationController : BaseController<Model.Notificacao, CreateNotificationDTO, ReadNotificationDTO, UpdateNotificationDTO, int>
{
    private NotificacaoService _notificationService;

    public NotificationController(NotificacaoService notificationService) : base(notificationService)
    {
    }

}
