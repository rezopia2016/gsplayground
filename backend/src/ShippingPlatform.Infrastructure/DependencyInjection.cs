using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Application.RulesEngine;
using ShippingPlatform.Application.Services;
using ShippingPlatform.Infrastructure.Carriers;
using ShippingPlatform.Infrastructure.D365;
using ShippingPlatform.Infrastructure.Labels;
using ShippingPlatform.Infrastructure.Persistence;
using RulesEngineImpl = ShippingPlatform.Application.RulesEngine.RulesEngine;

namespace ShippingPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddShippingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ShippingDatabase")));

        services.Configure<D365ClientOptions>(configuration.GetSection("D365"));

        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IKitRepository, KitRepository>();
        services.AddScoped<ICarrierRepository, CarrierRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<ID365SyncRepository, D365SyncRepository>();

        services.AddScoped<IShipmentService, ShipmentService>();
        services.AddScoped<IKitService, KitService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IRulesEngine, RulesEngineImpl>();
        services.AddScoped<ID365SyncOrchestrator, D365SyncOrchestrator>();

        services.AddSingleton<ID365AuthTokenProvider, D365AuthTokenProvider>();
        services.AddHttpClient<ID365IntegrationClient, D365IntegrationClient>((sp, client) =>
            {
                var opts = configuration.GetSection("D365").Get<D365ClientOptions>() ?? new D365ClientOptions();
                if (!string.IsNullOrWhiteSpace(opts.ResourceUrl))
                {
                    client.BaseAddress = new Uri(opts.ResourceUrl);
                }
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddScoped<ICarrierGateway, DhlCarrierGateway>();
        services.AddScoped<ICarrierGateway, UpsCarrierGateway>();
        services.AddScoped<ICarrierGateway, NitsuCarrierGateway>();
        services.AddScoped<ICarrierGatewayFactory, CarrierGatewayFactory>();

        services.AddHttpClient("DhlCarrierClient", (sp, client) =>
            client.BaseAddress = new Uri(configuration["Carriers:Dhl:BaseUrl"] ?? "https://api.dhl.com/"));
        services.AddHttpClient("UpsCarrierClient", (sp, client) =>
            client.BaseAddress = new Uri(configuration["Carriers:Ups:BaseUrl"] ?? "https://api.ups.com/"));
        services.AddHttpClient("NitsuCarrierClient", (sp, client) =>
            client.BaseAddress = new Uri(configuration["Carriers:Nitsu:BaseUrl"] ?? "https://api.nitsu.example/"));

        services.AddSingleton<ILabelRenderer, TemplateLabelRenderer>();
        services.AddSingleton<IPrintRoutingService, PrintRoutingService>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
