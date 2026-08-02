# GoLIMS Shipping Platform

A hybrid shipping platform for Eurofins' global genomics/laboratory operations: **Angular** +
**.NET Core** + **SQL Server**, integrated with **Microsoft Dynamics 365 Finance & Supply Chain
Management**. D365 remains the system of record for financials, inventory, and standard
transportation; this platform owns shipping orchestration, carrier integration, label management,
kit/collection logistics, and lab-specific compliance — the capabilities the D365 assessment rated
weakest for a genomics operation.

See [`docs/01-system-overview.md`](docs/01-system-overview.md) for the full architecture rationale,
including the D365 capability assessment this project is built from.

## Repository layout

```
docs/                          Documentation set (see below)
backend/                       .NET Core Web API — ShippingPlatform
  src/ShippingPlatform.Domain
  src/ShippingPlatform.Application
  src/ShippingPlatform.Infrastructure
  src/ShippingPlatform.Api
  tests/ShippingPlatform.Tests
database/                      SQL Server schema scripts
  schema/001_init_schema.sql
  schema/002_seed_data.sql
  schema/003_custody_immutability.sql
frontend/                      Angular frontend — shipping-platform-ui
  src/app/core                 Models, services, HTTP interceptors
  src/app/features             Dashboard, shipments, kits, labels, tracking, carriers, D365 sync
```

## Documentation set

| Document | Purpose |
|---|---|
| [`docs/01-system-overview.md`](docs/01-system-overview.md) | D365 native-feature assessment, hybrid architecture rationale, target architecture |
| [`docs/02-business-requirements.md`](docs/02-business-requirements.md) | Business Requirements Document (BRD) |
| [`docs/03-user-requirements.md`](docs/03-user-requirements.md) | User Requirement Document (URD) — personas & user stories |
| [`docs/04-product-backlog.md`](docs/04-product-backlog.md) | Product backlog — epics, features, user stories, release roadmap |
| [`docs/05-test-cases.md`](docs/05-test-cases.md) | Test cases across all epics, plus non-functional test coverage |

## Architecture at a glance

```
GoLIMS Shipping Platform (Angular + .NET Core + SQL Server)
                            │
     ┌──────────────────────┼────────────────────────┐
     │                      │                        │
 Shipment Engine      Carrier Service        Label Service
     │                      │                        │
 Rules Engine         DHL / UPS / Nitsu      PDF / ZPL / QR
     │                      │                        │
 Package Mgmt        Tracking APIs          Print Service
     │                      │
 Customs Engine      EDI Integration
     │
 Order Orchestration
     │
      ───────────────────────────────────────────────
                      Integration Layer
                            │
          D365 Finance & Supply Chain Management
                            │
      Billing • Inventory • Financials • ERP • Customs Docs
```

D365 downtime/latency never blocks shipping: all ERP sync goes through an outbox
(`D365SyncRecord` + background worker) rather than being called synchronously from user requests.

## Getting started

This environment has Node.js/npm but not the .NET SDK, so nothing here has been built or run —
the code is a structurally complete scaffold to build on, not a verified binary.

**Backend** (see [`backend/README.md`](backend/README.md)):
```bash
cd backend && dotnet restore && dotnet run --project src/ShippingPlatform.Api
```

**Frontend** (see [`frontend/README.md`](frontend/README.md)):
```bash
cd frontend && npm install && npm start
```

**Database** (see [`database/README.md`](database/README.md)):
```bash
sqlcmd -S <server> -i database/schema/001_init_schema.sql
sqlcmd -S <server> -i database/schema/002_seed_data.sql
sqlcmd -S <server> -i database/schema/003_custody_immutability.sql
```

Both the API and the Angular app expect an Azure AD app registration shared with D365 (BR-064) —
fill in the placeholders in `backend/src/ShippingPlatform.Api/appsettings.json` and
`frontend/src/environments/environment.ts` with real tenant/client IDs before running.
