using APIFORD.Data;
using APIFORD.Data.DTOS.Notifications;
using APIFORD.Model;
using AutoMapper;

namespace APIFORD.Services.NotificationService;

public class NotificacaoService : BaseService<Notificacao, CreateNotificationDTO, ReadNotificationDTO, UpdateNotificationDTO, int>
{
    private FordDbContext _context;
    private IMapper _mapper;

    public NotificacaoService(FordDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<ReadNotificationDTO> CreateAsync(CreateNotificationDTO dto)
    {
        
        Notificacao notificacao = Mapper.Map<Notificacao>(dto);
        notificacao.Mensagem = dto.Mensagem.ToString();
        await DbSet.AddAsync(notificacao);
        await Context.SaveChangesAsync();
        return Mapper.Map<ReadNotificationDTO>(notificacao);
    }

}
