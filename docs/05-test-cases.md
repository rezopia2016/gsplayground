# Test Cases Document
## GoLIMS Shipping Platform

| | |
|---|---|
| **Document** | Test Cases |
| **Project** | GoLIMS Shipping Platform |
| **Status** | Draft v1.0 |
| **Related documents** | System Overview Document, Business Requirements Document, User Requirement Document, Product Backlog |

---

## 1. Test Strategy Summary

| Test Level | Approach |
|---|---|
| Unit | xUnit (.NET Core services, rules engine, mappers) and Jasmine/Karma (Angular components/services). |
| Integration | API-level tests against SQL Server (test containers) and D365/carrier sandbox environments. |
| Contract | Per-carrier adapter contract tests validating rate/label/tracking responses against expected schema. |
| End-to-End | Playwright/Cypress against Angular UI covering golden paths per persona. |
| Compliance | Scenario-based tests validating EU/USA/Japan rule enforcement for biological shipments. |
| Non-Functional | Load testing (shipment creation throughput, D365 API latency/outage simulation), accessibility (WCAG 2.1 AA) audits, security testing (authz, audit logging). |
| Resilience | Chaos/failure-injection tests for D365 unavailability and carrier API failure, validating outbox/retry behavior. |

Priorities: **P1** blocking/critical, **P2** high, **P3** medium/edge case.

---

## 2. Shipment Orchestration (Epic E1)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E1-01 | Create shipment from existing order | Order exists in D365/GoLIMS | 1. Open "New Shipment" 2. Select order 3. Confirm | Shipment created with order/customer/address data pre-populated, no manual re-entry | P1 |
| TC-E1-02 | Shipment status lifecycle transitions | Shipment in "Created" status | Progress shipment through Ready → Dispatched → In-Transit → Delivered | Each transition recorded with timestamp; UI reflects current status in real time | P1 |
| TC-E1-03 | Cancel shipment before dispatch | Shipment in "Created"/"Ready" status | Cancel shipment | Shipment marked Cancelled; audit entry created; no carrier manifest generated | P1 |
| TC-E1-04 | Attempt to cancel dispatched shipment | Shipment in "Dispatched" status | Attempt cancel | System blocks cancellation with explanatory error | P2 |
| TC-E1-05 | Consolidate multiple shipments into a load | 2+ shipments ready for same carrier/route | Select shipments, consolidate | Single load/manifest created referencing all shipments | P2 |
| TC-E1-06 | Partial kit fulfillment dispatch | Kit with 3 components, only 2 ready | Dispatch shipment with available components | Shipment dispatches with 2 components; 3rd tracked as pending fulfillment | P1 |
| TC-E1-07 | Shipment list filter/search | Multiple shipments exist with varying status/carrier/date | Apply filters | List returns correctly filtered/sorted results | P2 |

## 3. Carrier Integration (Epic E2)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E2-01 | Get DHL rate quote | DHL sandbox credentials configured | Request rate for sample shipment | Valid rate response mapped to platform model | P1 |
| TC-E2-02 | Get UPS rate quote | UPS sandbox credentials configured | Request rate for sample shipment | Valid rate response mapped to platform model | P1 |
| TC-E2-03 | Generate Nitsu label | Nitsu adapter configured | Request label for sample shipment | Valid label returned in carrier-required format | P1 |
| TC-E2-04 | Carrier abstraction consistency | Two or more carriers configured | Call rate/label/track for each via common interface | All carriers return normalized response shape; no carrier-specific code in calling layer | P1 |
| TC-E2-05 | Onboard new carrier via configuration | New carrier adapter registered | Configure carrier, request rate | Rate call succeeds without core code deployment | P2 |
| TC-E2-06 | Carrier API failure fallback | Primary carrier API simulated as down | Request rate/label | System automatically falls back to secondary carrier (if configured) or surfaces clear error | P3 |
| TC-E2-07 | Inbound carrier tracking EDI update | EDI tracking message received via Azure Integration Services | Process message | Shipment status updated to match EDI event | P2 |

## 4. Label Management (Epic E3)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E3-01 | Generate all required label types for a kit dispatch | Kit ready for dispatch | Trigger label generation | Customer, internal, sample, kit, hazard (if applicable), and return labels all generated | P1 |
| TC-E3-02 | ZPL label renders correctly | Label template configured | Generate label in ZPL format | Valid ZPL output parses and prints correctly on test printer/emulator | P1 |
| TC-E3-03 | PDF label renders correctly | Label template configured | Generate label in PDF format | Valid, human-readable PDF with correct data fields | P1 |
| TC-E3-04 | Barcode/QR scan validation | Label generated with 1D/2D barcode or QR | Scan barcode with test scanner | Correct shipment/sample/kit ID decoded | P1 |
| TC-E3-05 | Label reprint audit trail | Label previously printed | Reprint label with reason code | New print event logged with user/timestamp/reason; original print history retained | P2 |
| TC-E3-06 | Dynamic template data binding | Shipment/kit metadata varies (e.g., hazard flag true/false) | Generate label | Hazard label included only when hazard flag is true; content reflects current metadata | P1 |
| TC-E3-07 | Print routing to correct printer | Printer mapped to lab/location/kit type | Generate label | Label routed to correct configured printer | P2 |

## 5. Kit & Collection Logistics (Epic E4)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E4-01 | Define multi-component kit | N/A | Create kit definition with 3 component types | Kit definition saved with correct component list/quantities | P1 |
| TC-E4-02 | Packaging validation blocks incomplete kit | Kit missing a required component | Attempt dispatch | Dispatch blocked with validation error identifying missing component | P1 |
| TC-E4-03 | Consumable inventory decremented on assembly | Consumable stock = 10 | Assemble kit consuming 1 unit | Stock reduces to 9; low-stock alert triggers if below threshold | P2 |
| TC-E4-04 | Chain-of-custody event recorded on dispatch | Kit ready for dispatch | Dispatch kit | Custody event created (actor, timestamp, location, action) immutably | P1 |
| TC-E4-05 | Chain-of-custody event recorded on receipt | Kit in transit, arrives at destination lab | Record receipt | Custody event appended; full custody chain viewable end-to-end | P1 |
| TC-E4-06 | Kit lifecycle status visibility | Kit dispatched | View kit in Angular UI | Status (Dispatched/Collected/In-Transit/Returned/Received) shown accurately | P1 |
| TC-E4-07 | Attempt to tamper with custody record | Custody event exists | Attempt to edit/delete custody event via API | System rejects modification; custody records are append-only | P1 |

## 6. Rules Engine (Epic E5)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E5-01 | VIP customer routing rule | Rule configured for VIP customers | Create shipment for VIP customer | Expedited routing/handling automatically applied | P1 |
| TC-E5-02 | Biological material handling rule enforcement | Shipment flagged as containing biological material | Create shipment | Required compliance steps (packaging, labels, documentation) automatically enforced | P1 |
| TC-E5-03 | OEM-specific handling rule | Rule configured for OEM customer type | Create shipment for OEM customer | OEM-specific handling rule applied correctly | P2 |
| TC-E5-04 | Pickup-schedule-based routing | Pickup window rule configured | Create shipment within/outside window | Carrier/route selection reflects pickup-window logic | P2 |
| TC-E5-05 | Rule update takes effect without redeploy | Existing rule active | Update rule via admin UI/config | New shipments immediately reflect updated rule; no app restart required | P2 |
| TC-E5-06 | Conflicting rule resolution | Two rules could both apply to a shipment | Create shipment matching both rule conditions | Rule precedence resolves deterministically per defined priority order | P3 |

## 7. Compliance (Epic E6)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E6-01 | EU biological shipment compliance enforcement | Shipment destined for EU, biological material flagged | Create shipment | Required EU biological/diagnostic compliance steps enforced (documentation, labeling) | P1 |
| TC-E6-02 | USA laboratory shipment compliance enforcement | Shipment destined for USA, lab material flagged | Create shipment | Required USA lab compliance steps enforced | P1 |
| TC-E6-03 | Japan courier/localization compliance enforcement | Shipment destined for Japan | Create shipment | Japan-specific courier/localization rules applied (labels, documentation, language) | P1 |
| TC-E6-04 | Compliance document generated and linked to D365 | Shipment complete | Generate compliance document | Document generated and linked/attached to corresponding D365 shipment/customs record | P2 |
| TC-E6-05 | Non-compliant shipment blocked | Shipment missing required compliance data | Attempt to dispatch | Dispatch blocked with clear compliance error message | P1 |

## 8. D365 Integration (Epic E7)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E7-01 | D365 OAuth2 authentication | Azure AD app registration configured | Platform requests D365 token | Token acquired via client-credentials flow; API calls authorized | P1 |
| TC-E7-02 | Inbound order sync | Order exists in D365 | Trigger sync | Order/customer/address data correctly mapped into platform domain model | P1 |
| TC-E7-03 | Outbound billing sync | Shipment marked Delivered with cost data | Trigger sync | Billing event posted to D365; amounts match source data | P1 |
| TC-E7-04 | Inventory sync on kit consumption | Kit consumed/returned | Trigger sync | D365 inventory reflects consumption/return accurately | P2 |
| TC-E7-05 | Shipping continues during D365 outage | D365 API simulated unavailable | Create/dispatch shipment | Shipment operations succeed locally; sync operation queued in outbox | P1 |
| TC-E7-06 | Outbox retry after D365 recovery | Queued sync operations exist, D365 restored | Wait for retry cycle / trigger manually | Queued operations sync successfully; reconciliation report shows 0 pending | P1 |
| TC-E7-07 | Reconciliation report accuracy | Mixed synced/pending/failed records exist | Open reconciliation report | Report accurately categorizes each record with drill-down detail | P2 |
| TC-E7-08 | D365 API throttling handling | D365 API returns 429/throttling response | Trigger sync | Platform backs off and retries per policy; no data loss or duplicate postings | P2 |

## 9. Monitoring & Dashboard (Epic E8)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E8-01 | Shipment volume dashboard accuracy | Known set of shipments exist | Open dashboard | Volume/exception/SLA metrics match underlying data | P2 |
| TC-E8-02 | D365 sync failure alert | Sync failure occurs | Wait for/trigger alert | Alert raised to platform administrator with actionable detail | P2 |
| TC-E8-03 | Application Insights trace correlation | Request spans GoLIMS and D365 call | Trigger request, inspect trace | End-to-end trace correlates both systems' spans under one operation ID | P3 |

## 10. Security, Identity & Administration (Epic E10)

| TC ID | Title | Preconditions | Steps | Expected Result | Priority |
|---|---|---|---|---|---|
| TC-E10-01 | Azure AD SSO login | User has valid Azure AD account with assigned role | Log in to platform | User authenticated via Azure AD; no separate credentials required | P1 |
| TC-E10-02 | Role-based access enforcement | Users assigned different roles (Lab Ops, Compliance, Finance, Admin) | Attempt to access restricted screen/API with insufficient role | Access denied (403) with no data leakage | P1 |
| TC-E10-03 | Audit log completeness | Sensitive action performed (custody event, reprint, rule change) | Query audit log | Entry present with correct user, timestamp, action, reason | P1 |
| TC-E10-04 | Unauthorized API access attempt | No/invalid token | Call protected API endpoint | Request rejected (401) | P1 |

## 11. Non-Functional Test Cases

| TC ID | Title | Type | Expected Result | Priority |
|---|---|---|---|---|
| TC-NFR-01 | Dashboard/shipment list load time | Performance | Loads within 2 seconds under normal load (per NFR-001) | P2 |
| TC-NFR-02 | Platform availability during D365 outage | Resilience | Core shipping functions remain available (per NFR-002) | P1 |
| TC-NFR-03 | Accessibility audit (WCAG 2.1 AA) | Accessibility | No critical/serious violations on core screens | P3 |
| TC-NFR-04 | Responsive layout on tablet | Usability | Core screens usable on tablet viewport in warehouse/lab context | P3 |
| TC-NFR-05 | Data encryption in transit/at rest | Security | TLS enforced on all endpoints; SQL Server TDE enabled | P1 |
| TC-NFR-06 | Load test — concurrent shipment creation | Performance | System sustains target throughput without error rate increase | P2 |

## 12. Traceability Note

Every test case above maps to a Product Backlog story (Epic/Story ID referenced in section headers)
and, transitively, to the BRD/URD requirement(s) that story satisfies. Test execution status should
be tracked per release phase (MVP / Lab Domain / Compliance & Scale) alongside the roadmap in the
Product Backlog document.
