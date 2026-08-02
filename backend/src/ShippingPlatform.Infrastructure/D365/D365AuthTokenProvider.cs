using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace ShippingPlatform.Infrastructure.D365;

/// <summary>
/// Acquires and caches an Azure AD app-only (client-credentials) token scoped to the D365
/// Finance &amp; Supply Chain Management environment, satisfying BR-064 (shared Azure AD identity)
/// and E7.S1. MSAL handles token caching/expiry internally.
/// </summary>
public interface ID365AuthTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
}

public class D365AuthTokenProvider : ID365AuthTokenProvider
{
    private readonly IConfidentialClientApplication _app;
    private readonly string[] _scopes;

    public D365AuthTokenProvider(IOptions<D365ClientOptions> options)
    {
        var opts = options.Value;
        _app = ConfidentialClientApplicationBuilder.Create(opts.ClientId)
            .WithClientSecret(opts.ClientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{opts.TenantId}")
            .Build();

        _scopes = new[] { $"{opts.ResourceUrl.TrimEnd('/')}/.default" };
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        var result = await _app.AcquireTokenForClient(_scopes).ExecuteAsync(ct);
        return result.AccessToken;
    }
}
