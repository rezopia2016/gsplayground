using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Services;

namespace ShippingPlatform.Api.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<IReadOnlyList<ShipmentResponse>>> List(CancellationToken ct) =>
        Ok(await _shipmentService.ListShipmentsAsync(ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<ShipmentResponse>> Get(Guid id, CancellationToken ct)
    {
        var shipment = await _shipmentService.GetShipmentAsync(id, ct);
        return shipment is null ? NotFound() : Ok(shipment);
    }

    [HttpPost]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest request, CancellationToken ct)
    {
        var shipment = await _shipmentService.CreateShipmentAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = shipment.Id }, shipment);
    }

    [HttpPost("{id:guid}/dispatch")]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<ShipmentResponse>> Dispatch(Guid id, [FromBody] DispatchShipmentRequest request, CancellationToken ct) =>
        Ok(await _shipmentService.DispatchAsync(id, request.CarrierCode, request.ServiceCode, ct));

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<ShipmentResponse>> Cancel(Guid id, [FromBody] CancelShipmentRequest request, CancellationToken ct) =>
        Ok(await _shipmentService.CancelAsync(id, request.Reason, ct));

    [HttpPost("rates")]
    [Authorize(Policy = "ShippingSpecialist")]
    public async Task<ActionResult<IReadOnlyList<RateQuoteResponse>>> GetRates(RateQuoteRequest request, CancellationToken ct) =>
        Ok(await _shipmentService.GetRatesAsync(request, ct));
}

public record DispatchShipmentRequest(string CarrierCode, string ServiceCode);
public record CancelShipmentRequest(string? Reason);
