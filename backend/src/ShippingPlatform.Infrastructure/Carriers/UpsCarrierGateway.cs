using System.Net.Http.Json;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;

namespace ShippingPlatform.Infrastructure.Carriers;

/// <summary>UPS API adapter — see DhlCarrierGateway remarks; identical contract, different wire format under the hood.</summary>
public class UpsCarrierGateway : ICarrierGateway
{
    public string AdapterKey => "ups";

    private readonly HttpClient _httpClient;

    public UpsCarrierGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UpsCarrierClient");
    }

    public async Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("rating/v2409/Shop", request, ct);
        response.EnsureSuccessStatusCode();
        var rates = await response.Content.ReadFromJsonAsync<List<RateQuoteResponse>>(cancellationToken: ct);
        return rates ?? new List<RateQuoteResponse>();
    }

    public async Task<CarrierLabelResult> CreateLabelAsync(CarrierLabelRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("shipments/v2409/ship", request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierLabelResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("UPS API returned an empty label response.");
    }

    public async Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"track/v1/details/{trackingNumber}", ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CarrierTrackingResult>(cancellationToken: ct);
        return result ?? throw new InvalidOperationException("UPS API returned an empty tracking response.");
    }

    public async Task VoidShipmentAsync(string trackingNumber, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync($"shipments/v2409/void/cancel/{trackingNumber}", ct);
        response.EnsureSuccessStatusCode();
    }
}
