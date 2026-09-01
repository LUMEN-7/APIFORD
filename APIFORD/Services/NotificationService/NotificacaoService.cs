using APIFORD.Data;
using APIFORD.Data.DTOS.Notifications;
using APIFORD.Hubs;
using APIFORD.Model;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.NotificationService;

public class NotificacaoService : BaseService<Notificacao, CreateNotificationDTO, ReadNotificationDTO, UpdateNotificationDTO, int>
{
    // Removi a declaração duplicada de _context e _mapper, pois elas 
    // já existem protegidas no BaseService (Context e Mapper/DbSet).
    private readonly IHubContext<NotificacaoHub> _hubContext;
    public NotificacaoService(
        FordDbContext context,
        IMapper mapper,
        IHubContext<NotificacaoHub> hubContext) : base(context, mapper)
    {
        _hubContext = hubContext;
    }

    public override async Task<ReadNotificationDTO> CreateAsync(CreateNotificationDTO dto)
    {
        Notificacao notificacao = Mapper.Map<Notificacao>(dto);
        notificacao.Mensagem = dto.Mensagem.ToString();
        // A DataCriacao e status de 'Lida = false' idealmente são setados no próprio modelo ou banco

        await DbSet.AddAsync(notificacao);
        await Context.SaveChangesAsync();

        // TODO Futuro: É exatamente AQUI que você chamaria o SignalR para empurrar 
        // a notificação para a tela do usuário em tempo real!

        return Mapper.Map<ReadNotificationDTO>(notificacao);
    }

    public async Task NotificarAtualizacaoCarroAsync(int carroId, string marca, string modelo)
    {
        // 1. Descobre todos os usuários que favoritaram esse carro específico
        var usuariosInteressados = await Context.ModeloSalvos
            .Where(ms => ms.CarroId == carroId)
            .Select(ms => ms.UserId)
            .ToListAsync();

        if (!usuariosInteressados.Any()) return; // Ninguém favoritou, aborta.

        // 2. Prepara as notificações para o banco de dados
        string mensagemAlerta = $"O veículo {marca} {modelo} que está nos seus favoritos acaba de receber uma atualização de dados!";

        var novasNotificacoes = usuariosInteressados.Select(userId => new Notificacao
        {
            UserId = userId,
            Titulo = "Atualização de Veículo",
            Mensagem = mensagemAlerta,
            Lida = false,
            DataCriacao = DateTime.UtcNow
        }).ToList();

        // 3. Salva todas de uma vez no banco (Performance)
        await DbSet.AddRangeAsync(novasNotificacoes);
        await Context.SaveChangesAsync();

        // 4. Mapeia para DTO para não vazar a entidade do banco no WebSocket
        var notificacoesDto = Mapper.Map<List<ReadNotificationDTO>>(novasNotificacoes);

        // 5. Dispara o Push Notification (SignalR) apenas para os interessados
        foreach (var notificacaoDto in notificacoesDto)
        {
            // Envia apenas para o Client que possui o JWT do UserId correspondente
            await _hubContext.Clients
                .User(notificacaoDto.UserId)
                .SendAsync("ReceberNovaNotificacao", notificacaoDto);
        }
    }

    public async Task<List<ReadNotificationDTO>> ListarNotificacoesDoUsuarioAsync(string usuarioId)
    {
        // Garante que traz apenas as notificações do usuário logado, ordenadas pelas mais recentes
        var notificacoes = await DbSet
            .Where(n => n.UserId == usuarioId) // Assumindo que Notificacao tem UserId
            .OrderByDescending(n => n.DataCriacao)
            .ToListAsync();

        return Mapper.Map<List<ReadNotificationDTO>>(notificacoes);
    }

    public async Task<bool> MarcarComoLidaAsync(int id, string usuarioId)
    {
        // Busca garantindo que a notificação pertence ao usuário que pediu a alteração
        var notificacao = await DbSet
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == usuarioId);

        if (notificacao == null) return false;

        notificacao.Lida = true; // Assumindo que sua model Notificacao tem um bool Lida
        await Context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarcarTodasComoLidasAsync(string usuarioId)
    {
        var notificacoesNaoLidas = await DbSet
            .Where(n => n.UserId == usuarioId && !n.Lida)
            .ToListAsync();

        if (!notificacoesNaoLidas.Any()) return true;

        foreach (var notif in notificacoesNaoLidas)
        {
            notif.Lida = true;
        }

        await Context.SaveChangesAsync();
        return true;
    }
}