using APIFORD.Data;
using APIFORD.Data.DTOS.Search;
using APIFORD.Model.Enum;
using APIFORD.Model.Schedule;
using APIFORD.Model.User;
using APIFORD.Services.NotificationService;
using APIFORD.Services.Search;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace APIFORD.Services.Schedule;

public class EscutaLancamentoService
{
    private readonly FordDbContext _context;
    private readonly PesquisaService _pesquisaService;
    private readonly NotificacaoService _notificacaoService;

    public EscutaLancamentoService(FordDbContext context, PesquisaService pesquisaService, NotificacaoService notificacaoService)
    {
        _context = context;
        _pesquisaService = pesquisaService;
        _notificacaoService = notificacaoService;
    }

    public async Task<List<EscutaLancamento>> ListarPorUsuarioAsync(string userId)
    {
        return await _context.EscutasLancamentoUsuario
            .Where(u => u.UserId == userId)
            .Include(u => u.EscutaLancamento)
            .Where(u => u.EscutaLancamento.Status == StatusAgendamento.Ativo)
            .Select(u => u.EscutaLancamento)
            .ToListAsync();
    }

    public async Task CriarOuEntrarAsync(string userId, string marca, string modelo, int? ano, DateTime? expiraEm)
    {
        var escuta = await _context.EscutasLancamento.FirstOrDefaultAsync(e =>
            e.Marca == marca && e.Modelo == modelo && e.Ano == ano && e.Status == StatusAgendamento.Ativo);

        if (escuta == null)
        {
            escuta = new EscutaLancamento { Marca = marca, Modelo = modelo, Ano = ano, ExpiraEm = expiraEm };
            await _context.EscutasLancamento.AddAsync(escuta);
            await _context.SaveChangesAsync(); // gera o Id antes de linkar o usuário
        }

        bool jaEsta = await _context.EscutasLancamentoUsuario.AnyAsync(u => u.EscutaLancamentoId == escuta.Id && u.UserId == userId);
        if (!jaEsta)
        {
            await _context.EscutasLancamentoUsuario.AddAsync(new EscutaLancamentoUsuario { EscutaLancamentoId = escuta.Id, UserId = userId });
            await _context.SaveChangesAsync();
        }
    }

    // Sair da escuta: se o usuário era o último interessado, cancela a escuta inteira
    // (sem isso, o worker continuaria gastando ciclo com uma escuta que ninguém mais quer)
    public async Task SairAsync(int escutaId, string userId)
    {
        var vinculo = await _context.EscutasLancamentoUsuario
            .FirstOrDefaultAsync(u => u.EscutaLancamentoId == escutaId && u.UserId == userId);
        if (vinculo == null) return;

        _context.EscutasLancamentoUsuario.Remove(vinculo);
        await _context.SaveChangesAsync();

        bool aindaTemInteressado = await _context.EscutasLancamentoUsuario.AnyAsync(u => u.EscutaLancamentoId == escutaId);
        if (!aindaTemInteressado)
        {
            var escuta = await _context.EscutasLancamento.FindAsync(escutaId);
            if (escuta != null)
            {
                escuta.Status = StatusAgendamento.Cancelado;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task ProcessarPendentesAsync()
    {
        var agora = DateTime.UtcNow;

        // 1ª parte: checa jobs que já foram disparados em ciclos anteriores
        var comJobPendente = await _context.EscutasLancamento
            .Where(e => e.Status == StatusAgendamento.Ativo && e.UltimoJobId != null)
            .ToListAsync();

        foreach (var escuta in comJobPendente)
        {
            var job = await _context.Jobs.FindAsync(escuta.UltimoJobId);
            if (job == null || job.Status == "pending" || job.Status == "running") continue; // ainda rodando, deixa pro próximo ciclo

            if (job.Status == "done")
            {
                var resultado = JsonSerializer.Deserialize<JobResultDTO>(job.Result!);
                var carro = await _context.Carros.FirstOrDefaultAsync(c => c.Id == resultado.CarroId);
                if (carro != null)
                {
                    var userIds = await _context.EscutasLancamentoUsuario
                        .Where(u => u.EscutaLancamentoId == escuta.Id)
                        .Select(u => u.UserId)
                        .ToListAsync();

                    await _notificacaoService.NotificarLancamentoAsync(userIds, carro.LinhagemId, carro.Marca, carro.Modelo);

                    escuta.Status = StatusAgendamento.Concluido;
                    continue;
                }
            }

            // not_found ou error: limpa o job e deixa pronto pra tentar de novo no próximo ciclo agendado
            escuta.UltimoJobId = null;
        }

        // 2ª parte: dispara tentativa nova pra quem está na hora e não tem job rodando
        var prontosPraTentar = await _context.EscutasLancamento
            .Where(e => e.Status == StatusAgendamento.Ativo && e.UltimoJobId == null && e.ProximaTentativa <= agora)
            .ToListAsync();

        foreach (var escuta in prontosPraTentar)
        {
            if (escuta.ExpiraEm.HasValue && escuta.ExpiraEm.Value <= agora)
            {
                escuta.Status = StatusAgendamento.Cancelado;
                continue;
            }

            var job = await _pesquisaService.IniciarBusca(new BuscaDTO { Brand = escuta.Marca, Model = escuta.Modelo, Year = escuta.Ano });
            escuta.UltimoJobId = job;
            escuta.ProximaTentativa = agora.AddDays(1);
        }

        await _context.SaveChangesAsync();
    }
}