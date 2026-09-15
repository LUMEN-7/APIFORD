using APIFORD.Data;
using APIFORD.Services.NotificationService;
using Microsoft.EntityFrameworkCore;

namespace APIFORD.Services.Search;

public class BuscaJobWatcherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(15);

    public BuscaJobWatcherService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FordDbContext>();
            var pesquisaService = scope.ServiceProvider.GetRequiredService<PesquisaService>();
            var notificacaoService = scope.ServiceProvider.GetRequiredService<NotificacaoService>();

            var jobsConcluidos = await context.Jobs
                .Where(j => j.Status == "done" && j.CarroId == null)
                .ToListAsync(stoppingToken);
            //Console.WriteLine($"[Watcher] Jobs pendentes de processamento: {jobsConcluidos.Count}");

            foreach (var job in jobsConcluidos)
            {
                try
                {
                    var carroDto = await pesquisaService.ProcessarJobPendenteAsync(job);

                    if (!string.IsNullOrEmpty(job.UserId) && !job.Notificado)
                    {
                        await notificacaoService.NotificarBuscaConcluidaAsync(job.UserId, job.Id, carroDto);
                        job.Notificado = true;
                    }
                }
                catch (Exception ex)
                {
                    job.Status = "error";
                    job.Error = ex.Message;
                }
            }

            if (jobsConcluidos.Any())
                await context.SaveChangesAsync(stoppingToken);

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
