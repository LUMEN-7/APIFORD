using APIFORD.Data;
using APIFORD.Data.DTOS.Notifications;
using APIFORD.Hubs;
using APIFORD.Model;
using APIFORD.Model.Notification;
using APIFORD.Util;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.NotificationService;

public class NotificacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificacaoHub> _hubContext;

    public NotificacaoService(FordDbContext context, IMapper mapper, IHubContext<NotificacaoHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<ReadNotificationDTO> CriarBroadcastAsync(CreateNotificationDTO dto)
    {
        var destinatarios = await ResolverDestinatariosAsync(dto);

        if (!destinatarios.Any())
            throw new InvalidOperationException("Nenhum destinatário encontrado para essa notificação.");

        var evento = new NotificacaoEvento
        {
            Tipo = dto.Tipo,
            Titulo = dto.Titulo,
            Subtitulo = dto.Subtitulo,
            Mensagem = dto.Mensagem,
            DataCriacao = DateTime.UtcNow,
            Destinatarios = destinatarios.Select(userId => new NotificacaoUsuario
            {
                UserId = userId,
                Lida = false
            }).ToList()
        };

        await _context.NotificacoesEventos.AddAsync(evento);
        await _context.SaveChangesAsync(); // 1 única chamada: EF Core resolve a FK pela navegação

        await DispararPushAsync(evento.Destinatarios.ToList());

        return _mapper.Map<ReadNotificationDTO>(evento.Destinatarios.First());
    }

    // Mantém a mesma assinatura de antes -> o CarroInternoController continua funcionando sem mudar nada
    public async Task NotificarAtualizacaoCarroAsync(int linhagemId, string marca, string modelo)
    {
        await CriarBroadcastAsync(new CreateNotificationDTO
        {
            Tipo = 0, // ajusta pro valor/enum que representa "atualização de veículo"
            Titulo = "Atualização de Veículo",
            Mensagem = $"O veículo {marca} {modelo} que está nos seus favoritos acaba de receber uma atualização de dados!",
            TipoDestino = TipoDestinoNotificacao.FavoritantesDeCarro,
            LinhagemId = linhagemId
        });
    }

    public async Task NotificarLancamentoAsync(List<string> userIds, int linhagemId, string marca, string modelo)
    {
        if (!userIds.Any()) return;

        var evento = new NotificacaoEvento
        {
            Tipo = NotificationTypes.LANCAMENTO, // PLACEHOLDER — trocar pelo enum/valor certo de "lançamento" quando eu souber
            Titulo = $"{marca} {modelo} já está disponível!",
            Mensagem = $"O carro que você estava esperando ({marca} {modelo}) apareceu nas nossas fontes.",
            DataCriacao = DateTime.UtcNow
        };

        await _context.NotificacoesEventos.AddAsync(evento); // nome do DbSet — confere se bate com o seu
        await _context.SaveChangesAsync(); // gera o Id do evento antes de linkar os usuários

        var notificacoesUsuario = userIds.Select(userId => new NotificacaoUsuario
        {
            NotificacaoEventoId = evento.Id,
            UserId = userId,
            Lida = false
        }).ToList();

        await _context.NotificacoesUsuarios.AddRangeAsync(notificacoesUsuario);
        await _context.SaveChangesAsync();

        foreach (var userId in userIds)
            await _hubContext.Clients.User(userId).SendAsync("ReceberNovaNotificacao", evento);
    }

    private async Task<List<string>> ResolverDestinatariosAsync(CreateNotificationDTO dto)
    {
        return dto.TipoDestino switch
        {
            TipoDestinoNotificacao.Todos =>
                await _context.Users.Select(u => u.Id).Distinct().ToListAsync(), // ajusta pro seu DbSet de usuários

            TipoDestinoNotificacao.UsuariosEspecificos =>
                dto.UserIds ?? new List<string>(),

            TipoDestinoNotificacao.FavoritantesDeCarro =>
                await _context.ModeloSalvos
                    .Include(ms => ms.Carro)
                    .Where(ms => ms.Carro.LinhagemId == dto.LinhagemId)
                    .Select(ms => ms.UserId)
                    .Distinct()
                    .ToListAsync(),

            _ => throw new ArgumentOutOfRangeException(nameof(dto.TipoDestino))
        };
    }

    public async Task<List<ReadNotificationDTO>> ListarNotificacoesDoUsuarioAsync(string usuarioId)
    {
        var notificacoes = await _context.NotificacoesUsuarios
            .Include(nu => nu.Evento) // obrigatório - o AutoMapper depende disso
            .Where(nu => nu.UserId == usuarioId)
            .OrderByDescending(nu => nu.Evento.DataCriacao)
            .ToListAsync();

        return _mapper.Map<List<ReadNotificationDTO>>(notificacoes);
    }

    public async Task<bool> MarcarComoLidaAsync(int id, string usuarioId)
    {
        var notificacao = await _context.NotificacoesUsuarios
            .FirstOrDefaultAsync(nu => nu.Id == id && nu.UserId == usuarioId);

        if (notificacao == null) return false;

        notificacao.Lida = true;
        notificacao.DataLeitura = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarcarTodasComoLidasAsync(string usuarioId)
    {
        var naoLidas = await _context.NotificacoesUsuarios
            .Where(nu => nu.UserId == usuarioId && !nu.Lida)
            .ToListAsync();

        if (!naoLidas.Any()) return true;

        var agora = DateTime.UtcNow;
        foreach (var notif in naoLidas)
        {
            notif.Lida = true;
            notif.DataLeitura = agora;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // ponto 7 — o disparo do SignalR
    private async Task DispararPushAsync(List<NotificacaoUsuario> notificacoes)
    {
        var envios = notificacoes.Select(nu =>
        {
            var dto = _mapper.Map<ReadNotificationDTO>(nu);
            return _hubContext.Clients.User(nu.UserId).SendAsync("ReceberNovaNotificacao", dto);
        });

        await Task.WhenAll(envios); // antes era foreach+await sequencial; agora dispara em paralelo
    }
}