# User Requirement Document (URD)
## GoLIMS Shipping Platform

| | |
|---|---|
| **Document** | User Requirement Document |
| **Project** | GoLIMS Shipping Platform |
| **Status** | Draft v1.0 |
| **Related documents** | System Overview Document, Business Requirements Document, Product Backlog, Test Cases |

---

## 1. Purpose

This document translates the business requirements into user-facing requirements expressed from the
perspective of the people who will use the GoLIMS Shipping Platform day to day, in the format
"As a [persona], I want [capability], so that [benefit]." It is the bridge between the BRD and the
Product Backlog.

## 2. User Personas

| Persona | Description | Primary Goals |
|---|---|---|
| **Lab Operations Coordinator** | Manages daily lab collection kit dispatch and receipt. | Fast, error-free kit dispatch; visibility into kit status. |
| **Shipping Specialist** | Creates and manages outbound/inbound shipments across carriers. | Accurate rating, label generation, and tracking with minimal manual rework. |
| **Compliance Officer** | Ensures shipments meet EU/USA/Japan biological-material regulations. | Auditable, enforced compliance rules; documentation on demand. |
| **Customer Service Representative** | Answers customer questions about shipment/kit status. | Real-time, accurate tracking and status information. |
| **Finance/ERP Administrator** | Manages billing and reconciliation between shipping and D365. | Accurate, timely financial sync with no manual journal entries. |
| **IT/Platform Administrator** | Configures carriers, rules, roles, and monitors system health. | Simple carrier onboarding; visibility into integration health. |
| **Courier/Carrier Ops Partner** (indirect) | External carrier systems consuming/providing tracking and EDI data. | Reliable, well-defined API/EDI contracts. |

## 3. User Requirements

### 3.1 Shipment Creation & Management

| ID | User Story | Priority |
|---|---|---|
| UR-001 | As a Shipping Specialist, I want to create a domestic or international shipment from an existing GoLIMS/D365 order, so that I don't have to re-key order details. | Must |
| UR-002 | As a Shipping Specialist, I want to consolidate multiple shipments into a single load, so that I can optimize carrier cost and pickup scheduling. | Should |
| UR-003 | As a Shipping Specialist, I want to see the full status lifecycle of a shipment in one screen, so that I can quickly answer status questions. | Must |
| UR-004 | As a Lab Operations Coordinator, I want to dispatch a kit even when only some components are ready, so that partial fulfillment doesn't block the whole shipment. | Must |

### 3.2 Carrier & Rating

| ID | User Story | Priority |
|---|---|---|
| UR-010 | As a Shipping Specialist, I want to get live rates from DHL, UPS, and regional carriers side by side, so that I can select the most cost-effective option. | Must |
| UR-011 | As an IT/Platform Administrator, I want to onboard a new regional carrier through configuration rather than code changes, so that expansion is fast and low-risk. | Should |
| UR-012 | As a Shipping Specialist, I want automatic fallback to an alternate carrier if the primary carrier's API is unavailable, so that shipments aren't blocked. | Could |

### 3.3 Label Management

| ID | User Story | Priority |
|---|---|---|
| UR-020 | As a Lab Operations Coordinator, I want to print sample labels, kit labels, and hazard labels for a single dispatch in one action, so that I don't miss a required label. | Must |
| UR-021 | As a Shipping Specialist, I want to reprint a lost or damaged label with a full audit entry, so that compliance is maintained. | Should |
| UR-022 | As an IT/Platform Administrator, I want to configure label templates without redeploying the application, so that label formats can evolve independently. | Should |
| UR-023 | As a Lab Operations Coordinator, I want labels routed automatically to the correct lab/warehouse printer, so that I don't have to manually select a printer each time. | Should |

### 3.4 Kit & Collection Logistics

| ID | User Story | Priority |
|---|---|---|
| UR-030 | As a Lab Operations Coordinator, I want to define a collection kit as a set of components, so that the system can validate a kit is complete before dispatch. | Must |
| UR-031 | As a Lab Operations Coordinator, I want to see consumable inventory levels for kit assembly, so that I can flag shortages before they block dispatch. | Should |
| UR-032 | As a Compliance Officer, I want to view the full chain-of-custody trail for any biological sample, so that I can respond to audits. | Must |
| UR-033 | As a Customer Service Representative, I want to see the kit lifecycle status (dispatched, collected, returned, received), so that I can inform customers accurately. | Must |

### 3.5 Business Rules & Compliance

| ID | User Story | Priority |
|---|---|---|
| UR-040 | As a Compliance Officer, I want shipping rules to enforce biological-material handling requirements automatically, so that non-compliant shipments are blocked or flagged. | Must |
| UR-041 | As an IT/Platform Administrator, I want to update a business rule (e.g., VIP handling) without a full application release, so that operational changes are fast. | Should |
| UR-042 | As a Compliance Officer, I want region-specific rules (EU/USA/Japan) to apply automatically based on origin/destination, so that I don't need to manually check regulations per shipment. | Must |

### 3.6 D365 Integration & Finance

| ID | User Story | Priority |
|---|---|---|
| UR-050 | As a Finance/ERP Administrator, I want completed shipment costs to post automatically to D365 billing, so that no manual journal entry is required. | Must |
| UR-051 | As a Finance/ERP Administrator, I want to see a reconciliation report of shipments synced vs. pending vs. failed against D365, so that I can resolve discrepancies quickly. | Should |
| UR-052 | As a Shipping Specialist, I want to keep creating and dispatching shipments even if D365 is temporarily unavailable, so that lab operations are never blocked by ERP downtime. | Must |

### 3.7 Monitoring & Administration

| ID | User Story | Priority |
|---|---|---|
| UR-060 | As an IT/Platform Administrator, I want a dashboard of shipment volumes, exceptions, and SLA breaches, so that I can react to operational issues quickly. | Must |
| UR-061 | As an IT/Platform Administrator, I want alerts when D365 integration sync fails repeatedly, so that I can intervene before it impacts billing. | Should |
| UR-062 | As an IT/Platform Administrator, I want role-based access using the same Azure AD identity as D365, so that users don't need separate credentials. | Must |

## 4. Non-Functional User Requirements

| ID | Requirement | Priority |
|---|---|---|
| NFR-001 | The web application shall load core screens (dashboard, shipment list) within 2 seconds under normal load. | Must |
| NFR-002 | The platform shall be available 99.9% of the time independent of D365 availability. | Must |
| NFR-003 | The platform shall support role-based access control aligned with least-privilege principles. | Must |
| NFR-004 | The platform UI shall meet WCAG 2.1 AA accessibility guidelines. | Should |
| NFR-005 | The platform shall support responsive layouts for desktop and tablet use in lab/warehouse environments. | Should |
| NFR-006 | All chain-of-custody and label reprint actions shall be fully audit-logged with user, timestamp, and reason. | Must |
| NFR-007 | The platform shall encrypt data in transit (TLS) and at rest (SQL Server TDE / Azure encryption). | Must |

## 5. Acceptance Criteria Summary

Each user requirement above maps to one or more acceptance criteria detailed in the Product Backlog
(per user story) and verified in the Test Cases document. A requirement is considered accepted when:

1. The corresponding backlog item(s) are implemented and pass their defined acceptance criteria.
2. Associated test cases (functional, integration, and — where applicable — compliance) pass.
3. The capability is demonstrably usable by the target persona without workaround or manual data
   re-entry between GoLIMS Shipping Platform and D365.
