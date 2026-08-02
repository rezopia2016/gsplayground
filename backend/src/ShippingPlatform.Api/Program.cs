using Microsoft.Identity.Web;
using ShippingPlatform.Api.Services;
using ShippingPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Azure AD is the shared identity provider between D365 and the GoLIMS Shipping Platform
// (BR-064/UR-062) — the platform trusts the same tenant D365 users already authenticate against.
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LabOperations", p => p.RequireRole("LabOperationsCoordinator", "PlatformAdministrator"));
    options.AddPolicy("ShippingSpecialist", p => p.RequireRole("ShippingSpecialist", "PlatformAdministrator"));
    options.AddPolicy("Compliance", p => p.RequireRole("ComplianceOfficer", "PlatformAdministrator"));
    options.AddPolicy("Finance", p => p.RequireRole("FinanceAdministrator", "PlatformAdministrator"));
    options.AddPolicy("PlatformAdmin", p => p.RequireRole("PlatformAdministrator"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddShippingInfrastructure(builder.Configuration);
builder.Services.AddHostedService<D365SyncBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
