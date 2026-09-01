using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace APIFORD.Hubs;

[Authorize] // Garante que apenas usuários com JWT válido conectem ao WebSocket
public class NotificacaoHub : Hub
{
    // O SignalR mapeia conexões automaticamente usando o ClaimTypes.NameIdentifier do JWT.
    // Não precisamos escrever lógica de controle de usuários aqui, o .NET faz sozinho.
}