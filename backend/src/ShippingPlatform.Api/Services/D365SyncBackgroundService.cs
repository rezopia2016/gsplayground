using ShippingPlatform.Application.Services;

namespace ShippingPlatform.Api.Services;

/// <summary>
/// Periodically drains the D365 sync outbox (BR-063/E7.S5). Runs independently of user request
/// traffic so shipment/kit operations are never blocked waiting on D365 availability.
/// </summary>
public class D365SyncBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<D365SyncBackgroundService> _logger;

    public D365SyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<D365SyncBackgroundService> logger)
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
                var orchestrator = scope.ServiceProvider.GetRequiredService<ID365SyncOrchestrator>();
                await orchestrator.ProcessPendingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "D365 sync background pass failed unexpectedly.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
