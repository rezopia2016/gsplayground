using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingPlatform.Application.Interfaces;

namespace ShippingPlatform.Api.Controllers;

/// <summary>Carrier admin console read endpoint (E10.S4) — new carriers are onboarded via configuration/seed data, not code changes (BR-013).</summary>
[ApiController]
[Route("api/carriers")]
[Authorize(Policy = "PlatformAdmin")]
public class CarriersController : ControllerBase
{
    private readonly ICarrierRepository _carriers;

    public CarriersController(ICarrierRepository carriers)
    {
        _carriers = carriers;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var carriers = await _carriers.GetActiveCarriersAsync(ct);
        return Ok(carriers.Select(c => new
        {
            c.Id,
            c.Code,
            c.Name,
            c.AdapterKey,
            c.IsActive,
            c.SupportsEdi,
            c.Priority,
            Services = c.Services.Select(s => new { s.ServiceCode, s.ServiceName, s.SupportsTemperatureControl, s.SupportsBiologicalMaterial })
        }));
    }
}
