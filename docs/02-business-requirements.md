# Business Requirements Document (BRD)
## GoLIMS Shipping Platform — Hybrid D365 Shipping Architecture

| | |
|---|---|
| **Document** | Business Requirements Document |
| **Project** | GoLIMS Shipping Platform |
| **Status** | Draft v1.0 |
| **Related documents** | System Overview Document, User Requirement Document, Product Backlog, Test Cases |

---

## 1. Business Context

Eurofins operates a global genomics/laboratory shipping operation that moves DNA and collection
kits, biological samples, and temperature-controlled materials across the USA, EU, and Japan, with
strict chain-of-custody, regulatory, and carrier requirements. The current evaluation compared using
Microsoft Dynamics 365 (D365) as the shipping system of record against a hybrid model where D365
handles ERP/finance and a custom platform (GoLIMS Shipping Platform) handles shipping orchestration.

D365 was assessed as strong for standard enterprise shipping, warehouse operations, transportation
management, and — above all — financial integration, but weak (⭐⭐☆☆☆ or lower) in laboratory
shipping, label management, kit logistics, and robotics integration, the capabilities most central to
Eurofins' genomics operation.

## 2. Business Problem Statement

Eurofins cannot rely on D365 alone to run genomics shipping operations without significant, high-risk
customization of the ERP's core transactional modules. Continuing to force lab-specific shipping
workflows into D365 creates: (a) escalating customization and upgrade cost, (b) fragile compliance
handling for biological materials, (c) no viable path for kit/collection logistics, and (d) no
operational shipping visibility. Eurofins needs a shipping capability purpose-built for its domain
that still leverages D365 where D365 is genuinely strong.

## 3. Business Objectives

| ID | Objective |
|---|---|
| BO-1 | Establish a single, purpose-built shipping platform for genomics/lab shipment orchestration, decoupled from ERP release cycles. |
| BO-2 | Preserve D365 as the system of record for financials, billing, inventory, and standard transportation — avoid duplicating what D365 already does well. |
| BO-3 | Support dynamic, multi-format label generation (customer, internal, sample, kit, hazard, return labels) across ZPL/PDF/QR. |
| BO-4 | Provide a carrier abstraction layer supporting global (DHL, UPS) and regional/specialty carriers (e.g., Nitsu) without per-carrier ERP customization. |
| BO-5 | Model laboratory-specific domain concepts natively: collection kits, biological samples, partial kit fulfillment, chain of custody. |
| BO-6 | Externalize complex shipping business rules (customer type, OEM, VIP, biological material, pickup schedules) into a maintainable rules engine. |
| BO-7 | Meet regional regulatory requirements for biological/diagnostic shipments in the EU, USA, and Japan. |
| BO-8 | Provide operational monitoring and shipping dashboards not available natively in D365. |
| BO-9 | Integrate bi-directionally with D365 for orders, inventory, billing, and customs documentation with resilience to ERP downtime/latency. |
| BO-10 | Minimize deep D365 customization to reduce long-term maintenance cost and upgrade risk. |

## 4. Scope

### 4.1 In Scope
- New shipping web application: Angular frontend, .NET Core backend, SQL Server database ("GoLIMS Shipping Platform").
- Shipment orchestration engine (creation, consolidation, status lifecycle).
- Carrier integration layer (DHL, UPS, Nitsu, extensible to additional carriers).
- Label management service (multi-format, multi-type, template-driven, print routing).
- Kit & collection logistics module (kit definition, component tracking, partial fulfillment, chain of custody).
- Business rules engine (customer/OEM/VIP/biological-material/pickup-schedule driven).
- Compliance extensions for EU, USA, and Japan biological/diagnostic shipment regulations.
- Bi-directional integration with D365 Finance & Supply Chain Management (orders, inventory, billing, customs).
- Operational monitoring/dashboard leveraging Azure Monitor/Application Insights.
- Role-based access control integrated with Azure AD (shared with D365).

### 4.2 Out of Scope (for this phase)
- Replacing D365 financial, billing, inventory, or standard transportation modules.
- Robotics integration (flagged as a future phase; current D365 rating ⭐☆☆☆☆, requires separate feasibility study).
- Full EDI trading-partner onboarding for every possible regional carrier (initial phase covers DHL, UPS, Nitsu; framework supports later additions).
- Replacing D365 customs/trade compliance for standard commercial shipments.

## 5. Business Requirements

Requirements are grouped by capability area and prioritized using MoSCoW (Must/Should/Could/Won't).

### 5.1 Shipment Orchestration

| ID | Requirement | Priority |
|---|---|---|
| BR-001 | The platform shall allow creation, modification, and cancellation of shipments for domestic and international genomics shipments. | Must |
| BR-002 | The platform shall support shipment consolidation and load-level grouping consistent with D365's transportation planning where applicable. | Should |
| BR-003 | The platform shall maintain a full shipment status lifecycle independent of D365 downtime. | Must |
| BR-004 | The platform shall support partial shipment/kit fulfillment where not all kit components ship together. | Must |

### 5.2 Carrier Integration

| ID | Requirement | Priority |
|---|---|---|
| BR-010 | The platform shall provide a carrier abstraction layer decoupling shipment logic from carrier-specific APIs. | Must |
| BR-011 | The platform shall integrate with DHL and UPS for rating, label generation, and tracking. | Must |
| BR-012 | The platform shall integrate with regional/specialty carriers (e.g., Nitsu) via custom adapters. | Must |
| BR-013 | The platform shall support onboarding additional carriers without core platform changes (plug-in adapter model). | Should |
| BR-014 | The platform shall process carrier EDI messages via Azure Integration Services or equivalent middleware. | Should |

### 5.3 Label Management

| ID | Requirement | Priority |
|---|---|---|
| BR-020 | The platform shall generate customer, internal, sample, collection-kit, hazard, and return labels. | Must |
| BR-021 | The platform shall support multiple barcode/label formats (ZPL, PDF, QR, 1D/2D barcodes). | Must |
| BR-022 | The platform shall support dynamic, template-driven label content based on shipment/kit/sample metadata. | Must |
| BR-023 | The platform shall support label reprint with full audit trail. | Should |
| BR-024 | The platform shall support enterprise print routing to lab/warehouse printers. | Should |

### 5.4 Kit & Collection Logistics

| ID | Requirement | Priority |
|---|---|---|
| BR-030 | The platform shall model collection kits as multi-component assemblies with individual component tracking. | Must |
| BR-031 | The platform shall track consumable inventory associated with kits. | Should |
| BR-032 | The platform shall validate packaging completeness prior to kit dispatch. | Must |
| BR-033 | The platform shall record full chain-of-custody events for biological samples and kits. | Must |
| BR-034 | The platform shall support the full kit lifecycle: creation, dispatch, in-transit, collection, return, receipt. | Must |

### 5.5 Business Rules

| ID | Requirement | Priority |
|---|---|---|
| BR-040 | The platform shall provide a configurable rules engine to drive shipping decisions based on customer type, OEM, VIP status, and material classification. | Must |
| BR-041 | The platform shall allow rule changes without requiring a full application release. | Should |
| BR-042 | The platform shall apply pickup-schedule-based routing rules. | Should |

### 5.6 Compliance

| ID | Requirement | Priority |
|---|---|---|
| BR-050 | The platform shall support EU biological/diagnostic shipment regulatory requirements (in addition to D365's native VAT/customs handling). | Must |
| BR-051 | The platform shall support USA laboratory shipment regulatory requirements. | Must |
| BR-052 | The platform shall support Japan courier-specific and localization requirements. | Must |
| BR-053 | The platform shall generate and attach lab-specific compliance documentation to the corresponding D365 shipment/customs record. | Should |

### 5.7 D365 Integration

| ID | Requirement | Priority |
|---|---|---|
| BR-060 | The platform shall consume order and customer/vendor address master data from D365 via standard Data Entities/OData APIs. | Must |
| BR-061 | The platform shall post billable shipment events (freight cost, service charges) back to D365 billing. | Must |
| BR-062 | The platform shall synchronize inventory-relevant events (kit consumption, returns) with D365 inventory. | Should |
| BR-063 | The platform shall remain operational for core shipping functions during D365 unavailability, queuing sync operations for later delivery. | Must |
| BR-064 | The platform shall reuse D365's Azure AD identity for authentication and map roles to platform RBAC. | Must |

### 5.8 Monitoring & Operations

| ID | Requirement | Priority |
|---|---|---|
| BR-070 | The platform shall provide an operational shipping dashboard (volumes, exceptions, SLA breaches, carrier performance). | Must |
| BR-071 | The platform shall integrate with Azure Monitor/Application Insights for end-to-end observability. | Should |

## 6. Success Criteria / KPIs

| KPI | Target |
|---|---|
| Reduction in D365 core-module customizations required for shipping | ≥ 80% reduction vs. D365-only approach |
| Label generation coverage (label types supported) | 100% of identified lab label types |
| Carrier onboarding time for a new regional carrier | ≤ 4 weeks via adapter model |
| Shipment orchestration availability during D365 outage | 100% (async sync via outbox) |
| Chain-of-custody event completeness | 100% of biological shipments have full custody trail |
| Financial reconciliation accuracy (GoLIMS → D365) | ≥ 99.5% auto-reconciled without manual intervention |

## 7. Assumptions

- D365 Finance & Supply Chain Management is already deployed and used for ERP/financial processes.
- Azure AD is the shared identity provider across D365 and the new platform.
- Existing GoLIMS/LIMS systems can provide or receive order-level integration via APIs.
- Carrier accounts/contracts (DHL, UPS, Nitsu, etc.) are commercially in place; this project delivers technical integration only.

## 8. Constraints

- Must not require modification of D365 core transactional schema for financial/inventory modules.
- Must operate within existing Azure landing zone security and networking policies.
- Regulatory requirements (EU/USA/Japan) must be independently configurable without a full release cycle.

## 9. Dependencies

- D365 Data Entities/OData/Dataverse Web API availability and stable schema for orders, customers, inventory, billing.
- Azure Integration Services (Logic Apps/Service Bus) availability for EDI and async sync.
- Carrier API/EDI credentials and sandbox environments for DHL, UPS, Nitsu.
- Azure AD tenant configuration for shared identity.

## 10. Risks

See System Overview Document §7 for the consolidated risk register; business-level risks are
carried forward as BRD acceptance conditions and tracked in the Product Backlog as non-functional
epics.
