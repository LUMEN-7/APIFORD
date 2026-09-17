using APIFORD.Data;
using APIFORD.Services.NotificationService;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Search;

public class BuscaJobWatcherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BuscaJobWatcherService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(15);

    public BuscaJobWatcherService(IServiceScopeFactory scopeFactory, ILogger<BuscaJobWatcherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<FordDbContext>();
                var pesquisaService = scope.ServiceProvider.GetRequiredService<PesquisaService>();
                var notificacaoService = scope.ServiceProvider.GetRequiredService<NotificacaoService>();
                var agora = DateTime.UtcNow;

                // A atualização condicional funciona como uma reivindicação atômica do job,
                // evitando que duas instâncias processem o mesmo resultado simultaneamente.
                var candidatos = await context.Jobs.AsNoTracking()
                    .Where(j => j.CarroId == null &&
                        (j.Status == "done" || (j.Status == "processing" && j.UpdatedAt < agora.AddMinutes(-10))))
                    .OrderBy(j => j.UpdatedAt)
                    .Take(50)
                    .Select(j => j.Id)
                    .ToListAsync(stoppingToken);

                foreach (var id in candidatos)
                {
                    var reivindicado = await context.Jobs
                        .Where(j => j.Id == id && j.CarroId == null &&
                            (j.Status == "done" || (j.Status == "processing" && j.UpdatedAt < agora.AddMinutes(-10))))
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(j => j.Status, "processing")
                            .SetProperty(j => j.UpdatedAt, agora), stoppingToken);
                    if (reivindicado == 0) continue;

                    var job = await context.Jobs.FindAsync([id], stoppingToken);
                    if (job == null) continue;

                    try
                    {
                        var carroDto = await pesquisaService.ProcessarJobPendenteAsync(job);
                        if (!string.IsNullOrEmpty(job.UserId) && !job.Notificado)
                        {
                            await notificacaoService.NotificarBuscaConcluidaAsync(job.UserId, job.Id, carroDto);
                            job.Notificado = true;
                        }

                        job.Status = "done";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha ao processar o job {JobId}", job.Id);
                        job.Status = "error";
                        job.Error = "Falha ao processar o resultado da busca.";
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Falha no ciclo de observação de jobs.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
