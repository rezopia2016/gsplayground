# Database — GoLIMS Shipping Platform (SQL Server)

Scripts in `schema/` create the SQL Server system-of-record database for the shipping platform.
Run in order against a SQL Server instance (LocalDB, Azure SQL, or SQL Server on Azure VM):

```bash
sqlcmd -S <server> -i schema/001_init_schema.sql
sqlcmd -S <server> -i schema/002_seed_data.sql
sqlcmd -S <server> -i schema/003_custody_immutability.sql
```

## Tables

| Table | Purpose | Backlog Epic |
|---|---|---|
| `Shipments`, `ShipmentStatusHistories`, `Packages` | Shipment orchestration & status lifecycle | E1 |
| `Carriers`, `CarrierServices` | Carrier master data for the abstraction layer | E2 |
| `Labels`, `LabelPrintEvents` | Label management with reprint audit trail | E3 |
| `Kits`, `KitComponents`, `ChainOfCustodyEvents` | Kit & collection logistics, append-only custody trail | E4 |
| `RuleDefinitions` | Externalized business rules (no redeploy required) | E5 |
| `D365SyncRecords` | D365 integration outbox (resilience to ERP downtime) | E7 |

`003_custody_immutability.sql` adds `INSTEAD OF UPDATE/DELETE` triggers on
`ChainOfCustodyEvents` and `LabelPrintEvents` so audit history cannot be altered even by a direct
database client — corrections must be new, appended events.

This schema is authoritative; the EF Core model in
`backend/src/ShippingPlatform.Infrastructure/Persistence` is kept in sync with it. If the two
diverge, generate an EF Core migration (`dotnet ef migrations add ...`) rather than hand-editing
both independently.
