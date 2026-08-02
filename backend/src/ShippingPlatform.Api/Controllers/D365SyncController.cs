using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Api.Controllers;

/// <summary>Reconciliation reporting for the D365 sync outbox (BR-051/E7.S6, UR-051).</summary>
[ApiController]
[Route("api/d365-sync")]
[Authorize(Policy = "Finance")]
public class D365SyncController : ControllerBase
{
    private readonly ID365SyncRepository _syncRepository;

    public D365SyncController(ID365SyncRepository syncRepository)
    {
        _syncRepository = syncRepository;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending([FromQuery] int maxAttempts, CancellationToken ct)
    {
        var records = await _syncRepository.GetPendingAsync(maxAttempts <= 0 ? 5 : maxAttempts, ct);
        return Ok(records.Select(r => new
        {
            r.Id,
            r.Direction,
            r.EntityType,
            r.Status,
            r.AttemptCount,
            r.LastError,
            r.CreatedAtUtc,
            r.LastAttemptAtUtc
        }));
    }
}
