using APIFORD.Data.DTOS.Notifications;
using APIFORD.Model.Notification;
using AutoMapper;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace APIFORD.Perfils;

public class NotificationPerfil : Profile
{
    public NotificationPerfil()
    {
        CreateMap<NotificacaoUsuario, ReadNotificationDTO>()
                   .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Evento.Tipo))
                   .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Evento.Titulo))
                   .ForMember(dest => dest.Subtitulo, opt => opt.MapFrom(src => src.Evento.Subtitulo))
                   .ForMember(dest => dest.Mensagem, opt => opt.MapFrom(src => src.Evento.Mensagem))
                   .ForMember(dest => dest.LinhagemIdReferenciado, opt => opt.MapFrom(src => src.Evento.LinhagemIdReferenciado))
                   .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.Evento.DataCriacao));
        // NotificacaoProfile.cs — adiciona ao CreateMap<NotificacaoUsuario, ReadNotificationDTO> já existente

        CreateMap<CreateNotificationDTO, NotificacaoEvento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore()) // setado 1x no Service, não aqui
            .ForMember(dest => dest.Destinatarios, opt => opt.Ignore());
    }

}
