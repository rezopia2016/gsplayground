namespace ShippingPlatform.Application.Interfaces;

/// <summary>
/// Thin, resilient client over D365 Finance &amp; Supply Chain Management's Data Entities /
/// OData API. All calls should go through the outbox (ID365SyncRepository) rather than being
/// invoked synchronously from user-facing request paths, so D365 latency/downtime never blocks
/// shipping operations (BR-063).
/// </summary>
public interface ID365IntegrationClient
{
    Task<D365OrderDto?> GetOrderAsync(string orderReference, CancellationToken ct = default);

    Task<D365AddressDto?> GetCustomerAddressAsync(string customerAccount, CancellationToken ct = default);

    Task PostBillingEventAsync(D365BillingEventDto billingEvent, CancellationToken ct = default);

    Task PostInventoryEventAsync(D365InventoryEventDto inventoryEvent, CancellationToken ct = default);
}

public record D365OrderDto(string OrderReference, string CustomerAccount, string CurrencyCode, decimal OrderTotal);

public record D365AddressDto(string CustomerAccount, string AddressLine1, string City, string CountryCode, string PostalCode);

public record D365BillingEventDto(string ShipmentReference, string CustomerAccount, decimal Amount, string CurrencyCode, DateTimeOffset EventDateUtc);

public record D365InventoryEventDto(string KitReference, string ItemNumber, int QuantityDelta, string Warehouse, DateTimeOffset EventDateUtc);
