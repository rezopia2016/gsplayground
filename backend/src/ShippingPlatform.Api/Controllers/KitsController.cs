using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Services;

namespace ShippingPlatform.Api.Controllers;

[ApiController]
[Route("api/kits")]
[Authorize(Policy = "LabOperations")]
public class KitsController : ControllerBase
{
    private readonly IKitService _kitService;

    public KitsController(IKitService kitService)
    {
        _kitService = kitService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KitResponse>> Get(Guid id, CancellationToken ct)
    {
        var kit = await _kitService.GetKitAsync(id, ct);
        return kit is null ? NotFound() : Ok(kit);
    }

    [HttpPost]
    public async Task<ActionResult<KitResponse>> Create(CreateKitRequest request, CancellationToken ct)
    {
        var kit = await _kitService.CreateKitAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = kit.Id }, kit);
    }

    [HttpPost("{id:guid}/components/{componentCode}/fulfill")]
    public async Task<ActionResult<KitResponse>> FulfillComponent(Guid id, string componentCode, CancellationToken ct) =>
        Ok(await _kitService.FulfillComponentAsync(id, componentCode, ct));

    [HttpPost("{id:guid}/dispatch")]
    public async Task<ActionResult<KitResponse>> Dispatch(Guid id, [FromBody] DispatchKitRequest request, CancellationToken ct) =>
        Ok(await _kitService.DispatchKitAsync(id, request.ActorUserId, request.Location, ct));

    [HttpPost("{id:guid}/custody-events")]
    [Authorize(Policy = "Compliance")]
    public async Task<IActionResult> RecordCustodyEvent(Guid id, RecordCustodyEventRequest request, CancellationToken ct)
    {
        await _kitService.RecordCustodyEventAsync(id, request, ct);
        return NoContent();
    }
}

public record DispatchKitRequest(string? ActorUserId, string? Location);
