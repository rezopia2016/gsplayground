# Product Backlog
## GoLIMS Shipping Platform

| | |
|---|---|
| **Document** | Product Backlog |
| **Project** | GoLIMS Shipping Platform |
| **Status** | Draft v1.0 |
| **Related documents** | System Overview Document, Business Requirements Document, User Requirement Document, Test Cases |

---

## 1. Backlog Structure

The backlog is organized as **Epics → Features → User Stories**, each story sized in story points
(Fibonacci) and prioritized using MoSCoW. Traceability back to BRD/URD IDs is included so the backlog
never drifts from the agreed requirements.

## 2. Release Roadmap

| Phase | Theme | Epics Included |
|---|---|---|
| **Phase 1 — MVP** | Core shipment orchestration + D365 sync foundation | E1, E7 (core), E10 (core) |
| **Phase 2 — Lab Domain** | Kit logistics, label management, rules engine | E2, E3, E4, E5 |
| **Phase 3 — Compliance & Scale** | Compliance extensions, monitoring, additional carriers | E6, E8, E9, remaining E7/E2 |

## 3. Epics

| Epic | Title | BRD/URD Traceability |
|---|---|---|
| E1 | Shipment Orchestration Engine | BR-001–004, UR-001–004 |
| E2 | Carrier Integration Layer | BR-010–014, UR-010–012 |
| E3 | Label Management Service | BR-020–024, UR-020–023 |
| E4 | Kit & Collection Logistics | BR-030–034, UR-030–033 |
| E5 | Rules Engine | BR-040–042, UR-040–042 |
| E6 | Compliance & Regulatory Extensions | BR-050–053, UR-040/042 |
| E7 | D365 Integration Layer | BR-060–064, UR-050–052 |
| E8 | Monitoring & Operational Dashboard | BR-070–071, UR-060–061 |
| E9 | Document Management | System Overview §5.7 |
| E10 | Security, Identity & Administration | NFR-003, NFR-006–007, UR-062 |

---

## 4. Epic E1 — Shipment Orchestration Engine

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E1.S1 — Create shipment from order | Must | 5 | Shipment created from D365/GoLIMS order without re-keying customer/address data. |
| E1.S2 — Manage shipment status lifecycle | Must | 5 | Status transitions (Created → Ready → Dispatched → In-Transit → Delivered/Exception) tracked with timestamps. |
| E1.S3 — Cancel/modify shipment prior to dispatch | Must | 3 | User can cancel/edit shipment before carrier manifest; audit entry recorded. |
| E1.S4 — Shipment consolidation into loads | Should | 8 | Multiple shipments grouped into one load with combined manifest. |
| E1.S5 — Partial kit fulfillment support | Must | 8 | Shipment can dispatch with a subset of kit components; remainder tracked as pending. |
| E1.S6 — Shipment list & search UI | Must | 5 | Angular shipment list with filter/search by status, customer, date, carrier. |

## 5. Epic E2 — Carrier Integration Layer

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E2.S1 — Carrier abstraction interface (`ICarrierGateway`) | Must | 5 | Common interface for rate/label/track/void across all carriers. |
| E2.S2 — DHL adapter | Must | 8 | Rate, label, and tracking calls succeed against DHL sandbox. |
| E2.S3 — UPS adapter | Must | 8 | Rate, label, and tracking calls succeed against UPS sandbox. |
| E2.S4 — Nitsu (regional) adapter | Must | 8 | Custom adapter supports rate/label/tracking per Nitsu API/EDI spec. |
| E2.S5 — Carrier onboarding via configuration | Should | 5 | New carrier registered via config/plug-in without core code change. |
| E2.S6 — Carrier fallback/failover | Could | 5 | Automatic fallback to secondary carrier on primary API failure. |
| E2.S7 — Carrier EDI processing | Should | 8 | Inbound/outbound EDI messages processed via Azure Integration Services. |

## 6. Epic E3 — Label Management Service

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E3.S1 — Label template engine | Must | 8 | Templates define label content/layout per label type, data-bound to shipment/kit/sample. |
| E3.S2 — Generate customer/internal/sample/kit/hazard/return labels | Must | 8 | All six label types generate correctly for a sample shipment. |
| E3.S3 — Multi-format rendering (ZPL, PDF, QR/1D/2D barcodes) | Must | 8 | Same label data renders correctly in ZPL and PDF with valid, scannable barcodes. |
| E3.S4 — Label reprint with audit trail | Should | 3 | Reprint recorded with user, timestamp, reason; original label remains in history. |
| E3.S5 — Print routing to lab/warehouse printers | Should | 5 | Label routed automatically to configured printer based on location/kit type. |
| E3.S6 — Template configuration UI | Could | 5 | Admin can edit label templates without redeployment. |

## 7. Epic E4 — Kit & Collection Logistics

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E4.S1 — Kit definition (multi-component) | Must | 5 | Kit modeled as a set of components with type/quantity. |
| E4.S2 — Kit assembly & packaging validation | Must | 8 | Dispatch blocked if required components missing/invalid. |
| E4.S3 — Consumable inventory tracking | Should | 5 | Consumable stock decremented on kit assembly; low-stock flagged. |
| E4.S4 — Chain-of-custody event recording | Must | 8 | Every custody-relevant event (dispatch, pickup, receipt, transfer) recorded immutably. |
| E4.S5 — Kit lifecycle status tracking | Must | 5 | Kit status (Created, Dispatched, Collected, In-Transit, Returned, Received) visible end-to-end. |
| E4.S6 — Kit lifecycle UI (Lab Ops view) | Must | 5 | Lab Ops Coordinator can view/manage kit lifecycle from Angular UI. |

## 8. Epic E5 — Rules Engine

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E5.S1 — Rule definition model & evaluation engine | Must | 8 | Rules stored as data/config and evaluated against shipment/kit context. |
| E5.S2 — Customer type / OEM / VIP handling rules | Must | 5 | Correct routing/handling applied based on customer classification. |
| E5.S3 — Biological material handling rules | Must | 5 | Shipments with biological material trigger required compliance steps. |
| E5.S4 — Pickup-schedule-based routing rules | Should | 5 | Pickup windows influence carrier/route selection. |
| E5.S5 — Rule change without redeploy | Should | 5 | Rule updated via admin UI/config takes effect without app restart. |

## 9. Epic E6 — Compliance & Regulatory Extensions

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E6.S1 — EU biological/diagnostic shipment compliance rules | Must | 8 | EU-bound biological shipments enforce required documentation/handling. |
| E6.S2 — USA laboratory shipment compliance rules | Must | 8 | USA-bound shipments enforce applicable lab regulations. |
| E6.S3 — Japan courier/localization compliance rules | Must | 8 | Japan-bound shipments apply courier-specific and localization rules. |
| E6.S4 — Compliance document generation & D365 attachment | Should | 5 | Generated compliance docs linked to corresponding D365 shipment/customs record. |

## 10. Epic E7 — D365 Integration Layer

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E7.S1 — D365 OAuth2 client-credentials auth client | Must | 5 | Platform authenticates to D365 OData/Dataverse API using Azure AD app registration. |
| E7.S2 — Order/customer/address data sync (inbound) | Must | 8 | Orders and master data pulled from D365 and mapped to platform domain model. |
| E7.S3 — Billing/financial event sync (outbound) | Must | 8 | Completed shipment costs posted to D365 billing automatically. |
| E7.S4 — Inventory event sync | Should | 5 | Kit consumption/returns reflected in D365 inventory. |
| E7.S5 — Outbox pattern & resilience (D365 downtime) | Must | 8 | Sync operations queued and retried; shipping unaffected by D365 outage. |
| E7.S6 — Reconciliation reporting | Should | 5 | Report shows synced/pending/failed records with drill-down. |

## 11. Epic E8 — Monitoring & Operational Dashboard

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E8.S1 — Shipment volume & exception dashboard | Must | 5 | Dashboard shows volumes, exceptions, SLA breaches by carrier/region. |
| E8.S2 — D365 sync health monitoring | Should | 5 | Dashboard/alerts show sync failures and retry status. |
| E8.S3 — Azure Monitor/App Insights integration | Should | 5 | End-to-end tracing correlated across GoLIMS and D365 calls. |

## 12. Epic E9 — Document Management

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E9.S1 — Lab-specific document generation (certificates, sample paperwork) | Should | 5 | Documents generated from shipment/kit/sample metadata. |
| E9.S2 — Document storage & retrieval | Should | 3 | Documents retrievable by shipment/kit ID with access control. |

## 13. Epic E10 — Security, Identity & Administration

| Story | Priority | Points | Acceptance Criteria (summary) |
|---|---|---|---|
| E10.S1 — Azure AD SSO integration | Must | 5 | Users authenticate via the same Azure AD tenant as D365. |
| E10.S2 — Role-based access control | Must | 5 | Roles (Lab Ops, Shipping Specialist, Compliance, Finance, Admin) restrict UI/API access appropriately. |
| E10.S3 — Audit logging (custody, reprint, rule changes) | Must | 5 | All sensitive actions logged with user/timestamp/reason and queryable. |
| E10.S4 — Carrier/rule/template admin console | Should | 8 | Admin UI to manage carriers, rules, and label templates. |

## 14. Backlog Summary

| Phase | Total Stories | Total Points (approx.) |
|---|---|---|
| Phase 1 — MVP (E1, E7 core, E10 core) | 12 | ~64 |
| Phase 2 — Lab Domain (E2, E3, E4, E5) | 24 | ~140 |
| Phase 3 — Compliance & Scale (E6, E8, E9, remaining) | 12 | ~65 |

*Story points are directional estimates for planning purposes and should be re-estimated by the
delivery team during sprint planning.*
