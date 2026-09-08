namespace APIFORD.Services.Schedule;

public class AgendamentoWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AgendamentoWorker> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(1);

    public AgendamentoWorker(IServiceScopeFactory scopeFactory, ILogger<AgendamentoWorker> logger)
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
                await scope.ServiceProvider.GetRequiredService<AgendamentoPesquisaService>().ProcessarPendentesAsync();
                await scope.ServiceProvider.GetRequiredService<EscutaLancamentoService>().ProcessarPendentesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar agendamentos pendentes.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
