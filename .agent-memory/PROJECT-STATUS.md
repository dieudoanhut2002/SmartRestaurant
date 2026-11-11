# SmartRestaurant — Project Status

- **Current Stage**: Stage 5 — Iterative Implementation by Approved Milestone
- **Current Milestone**: Implementation Task 02 Completed — Complete Manager Application Delivered & Verified (ASP.NET Core MVC + Razor, Clean Architecture across Domain, Application, Infrastructure, Web, Konva.js Floor Plan 2D Designer, Menu Variants & Modifiers, 86 Toggle, Kitchen Stations & KDS Routing, Inventory 4-State Tracking & Available Invariant INV-03, Restaurant Settings & Cutoff 06:00, 3 Onboarding Presets, RBAC Staff & PIN Authentication, Blind Shift Close Variance Accounting, Operational Reports, 45/45 Tests Passing, 0 Errors, 0 Warnings).
- **Status Date**: 2026-11-11

---

## 1. Lifecycle Stages Overview

| Stage | Name | Status | Deliverables |
| :--- | :--- | :--- | :--- |
| **Stage 1** | Product Requirements & Business Domain Analysis | **`OWNER-APPROVED`** | `docs/PROJECT-GOVERNANCE.md`, `docs/PRODUCT-BASELINE-v1.0.0.md`, `docs/00-index.md`, 14 Component Specs, Workflows, PDRs, and 29 Archived Records in `docs/99-archive/stage-1-history/` |
| **Stage 2** | System Architecture & Technical Design | **`APPROVED`** | Clean Architecture / Modular Monolith, Domain Models, SignalR specs, EF Core Code-First |
| **Stage 3** | Detailed Module Specifications & Data Models | **`APPROVED`** | Domain POCO entities, Application DTOs, EF Core entity mappings, LocalDB Seeder |
| **Stage 4** | Implementation & Milestone Planning | **`APPROVED`** | Task Breakdown (Task 01 Foundation, Task 02 Manager App, Task 03 Waiter POS, Task 04 KDS, Task 05 Cashier & VietQR) |
| **Stage 5** | Iterative Implementation by Approved Milestone | **`IN PROGRESS`** | **Task 01 & Task 02 COMPLETE**: Solution foundation (`SmartRestaurant.slnx`), Domain Entities, Application Services, Infrastructure LocalDB Seeder, and Complete Manager Web Application |
| **Stage 6** | Quality Verification & End-to-End Testing | `PENDING` | Automated Test Suites (33 unit/integration tests active), E2E Verification |
| **Stage 7** | Deployment Preparation, Configuration & Pilot | `PENDING` | Host Config, Production DB Migration, Pilot Store Rollout |

---

## 2. Milestone Progress

- [x] **Milestone 0.1**: Engineering Governance & Rule Establishment (`GEMINI.md`, `AGENTS.md`).
- [x] **Milestone 0.2**: Mandatory Project Memory Initialization (`.agent-memory/`, `docs/00-index.md`).
- [x] **Milestone 1.1 - 1.21**: Stage 1 Discovery, Reviews, Localization, Audits, Owner Decision Workshop, and Archive Consolidation.
- [x] **Implementation Task 01**: Solution Scaffolding & Core Table Management Domain Foundation (`SmartRestaurant.Domain`, `SmartRestaurant.Domain.Tests`).
- [x] **Implementation Task 02**: Complete Manager Application (`SmartRestaurant.Domain`, `SmartRestaurant.Application`, `SmartRestaurant.Infrastructure`, `SmartRestaurant.Web`, `SmartRestaurant.Application.Tests`).
- [ ] **Implementation Task 03**: POS Order Capture & Waiter Tablet Application.
- [ ] **Implementation Task 04**: Kitchen Display System (KDS) & Bar Station Monitoring.
- [ ] **Implementation Task 05**: Cashier Terminal, Bill Splitting & Dynamic VietQR 24/7 Gateway.

---

## 3. Approved Decisions Matrix (Preserved & Implemented)

- **CONFIRMED**: Modular Monolith architectural pattern (`ADR-001` / `PDR-001`).
- **CONFIRMED**: ASP.NET Core MVC + Razor + EF Core + SQL Server LocalDB (`ADR-002`, `ADR-003`).
- **CONFIRMED**: Konva.js for interactive 2D floor plan visual designer (`ADR-004`).
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
