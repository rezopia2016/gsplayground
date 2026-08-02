namespace ShippingPlatform.Application.Services;

/// <summary>
/// Drains the D365 sync outbox (BR-063/E7.S5). Intended to run on a background timer/hosted
/// service so shipping/kit operations remain fully functional while D365 is unavailable or slow;
/// queued records are retried until they succeed or exceed the max attempt threshold, at which
/// point they surface on the reconciliation report (BR-051/E7.S6).
/// </summary>
public interface ID365SyncOrchestrator
{
    Task ProcessPendingAsync(CancellationToken ct = default);
}
