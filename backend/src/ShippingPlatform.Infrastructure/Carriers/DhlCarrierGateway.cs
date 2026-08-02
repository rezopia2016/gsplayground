using System.Net.Http.Json;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;

namespace ShippingPlatform.Infrastructure.Carriers;

/// <summary>
/// DHL Express MyDHL API adapter. Global carrier with a strong native connector story per the
/// D365 assessment (Carrier Integration: Partial/Medium) — implemented here behind the shared
/// ICarrierGateway abstraction so the shipment engine never depends on DHL-specific contracts.
/// </summary>
public class DhlCarrierGateway : ICarrierGateway
{
    public string AdapterKey => "dhl";

    private readonly HttpClient _httpClient;

    public DhlCarrierGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DhlCarrierClient");
    }

    public async Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("rates", request, ct);
        response.EnsureSuccessStatusCode();
        var rates = await response.Content.ReadFromJsonAsync<List<RateQuoteResponse>>(cancellationToken: ct);
        return rates ?? new List<RateQuoteResponse>();
    }

    public async Task<CarrierLabelResult> CreateLabelAsync(CarrierLabelRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("shipments", request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierLabelResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("DHL API returned an empty label response.");
    }

    public async Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"tracking/{trackingNumber}", ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierTrackingResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("DHL API returned an empty tracking response.");
    }

    public async Task VoidShipmentAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync($"shipments/{trackingNumber}", ct);
        response.EnsureSuccessStatusCode();
    }
}
