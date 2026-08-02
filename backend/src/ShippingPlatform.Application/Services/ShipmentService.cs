using System.Text.Json;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Application.RulesEngine;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.Services;

/// <summary>
/// Core shipment orchestration service (E1). Shipment creation/dispatch never calls D365 or a
/// carrier synchronously on the critical path in a way that can block the operation — carrier
/// calls are direct (needed to obtain a tracking number), but all D365 sync is queued through the
/// outbox (BR-063) so ERP downtime cannot block lab shipping.
/// </summary>
public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipments;
    private readonly ICarrierRepository _carriers;
    private readonly ICarrierGatewayFactory _carrierGatewayFactory;
    private readonly IRulesEngine _rulesEngine;
    private readonly ID365SyncRepository _d365Sync;

    public ShipmentService(
        IShipmentRepository shipments,
        ICarrierRepository carriers,
        ICarrierGatewayFactory carrierGatewayFactory,
        IRulesEngine rulesEngine,
        ID365SyncRepository d365Sync)
    {
        _shipments = shipments;
        _carriers = carriers;
        _carrierGatewayFactory = carrierGatewayFactory;
        _rulesEngine = rulesEngine;
        _d365Sync = d365Sync;
    }

    public async Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request, CancellationToken ct = default)
    {
        var ruleContext = new ShipmentContext(
            request.OriginCountryCode,
            request.DestinationCountryCode,
            request.MaterialClassification,
            request.RequiresTemperatureControl,
            request.CustomerType,
            request.IsVipHandling,
            null);

        var ruleResult = await _rulesEngine.EvaluateAsync(ruleContext, ct);
        if (ruleResult.IsBlocked)
        {
            throw new InvalidOperationException($"Shipment blocked by compliance rule: {ruleResult.BlockedReason}");
        }

        var shipment = new Shipment
        {
            D365OrderReference = request.D365OrderReference,
            Direction = request.Direction,
            OriginCountryCode = request.OriginCountryCode,
            DestinationCountryCode = request.DestinationCountryCode,
            OriginAddress = request.OriginAddress,
            DestinationAddress = request.DestinationAddress,
            MaterialClassification = request.MaterialClassification,
            RequiresTemperatureControl = request.RequiresTemperatureControl,
            CustomerType = request.CustomerType,
            IsVipHandling = request.IsVipHandling,
            KitId = request.KitId,
            ShipmentNumber = GenerateShipmentNumber(),
            Status = ShipmentStatus.Created
        };
        shipment.StatusHistory.Add(new ShipmentStatusHistory { ShipmentId = shipment.Id, Status = ShipmentStatus.Created });

        await _shipments.AddAsync(shipment, ct);

        if (!string.IsNullOrWhiteSpace(request.D365OrderReference))
        {
            await _d365Sync.EnqueueAsync(new D365SyncRecord
            {
                Direction = D365SyncDirection.Inbound,
                EntityType = D365SyncEntityType.Order,
                ShipmentId = shipment.Id,
                Payload = JsonSerializer.Serialize(new { request.D365OrderReference })
            }, ct);
        }

        return ToResponse(shipment);
    }

    public async Task<ShipmentResponse?> GetShipmentAsync(Guid id, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, ct);
        return shipment is null ? null : ToResponse(shipment);
    }

    public async Task<IReadOnlyList<ShipmentResponse>> ListShipmentsAsync(CancellationToken ct = default)
    {
        var shipments = await _shipments.ListAsync(ct);
        return shipments.Select(ToResponse).ToList();
    }

    public async Task<ShipmentResponse> DispatchAsync(Guid id, string carrierCode, string serviceCode, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Shipment {id} not found.");

        if (shipment.Status is ShipmentStatus.Dispatched or ShipmentStatus.InTransit or ShipmentStatus.Delivered)
        {
            throw new InvalidOperationException("Shipment has already been dispatched.");
        }

        var carrier = await _carriers.GetByCodeAsync(carrierCode, ct)
            ?? throw new KeyNotFoundException($"Carrier {carrierCode} not found or inactive.");

        var gateway = _carrierGatewayFactory.Resolve(carrier.AdapterKey);
        var totalWeight = shipment.Packages.Sum(p => p.WeightKg);

        var labelResult = await gateway.CreateLabelAsync(new CarrierLabelRequest(
            serviceCode,
            shipment.OriginAddress,
            shipment.DestinationAddress,
            totalWeight,
            shipment.RequiresTemperatureControl,
            shipment.MaterialClassification == MaterialClassification.BiologicalSample), ct);

        shipment.CarrierId = carrier.Id;
        shipment.CarrierServiceCode = serviceCode;
        shipment.CarrierTrackingNumber = labelResult.TrackingNumber;
        shipment.Status = ShipmentStatus.Dispatched;
        shipment.DispatchedAtUtc = DateTimeOffset.UtcNow;
        shipment.StatusHistory.Add(new ShipmentStatusHistory { ShipmentId = shipment.Id, Status = ShipmentStatus.Dispatched });

        await _shipments.UpdateAsync(shipment, ct);

        if (shipment.FreightCost.HasValue)
        {
            await _d365Sync.EnqueueAsync(new D365SyncRecord
            {
                Direction = D365SyncDirection.Outbound,
                EntityType = D365SyncEntityType.BillingEvent,
                ShipmentId = shipment.Id,
                Payload = JsonSerializer.Serialize(new
                {
                    shipment.ShipmentNumber,
                    Amount = shipment.FreightCost.Value,
                    shipment.CurrencyCode
                })
            }, ct);
        }

        return ToResponse(shipment);
    }

    public async Task<ShipmentResponse> CancelAsync(Guid id, string? reason, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Shipment {id} not found.");

        if (shipment.Status is not (ShipmentStatus.Created or ShipmentStatus.Ready))
        {
            throw new InvalidOperationException("Only shipments that have not yet been dispatched can be cancelled.");
        }

        shipment.Status = ShipmentStatus.Cancelled;
        shipment.StatusHistory.Add(new ShipmentStatusHistory
        {
            ShipmentId = shipment.Id,
            Status = ShipmentStatus.Cancelled,
            Notes = reason
        });

        await _shipments.UpdateAsync(shipment, ct);
        return ToResponse(shipment);
    }

    public async Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default)
    {
        var carriers = await _carriers.GetActiveCarriersAsync(ct);
        var results = new List<RateQuoteResponse>();

        foreach (var carrier in carriers.OrderBy(c => c.Priority))
        {
            var gateway = _carrierGatewayFactory.Resolve(carrier.AdapterKey);
            var rates = await gateway.GetRatesAsync(request, ct);
            results.AddRange(rates);
        }

        return results.OrderBy(r => r.Amount).ToList();
    }

    private static string GenerateShipmentNumber() => $"SHP-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private static ShipmentResponse ToResponse(Shipment s) => new(
        s.Id,
        s.ShipmentNumber,
        s.Status,
        s.Direction,
        s.OriginCountryCode,
        s.DestinationCountryCode,
        s.Carrier?.Code,
        s.CarrierServiceCode,
        s.CarrierTrackingNumber,
        s.FreightCost,
        s.CurrencyCode,
        s.CreatedAtUtc,
        s.DispatchedAtUtc,
        s.DeliveredAtUtc);
}
