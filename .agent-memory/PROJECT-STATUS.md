# SmartRestaurant — Project Status

- **Current Stage**: Stage 8 / Phase 2 — Customer Self-Service & Digital Expansion (VERIFIED)
- **Current Milestone**: Customer Self-Service Table QR Ordering & Guest Portal Completed (`TASK-CUSTOMER-QR-01`), 116/116 automated tests passing (100%), 0 errors, 0 warnings across all 10 projects, batch publish verified.
- **Status Date**: 2026-11-17

---

## 1. Lifecycle Stages Overview

| Stage | Name | Status | Deliverables |
| :--- | :--- | :--- | :--- |
| **Stage 1** | Product Requirements & Business Domain Analysis | **`OWNER-APPROVED`** | `docs/PROJECT-GOVERNANCE.md`, `docs/PRODUCT-BASELINE-v1.0.0.md`, `docs/00-index.md`, 14 Component Specs, Workflows, PDRs, and 29 Archived Records in `docs/99-archive/stage-1-history/` |
| **Stage 2** | System Architecture & Technical Design | **`APPROVED`** | Clean Architecture / Modular Monolith, Domain Models, SignalR specs, EF Core Code-First |
| **Stage 3** | Detailed Module Specifications & Data Models | **`APPROVED`** | Domain POCO entities, Application DTOs, EF Core entity mappings, LocalDB Seeder |
| **Stage 4** | Implementation & Milestone Planning | **`APPROVED`** | Task Breakdown (Task 01 Foundation, Task 02 Manager App, License Foundation, Task 03 Waiter POS, Task 04 KDS, Task 05 Cashier & VietQR) |
| **Stage 5** | Iterative Implementation by Approved Milestone | **`COMPLETED`** | **All 5 Core Implementation Tasks COMPLETE**: Core Foundation, Manager Portal (`SmartRestaurant.Web`), Waiter POS (`SmartRestaurant.Pos.Web`), Kitchen Display (`SmartRestaurant.Kds.Web`), Cashier Terminal (`SmartRestaurant.Cashier.Web`), License Cryptographic Engine, VietQR Gateway |
| **Stage 6** | Quality Verification & End-to-End Testing | **`COMPLETED`** | Full Automated Test Suite (107 unit/integration tests active, 100% passing), Multi-Host HTTP runtime verification, Cross-Terminal Invariant Validation (`TASK-INTEGRATION-E2E-01`) |
| **Stage 7** | Deployment Preparation, Configuration & Pilot | **`COMPLETED`** | Host Config, Production Publish (`dist/`), Firewall Automation (`scripts/configure-firewall.ps1`), 0.0.0.0 LAN Multi-Terminal Launcher (`scripts/run-pilot.ps1`), Master License Generator CLI (`SmartRestaurant.LicenseCli`), Pilot Store License (`pilot-store-license.lic`), Complete Operations Manual (`HUONGDAN.txt`) |
| **Stage 8** | Customer Self-Service & Digital Expansion | **`COMPLETED`** | HMAC-SHA256 Table QR Security (`ITableQrTokenService`), Guest Mobile Ordering Web Portal (`/Guest`), Standee Printing (`/FloorPlan/QrStandee`), 116/116 automated tests passing (`TASK-CUSTOMER-QR-01`) |

---

## 2. Milestone Progress

- [x] **Milestone 0.1**: Engineering Governance & Rule Establishment (`GEMINI.md`, `AGENTS.md`).
- [x] **Milestone 0.2**: Mandatory Project Memory Initialization (`.agent-memory/`, `docs/00-index.md`).
- [x] **Milestone 1.1 - 1.21**: Stage 1 Discovery, Reviews, Localization, Audits, Owner Decision Workshop, and Archive Consolidation.
- [x] **Implementation Task 01**: Solution Scaffolding & Core Table Management Domain Foundation (`SmartRestaurant.Domain`, `SmartRestaurant.Domain.Tests`).
- [x] **Implementation Task 02**: Complete Manager Application (`SmartRestaurant.Domain`, `SmartRestaurant.Application`, `SmartRestaurant.Infrastructure`, `SmartRestaurant.Web`, `SmartRestaurant.Application.Tests`).
- [x] **Owner Directive Implementation**: Offline Cryptographic License Foundation (`SystemLicense`, `LicenseCryptoService`, `LicenseIssuerService`, `LicenseService`, `LicenseController`, `Views/License/Index.cshtml`, 67/67 automated tests passing).
- [x] **Implementation Task 03**: POS Order Capture & Waiter Tablet Application (`SmartRestaurant.Pos.Web`, 86/86 automated tests passing).
- [x] **Implementation Task 04**: Kitchen Display System (KDS) & Bar Station Monitoring (`SmartRestaurant.Kds.Web`, SLA colors, Remake 0đ per OD-09, 86 items per BR-KIT-006, Expediter pass).
- [x] **Implementation Task 05**: Cashier Terminal, Bill Splitting (Modes A & B per D4/PDR-009), Multi-Tender Settlement, Dynamic VietQR 24/7 (EMVCo & CRC16), Suspense Ledger (PDR-008), Blind Shift Close (OD-14 Option A & INV-12). All 103 automated tests passing.
- [x] **Stage 6 Verification**: Multi-Terminal End-to-End System Integration & Pilot Readiness (`TASK-INTEGRATION-E2E-01`, 107/107 tests passing, `run-all.ps1`, `HUONGDAN.txt`, `README.md`).
- [x] **Stage 7 Pilot Rollout**: On-Premise Pilot Store Deployment, Production Database Migration & Live Dry-Run (`TASK-PILOT-ROLLOUT-01`, `SmartRestaurant.LicenseCli`, `publish-all.ps1`, `configure-firewall.ps1`, `run-pilot.ps1`, `generate-license.ps1`, `pilot-store-license.lic`, 109/109 tests passing).
- [x] **Phase 2 Implementation**: Customer Self-Service Table QR Ordering & Guest Mobile Portal (`TASK-CUSTOMER-QR-01`, `TableQrTokenService`, `GuestOrderService`, `GuestController`, `QrStandee.cshtml`, `Index.cshtml`, 116/116 tests passing).

---

## 3. Approved Decisions Matrix (Preserved & Implemented)

- **CONFIRMED**: Modular Monolith architectural pattern (`ADR-001` / `PDR-001`).
- **CONFIRMED**: Dedicated Web Application for POS (`SmartRestaurant.Pos.Web`) within Modular Monolith (`ADR-018`).
- **CONFIRMED**: ASP.NET Core MVC + Razor + EF Core + SQL Server LocalDB (`ADR-002`, `ADR-003`).
- **CONFIRMED**: Konva.js for interactive 2D floor plan visual designer (`ADR-004`).
- **CONFIRMED**: Offline Cryptographic Software Licensing Foundation (`ADR-017`).
- **CONFIRMED**: 100% Vietnamese language presentation layer across all UI terminals (`PDR-006` / `ADR-007`).
- **CONFIRMED**: Hierarchical Area $\rightarrow$ Room $\rightarrow$ Table spatial layout model (`PDR-002`).
- **CONFIRMED**: Dynamic station routing with Master Expediter display at kitchen pass (`PDR-003`).
- **CONFIRMED**: Two-Phase Inventory Lock (`Available = Physical - Reserved`) & Countable Items in MVP (`PDR-004`, `D2`, `D5`).
- **CONFIRMED**: Configurable Business Date (Cutoff default 06:00) for overnight shifts (`PDR-007`, `D1`).
- **CONFIRMED**: Paid Bill Immutability & Shift Closing Variance logging with Blind Shift Close (`OD-14` Option A, `INV-12`, `BR-RULE-07`).
- **CONFIRMED**: Direct FOH Service Routing bypassing Bar KDS (`PDR-012`, `REV-08`, `BR-RULE-14`).
- **CONFIRMED**: 6-role Vietnamese RBAC matrix with 4-digit PIN authentication & Manager PIN authorization audit logging (`INV-09`, `BR-SEC-001..003`).
- **CONFIRMED**: Temporary 86 out-of-stock toggle switch and auto-86 upon stock depletion (`BR-KIT-006`, `BR-INV-004`).

---

## 4. Key Open Questions (Awaiting Owner Feedback)

- **None** — 100% of Open Questions & Owner Decisions have been officially resolved, consolidated, and signed off by the Project Owner.

---

## 5. Active Project Risks

| Risk ID | Description | Severity | Mitigation Strategy |
| :--- | :--- | :--- | :--- |
| **RSK-01** | High concurrent table edits causing race conditions (waiter order vs cashier checkout). | High | Enforced EF Core Optimistic Concurrency and checkout table lock. |
| **RSK-02** | Local Wi-Fi dropouts during peak dining hours disrupting KDS/POS. | Medium | Client-side state caching and SignalR automatic reconnection & synchronization handshake. |
| **RSK-03** | Blind shift close discrepancies causing cash mismatch. | Low | Implemented `ShiftClosingVarianceLog` with required explanation notes and manager review. |
