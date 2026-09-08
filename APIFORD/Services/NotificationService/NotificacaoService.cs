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

/// <summary>
/// Serviço responsável pela criação, distribuição e gerenciamento de notificações dos usuários,
/// incluindo o disparo em tempo real via SignalR (<see cref="NotificacaoHub"/>) para os
/// destinatários resolvidos (todos os usuários, usuários específicos ou favoritantes de um carro).
/// </summary>
public class NotificacaoService
{
    private readonly FordDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificacaoHub> _hubContext;
    
    /// <summary>
    /// Inicializa uma nova instância do <see cref="NotificacaoService"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados Ford.</param>
    /// <param name="mapper">Mapeador AutoMapper para conversão entre entidades e DTOs.</param>
    /// <param name="hubContext">Contexto do hub SignalR utilizado para enviar notificações em tempo real aos clientes conectados.</param>
    public NotificacaoService(FordDbContext context, IMapper mapper, IHubContext<NotificacaoHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Cria um evento de notificação e o distribui (broadcast) para todos os destinatários
    /// resolvidos a partir do tipo de destino informado, persistindo o evento e disparando
    /// o push em tempo real via SignalR.
    /// </summary>
    /// <param name="dto">
    /// Dados da notificação a ser criada, incluindo tipo, título, mensagem e o tipo de destino
    /// (todos os usuários, usuários específicos ou favoritantes de um carro).
    /// </param>
    /// <returns>O <see cref="ReadNotificationDTO"/> referente ao primeiro destinatário do evento criado.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando nenhum destinatário é encontrado para a notificação.</exception>
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

    /// <summary>
    /// Notifica os usuários que favoritaram um carro sobre uma atualização de dados na sua
    /// linhagem. Mantém a mesma assinatura utilizada anteriormente para não exigir alterações
    /// em quem já consome este método (ex: <c>CarroInternoController</c>).
    /// </summary>
    /// <param name="linhagemId">Id da linhagem do carro atualizado.</param>
    /// <param name="marca">Marca do carro atualizado, usada na mensagem da notificação.</param>
    /// <param name="modelo">Modelo do carro atualizado, usado na mensagem da notificação.</param>
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

    /// <summary>
    /// Notifica uma lista específica de usuários sobre o lançamento/disponibilidade de um carro
    /// que estava sendo aguardado, criando o evento de notificação, vinculando os destinatários
    /// e disparando o push em tempo real via SignalR.
    /// </summary>
    /// <param name="userIds">Lista de Ids dos usuários a serem notificados. Se vazia, o método não faz nada.</param>
    /// <param name="linhagemId">Id da linhagem do carro lançado.</param>
    /// <param name="marca">Marca do carro lançado, usada no título e na mensagem da notificação.</param>
    /// <param name="modelo">Modelo do carro lançado, usado no título e na mensagem da notificação.</param>
    public async Task NotificarLancamentoAsync(List<string> userIds, int linhagemId, string marca, string modelo)
    {
        if (!userIds.Any()) return;

        var evento = new NotificacaoEvento
        {
            Tipo = NotificationTypes.LANCAMENTO, 
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

    /// <summary>
    /// Resolve a lista de Ids de usuários destinatários de uma notificação, de acordo com o
    /// <see cref="TipoDestinoNotificacao"/> informado no DTO (todos os usuários, usuários
    /// específicos informados diretamente, ou usuários que favoritaram carros de uma linhagem).
    /// </summary>
    /// <param name="dto">Dados da notificação contendo o tipo de destino e, conforme o caso, a lista de UserIds ou o Id da linhagem.</param>
    /// <returns>Lista de Ids de usuários destinatários da notificação.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Lançada quando o <see cref="TipoDestinoNotificacao"/> informado não é reconhecido.</exception>
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

    /// <summary>
    /// Lista todas as notificações recebidas por um usuário, ordenadas da mais recente para a mais antiga.
    /// </summary>
    /// <param name="usuarioId">Id do usuário cujas notificações serão listadas.</param>
    /// <returns>Lista de <see cref="ReadNotificationDTO"/> representando as notificações do usuário.</returns>
    public async Task<List<ReadNotificationDTO>> ListarNotificacoesDoUsuarioAsync(string usuarioId)
    {
        var notificacoes = await _context.NotificacoesUsuarios
            .Include(nu => nu.Evento) // obrigatório - o AutoMapper depende disso
            .Where(nu => nu.UserId == usuarioId)
            .OrderByDescending(nu => nu.Evento.DataCriacao)
            .ToListAsync();

        return _mapper.Map<List<ReadNotificationDTO>>(notificacoes);
    }

    /// <summary>
    /// Marca uma notificação específica de um usuário como lida, registrando a data/hora da leitura.
    /// </summary>
    /// <param name="id">Id da notificação (registro de <c>NotificacaoUsuario</c>) a ser marcada como lida.</param>
    /// <param name="usuarioId">Id do usuário dono da notificação, usado para garantir que ele só marque as próprias notificações.</param>
    /// <returns><c>true</c> se a notificação foi encontrada e marcada como lida; <c>false</c> caso não seja encontrada para o usuário informado.</returns>
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

    /// <summary>
    /// Marca todas as notificações não lidas de um usuário como lidas, registrando a mesma
    /// data/hora de leitura para todas.
    /// </summary>
    /// <param name="usuarioId">Id do usuário cujas notificações não lidas serão marcadas.</param>
    /// <returns><c>true</c> em caso de sucesso (inclusive quando não havia notificações não lidas).</returns>
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
    /// <summary>
    /// Dispara, em paralelo, o push em tempo real via SignalR para cada destinatário de uma
    /// lista de notificações de usuário, enviando o evento "ReceberNovaNotificacao" ao cliente conectado.
    /// </summary>
    /// <param name="notificacoes">Lista de notificações de usuário a serem enviadas via push.</param>
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