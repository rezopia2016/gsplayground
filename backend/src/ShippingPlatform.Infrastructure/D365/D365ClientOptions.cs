namespace ShippingPlatform.Infrastructure.D365;

/// <summary>Bound from configuration section "D365" (see appsettings.json).</summary>
public class D365ClientOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>D365 F&amp;SCM environment base URL, e.g. https://contoso.operations.dynamics.com</summary>
    public string ResourceUrl { get; set; } = string.Empty;

    /// <summary>OData root, typically "{ResourceUrl}/data".</summary>
    public string ODataPath { get; set; } = "/data";

    public int MaxSyncAttempts { get; set; } = 5;
}
