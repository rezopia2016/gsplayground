# shipping-platform-ui (Angular Frontend)

Angular (standalone components) frontend for the GoLIMS Shipping Platform. See
[`/docs/01-system-overview.md`](../docs/01-system-overview.md) for the overall architecture.

## Features

| Route | Component | Backlog Story |
|---|---|---|
| `/dashboard` | `DashboardComponent` | E8.S1 — operational shipping dashboard |
| `/shipments` | `ShipmentListComponent` | E1.S6 — shipment list/search |
| `/shipments/new` | `ShipmentCreateComponent` | E1.S1 — create shipment, multi-carrier rate comparison |
| `/kits` | `KitLogisticsComponent` | E4.S6 — kit lifecycle, packaging validation |
| `/labels` | `LabelManagementComponent` | E3 — multi-type/format label generation & reprint |
| `/tracking` | `TrackingComponent` | UR-003/033 — shipment tracking lookup |
| `/carriers` | `CarrierAdminComponent` | E10.S4 — carrier admin console |
| `/d365-sync` | `D365SyncStatusComponent` | E7.S6 — D365 sync reconciliation report |

## Running locally

> **Note:** this environment did not run `npm install`/`ng serve`, so the app has not been built
> here. `node`/`npm` are available; Angular CLI/build tooling needs installing first.

```bash
cd frontend
npm install
npm start   # ng serve, default http://localhost:4200
```

Configure `src/environments/environment.ts` with your Azure AD app registration (client ID,
tenant) and the backend API base URL before running — the checked-in file only contains
placeholders shared with the backend's `appsettings.json`.

## Auth

`AuthService` wraps `@azure/msal-browser` for Azure AD SSO (the same tenant D365 users
authenticate against — BR-064). `authInterceptor` attaches the resulting bearer token to every
API call so the backend's role-based policies (`LabOperations`, `ShippingSpecialist`,
`Compliance`, `Finance`, `PlatformAdmin`) apply consistently.
