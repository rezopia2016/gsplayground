# ShippingPlatform (.NET Core Backend)

Clean-architecture ASP.NET Core Web API implementing the GoLIMS Shipping Platform described in
[`/docs/01-system-overview.md`](../docs/01-system-overview.md).

## Project layout

```
src/
  ShippingPlatform.Domain          Entities & enums (Shipment, Kit, Label, Carrier, RuleDefinition, D365SyncRecord)
  ShippingPlatform.Application     Interfaces (ports), DTOs, Rules Engine, application services
  ShippingPlatform.Infrastructure  EF Core (SQL Server), D365 OAuth2/OData client, carrier adapters, label renderer
  ShippingPlatform.Api             ASP.NET Core Web API host, controllers, Azure AD auth, background sync worker
tests/
  ShippingPlatform.Tests           xUnit tests for the Rules Engine and Kit lifecycle
```

## Key architectural points

- **D365 outbox pattern**: `D365SyncRecord` + `D365SyncOrchestrator` + `D365SyncBackgroundService`
  ensure shipment/kit operations never block on D365 availability (BR-063).
- **Carrier abstraction**: `ICarrierGateway` / `ICarrierGatewayFactory` with `DhlCarrierGateway`,
  `UpsCarrierGateway`, and `NitsuCarrierGateway` adapters. Onboarding a new carrier means adding a
  new `ICarrierGateway` implementation and a `Carrier` row with a matching `AdapterKey` — no
  changes to `ShipmentService` (BR-011–013).
- **Rules Engine**: `RuleDefinition` rows evaluated by `RulesEngine` against a `ShipmentContext`,
  so operations can change VIP/biological-material/compliance handling without a release (BR-041).
- **Kit & custody**: `Kit`/`KitComponent`/`ChainOfCustodyEvent` model partial fulfillment and an
  append-only custody trail — the domain concept D365 has no native equivalent for (BR-030–034).
- **Label service**: `ILabelRenderer` + `IPrintRoutingService` support the six lab label types
  across ZPL/PDF/QR with full reprint audit trail (BR-020–024).

## Running locally

> **Note:** this environment does not have the .NET SDK installed, so the project has not been
> built/restored here. To run it locally:

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/ShippingPlatform.Api
```

Update `src/ShippingPlatform.Api/appsettings.json` (or use `dotnet user-secrets` /
`appsettings.Development.json`) with real Azure AD, D365, and carrier sandbox credentials before
running — the checked-in file only contains placeholders.

Run tests:

```bash
dotnet test
```
