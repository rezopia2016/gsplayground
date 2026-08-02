using System.Net.Http.Json;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;

namespace ShippingPlatform.Infrastructure.Carriers;

/// <summary>
/// Nitsu (regional/specialty carrier) adapter — the concrete example from the D365 assessment of
/// a carrier with no standard D365 connector, requiring a custom API/EDI adapter (BR-012/E2.S4).
/// Demonstrates that adding a non-mainstream regional carrier is a self-contained adapter, not a
/// core platform or ERP change (BO-4).
/// </summary>
public class NitsuCarrierGateway : ICarrierGateway
{
    public string AdapterKey => "nitsu";

    private readonly HttpClient _httpClient;

    public NitsuCarrierGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NitsuCarrierClient");
    }

    public async Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/quote", request, ct);
        response.EnsureSuccessStatusCode();
        var rates = await response.Content.ReadFromJsonAsync<List<RateQuoteResponse>>(cancellationToken: ct);
        return rates ?? new List<RateQuoteResponse>();
    }

    public async Task<CarrierLabelResult> CreateLabelAsync(CarrierLabelRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/label", request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierLabelResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("Nitsu API returned an empty label response.");
    }

    public async Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/tracking/{trackingNumber}", ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierTrackingResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("Nitsu API returned an empty tracking response.");
    }

    public async Task VoidShipmentAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync($"api/v1/void/{trackingNumber}", content: null, ct);
        response.EnsureSuccessStatusCode();
    }
}
