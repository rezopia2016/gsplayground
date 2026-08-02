using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Application.Services;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Infrastructure.D365;

/// <summary>
/// Drains D365SyncRecord outbox rows and pushes/pulls the corresponding data via
/// <see cref="ID365IntegrationClient"/>. Designed to be invoked by a hosted background service on
/// a short interval (see Api/Program.cs) rather than inline with user requests.
/// </summary>
public class D365SyncOrchestrator : ID365SyncOrchestrator
{
    private readonly ID365SyncRepository _outbox;
    private readonly ID365IntegrationClient _client;
    private readonly D365ClientOptions _options;
    private readonly ILogger<D365SyncOrchestrator> _logger;

    public D365SyncOrchestrator(
        ID365SyncRepository outbox,
        ID365IntegrationClient client,
        IOptions<D365ClientOptions> options,
        ILogger<D365SyncOrchestrator> logger)
    {
        _outbox = outbox;
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task ProcessPendingAsync(CancellationToken ct = default)
    {
        var pending = await _outbox.GetPendingAsync(_options.MaxSyncAttempts, ct);

        foreach (var record in pending)
        {
            record.AttemptCount++;
            record.LastAttemptAtUtc = DateTimeOffset.UtcNow;

            try
            {
                await DispatchAsync(record, ct);
                record.Status = D365SyncStatus.Synced;
                record.CompletedAtUtc = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "D365 sync failed for record {RecordId}, attempt {Attempt}", record.Id, record.AttemptCount);
                record.Status = record.AttemptCount >= _options.MaxSyncAttempts
                    ? D365SyncStatus.Failed
                    : D365SyncStatus.Retrying;
                record.LastError = ex.Message;
            }

            await _outbox.UpdateAsync(record, ct);
        }
    }

    private Task DispatchAsync(Domain.Entities.D365SyncRecord record, CancellationToken ct) => record.EntityType switch
    {
        D365SyncEntityType.BillingEvent => _client.PostBillingEventAsync(
            JsonSerializer.Deserialize<D365BillingEventDto>(record.Payload)!, ct),
        D365SyncEntityType.InventoryEvent => _client.PostInventoryEventAsync(
            JsonSerializer.Deserialize<D365InventoryEventDto>(record.Payload)!, ct),
        _ => Task.CompletedTask // Order/CustomerAddress/ComplianceDocument sync handled by dedicated inbound pull jobs.
    };
}
