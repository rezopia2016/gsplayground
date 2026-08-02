# System Overview Document
## GoLIMS Shipping Platform — Hybrid Architecture with Microsoft Dynamics 365

| | |
|---|---|
| **Document** | System Overview Document (SOD) |
| **Project** | GoLIMS Shipping Platform / D365 Hybrid Integration |
| **Audience** | Enterprise Architecture, ERP Program, Laboratory Operations, IT Leadership |
| **Status** | Draft v1.0 |
| **Related documents** | Business Requirements Document, User Requirements Document, Product Backlog, Test Cases |

---

## 1. Purpose and Scope

This document defines the target system architecture for Eurofins' genomics/laboratory shipping
operations, evaluates the native shipping capabilities of Microsoft Dynamics 365 (D365) Finance &
Supply Chain Management, and justifies a **hybrid architecture** in which D365 remains the system of
record for ERP/financial processes while a purpose-built **GoLIMS Shipping Platform** (Angular +
.NET Core + SQL Server) owns shipping orchestration, carrier integration, label management, kit
logistics, and lab-specific compliance.

The central question this document answers is not *"can D365 ship a package?"* — it clearly can for
standard commercial logistics — but **whether D365 can be the system of record for a global,
temperature-sensitive, chain-of-custody-driven genomics shipping operation without deep, brittle
customization.** The assessment concludes it cannot, and that a hybrid model minimizes ERP
customization risk while preserving the ability to evolve lab-specific capabilities independently.

## 2. Executive Summary

D365 is an excellent ERP, warehousing, transportation, and financial platform for **standard
enterprise logistics**. It is **not** optimized to serve as the primary shipping platform for a
genomics organization with specialized laboratory workflows (collection kits, biological samples,
chain of custody, temperature control, dynamic multi-format labeling, and non-standard regional
carriers).

**Recommendation:** Adopt a hybrid architecture — D365 for financials, inventory, and standard
transportation; the GoLIMS Shipping Platform as the shipping orchestration layer — integrated
through a well-defined API and data-contract boundary.

## 3. D365 Native Feature Assessment

The table below reflects the capability assessment used to drive this architecture decision.

| Capability | D365 Support | Feasibility | Remarks |
|---|---|---|---|
| Shipment Management | ✅ Native | High | Mature outbound shipment, load and transportation management. |
| Domestic Shipment | ✅ Native | High | Fully supported. |
| International Shipment | ✅ Native | High | Supports export/import, trade, customs, landed cost. |
| Carrier Management | ✅ Native | High | Standard carrier master and transportation framework. |
| Carrier Service Management | ⚠️ Partial | Medium | Standard services supported; lab-specific services require extensions. |
| Package Management | ⚠️ Partial | Medium | Supports containers/license plates but lacks sophisticated parcel management. |
| Shipment Automation | ⚠️ Partial | Medium-High | Workflows available; complex business rules require Power Platform/customization. |
| Shipment Consolidation | ✅ Native | High | Load planning and shipment consolidation supported. |
| Customs Management | ✅ Native | High | Strong international trade capabilities. |
| EU Compliance | ⚠️ Partial | Medium-High | VAT and customs supported; biological shipment regulations require customization. |
| USA Compliance | ⚠️ Partial | Medium | Commercial compliance supported; laboratory regulations require additional development. |
| Japan Compliance | ⚠️ Partial | Medium | Localization available; courier-specific requirements require extensions. |
| Label Management | ⚠️ Limited | Medium-Low | Basic labels only; dynamic laboratory labels require a custom solution. |
| Label Printing | ⚠️ Limited | Medium-Low | Primarily ZPL; enterprise print routing needs middleware. |
| Label Reprint | ⚠️ Partial | Medium | Possible through customization. |
| Barcode & Tracking | ✅ Native | High | Warehouse barcode and tracking supported. |
| Carrier Integration | ⚠️ Partial | Medium | Standard connectors limited; custom APIs required for many carriers. |
| Authentication & Security | ✅ Native | High | Azure AD, RBAC, audit trail supported. |
| Data Contract Management | ❌ Custom | Low | No dedicated contract management; requires custom implementation. |
| EDI Processing | ⚠️ Partial | Medium | Requires Azure Integration Services or third-party middleware. |
| Return & Reverse Logistics | ✅ Native | High | Standard RMA and return orders supported. |
| Kit & Collection Logistics | ❌ Custom | Low | Not designed for laboratory collection kits; requires significant customization. |
| Order Integration | ✅ Native | High | Standard APIs available; integration with GoLIMS/LIMS required. |
| Address Management | ✅ Native | High | Strong customer/vendor address management. |
| Packaging Integration | ⚠️ Partial | Medium | Packaging exists but advanced packaging intelligence requires customization. |
| Billing Integration | ✅ Native | Very High | One of D365's strongest capabilities. |
| Monitoring & Observability | ⚠️ Partial | Medium | Azure Monitor/App Insights available but no operational shipping dashboard. |

### 3.1 Overall Ratings

| Area | Rating |
|---|---|
| Standard Enterprise Shipping | ⭐⭐⭐⭐⭐ |
| Warehouse Operations | ⭐⭐⭐⭐⭐ |
| Transportation Management | ⭐⭐⭐⭐⭐ |
| Financial Integration | ⭐⭐⭐⭐⭐ |
| International Trade | ⭐⭐⭐⭐☆ |
| Carrier Integration | ⭐⭐⭐☆☆ |
| Label Management | ⭐⭐☆☆☆ |
| Laboratory Shipping | ⭐⭐☆☆☆ |
| Robotics Integration | ⭐☆☆☆☆ |
| Kit Logistics | ⭐☆☆☆☆ |
| Extensibility | ⭐⭐⭐☆☆ |

## 4. Why D365 Is Strong: Pros of Native Use in a Hybrid Architecture

D365 should be retained and leveraged for the domains where it is genuinely best-in-class:

1. **Financial & Billing Integration (⭐⭐⭐⭐⭐, "Very High" feasibility).** Invoicing, landed cost,
   revenue recognition, and multi-entity financial consolidation are core D365 strengths. Rebuilding
   this outside the ERP would be costly and would fragment the general ledger.
2. **Warehouse & Transportation Management.** Load planning, shipment consolidation, dock scheduling,
   and standard carrier master data are mature, tested, and already embedded in Eurofins' operational
   processes.
3. **International Trade & Customs.** Export/import documentation, trade compliance, and landed-cost
   calculation are deep D365 modules that would be extremely expensive to replicate.
4. **Security & Identity.** Azure AD-based authentication, role-based access control, and audit
   trails are enterprise-grade and directly reusable by any system integrated into the Microsoft
   ecosystem — including the GoLIMS Shipping Platform itself.
5. **Order & Address Master Data.** Customer/vendor address management and order integration APIs
   are reliable sources of truth that the shipping platform should consume rather than duplicate.
6. **Reduced ERP customization risk.** Keeping D365 focused on what it does natively avoids the
   long-term maintenance burden, upgrade friction, and support-model complications that come from
   deeply customizing an ERP's core transactional modules.

**Conclusion of this section:** the pros of the hybrid model are realized by *not* fighting D365's
design center. Financials, inventory, and standard transportation stay in D365, integrated — not
replaced.

## 5. Why Partial or Missing Capabilities Are Better Built as a Custom Application

For every capability rated "Partial," "Limited," or "Custom" above, extending D365 directly carries
disproportionate cost and risk relative to building the capability in a purpose-fit service. The
rationale, by gap area:

### 5.1 Laboratory Workflow (⭐⭐☆☆☆)
D365 is modeled around commercial products moving through a supply chain, not around DNA kits,
collection kits, biological samples, temperature-controlled chain of custody, or partial kit
fulfillment. Modeling these concepts inside D365's data model would require deep schema extensions
to core SCM tables, high upgrade risk, and would still not match the domain naturally. A custom
domain model (in GoLIMS) can represent kits, components, and custody events as first-class entities.

### 5.2 Label Management (⭐⭐☆☆☆)
D365 provides only basic labels, primarily ZPL, with no native concept of the many label types a
genomics operation needs: customer labels, internal labels, sample labels, collection-kit labels,
hazard labels, return labels, multiple barcode symbologies, and dynamic templates driven by
sample/kit metadata. A dedicated **Label Management Service** with template-driven rendering
(PDF/ZPL/QR) and print routing middleware is architecturally cleaner and avoids overloading D365's
print management framework.

### 5.3 Carrier Integration (⭐⭐⭐☆☆)
Major global carriers (DHL, UPS) have connectors, but regional/specialized carriers (e.g., Nitsu) do
not, and D365 has no universal carrier abstraction layer. A **Carrier Service abstraction** in the
custom platform normalizes rate/label/tracking/EDI operations behind one interface, so onboarding a
new regional carrier is a plug-in, not an ERP customization.

### 5.4 Business Rules (Shipment Automation, ⭐⭐⭐)
Shipping decisions driven by customer type, OEM, VIP handling, biological-material classification, or
pickup schedules are highly dynamic and specific to lab operations. Implementing this logic as
Power Platform/X++ customization inside D365 couples volatile business rules to the ERP's release
cycle. A dedicated **Rules Engine** in GoLIMS keeps this logic independently versioned, testable, and
changeable without an ERP change window.

### 5.5 Packaging & Kit Logistics (⭐☆☆☆☆)
D365 has no native concept of collection kits, sample return kits, multi-component kit assembly,
consumable tracking, packaging validation, or kit lifecycle. This is the largest capability gap and
the strongest argument for a custom **Kit & Collection Logistics** module owned outside the ERP.

### 5.6 Compliance Extensions (EU/USA/Japan, ⭐⭐⭐-)
Commercial trade compliance is well covered, but biological-shipment-specific regulatory
requirements (e.g., UN3373/dangerous goods handling for diagnostic specimens, courier-specific
regional rules) require extensions best implemented as configurable compliance rules in the custom
platform, which can then write compliant records back to D365 for financial/customs purposes.

### 5.7 Document Management
Generating laboratory-specific documentation (certificates, sample paperwork, customs attachments
tied to biological content) goes beyond D365's standard document handling and is better served by a
document-generation service co-located with the shipment/kit domain data.

### 5.8 Data Contract Management & EDI (❌ Custom / ⚠️ Partial)
D365 has no dedicated contract management for carrier/partner data contracts, and EDI requires Azure
Integration Services or third-party middleware regardless of where it is hosted — so there is no
native-D365 advantage to be preserved by keeping this in the ERP. Housing it in the custom
integration layer keeps carrier EDI formats and versioning decoupled from ERP release cycles.

### 5.9 Monitoring & Observability (⚠️ Partial)
D365 exposes Azure Monitor/App Insights telemetry but has no operational shipping dashboard. An
operational, shipment-centric dashboard is a natural feature of the custom platform's UI, built on
the same Azure observability stack for consistency.

## 6. Target Hybrid Architecture

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

### 6.1 Component Responsibility Matrix

| Area | System of Record | Rationale |
|---|---|---|
| ERP & Billing | **D365** | Native strength; financial consolidation must stay centralized. |
| Warehouse Inventory | **D365** | Mature WMS already adopted operationally. |
| Transportation & Loads | **D365** (where applicable) | Load planning/consolidation is native and reusable. |
| Shipping Orchestration | **GoLIMS Shipping Platform** | Requires lab-specific domain model D365 doesn't have. |
| Carrier Integration | **GoLIMS Shipping Platform** | Universal carrier abstraction; regional carrier support. |
| Label Management | **GoLIMS Shipping Platform** | Dynamic, multi-format, template-driven labeling. |
| Rules Engine | **GoLIMS Shipping Platform** | Independently versioned business logic. |
| Laboratory / Kit Logistics | **GoLIMS Shipping Platform** | No native D365 equivalent; core domain gap. |
| Compliance Extensions | **GoLIMS Shipping Platform**, synced to D365 | Keeps regulatory logic agile; financial/customs records still land in D365. |
| Monitoring & Dashboards | **GoLIMS Shipping Platform** + Azure observability | Operational shipping visibility D365 lacks natively. |

### 6.2 Technology Stack (Custom Platform)

| Layer | Technology | Notes |
|---|---|---|
| Frontend | Angular (standalone components, TypeScript) | SPA for lab ops, shipping specialists, tracking, admin |
| Backend API | .NET Core (ASP.NET Core Web API) | Clean architecture: Domain / Application / Infrastructure / Api |
| Database | SQL Server | System of record for shipments, kits, labels, custody events |
| D365 Integration | D365 Finance & SCM OData / Dataverse Web API, Azure AD client-credentials OAuth2 | Bi-directional sync: orders, inventory, billing, customs docs |
| Messaging/EDI | Azure Integration Services (Logic Apps / Service Bus) | Carrier EDI, async D365 sync, retry/dead-letter handling |
| Identity | Azure Active Directory | Shared identity provider across D365 and GoLIMS Shipping Platform |
| Observability | Azure Monitor / Application Insights | Correlated tracing across both systems |

### 6.3 Integration Approach

- **Order Integration:** D365 remains authoritative for customer/vendor master data and commercial
  order data; GoLIMS consumes these via D365's standard OData/Data Entities APIs and enriches them
  with lab-specific shipment/kit data.
- **Financial Sync:** Shipment completion, carrier costs, and billable events are posted back to
  D365 as financial transactions (leveraging D365's native, very-high-feasibility billing engine).
- **Customs & Compliance:** International shipment/customs documents generated for standard trade
  flow through D365's native customs management; lab-specific compliance artifacts are generated by
  GoLIMS and attached/linked to the D365 shipment record.
- **Identity:** Both systems trust the same Azure AD tenant; RBAC roles are mapped, not duplicated.
- **Resilience:** All D365 integration calls are wrapped with retry/circuit-breaker policies and a
  local `D365SyncLog` (outbox pattern) so shipping operations are never blocked by ERP downtime.

## 7. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Two systems drift out of sync (orders, inventory) | Outbox/idempotent sync jobs with reconciliation reporting |
| D365 API throttling/latency affects shipping SLAs | Async integration via Service Bus, local caching of reference data |
| Carrier API changes break integrations | Carrier abstraction layer isolates adapters; contract tests per carrier |
| Regulatory changes (EU/USA/Japan) | Compliance rules externalized and versioned in Rules Engine, not hardcoded |
| Scope creep back into D365 customization | Architecture governance: any "Partial/Custom" capability defaults to GoLIMS unless a documented exception is approved |

## 8. Conclusion

D365 is an excellent ERP and transportation platform for standard enterprise logistics, but it is not
optimized to serve as the primary shipping platform for a genomics organization with specialized
laboratory workflows. The hybrid architecture described here — D365 for financials, inventory, and
standard transportation; the GoLIMS Shipping Platform for shipping orchestration — provides the best
balance of functionality, flexibility, and long-term maintainability, while minimizing deep ERP
customization and preserving the ability to evolve lab-specific shipping capabilities
independently.
