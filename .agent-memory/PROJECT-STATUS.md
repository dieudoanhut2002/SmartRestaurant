# SmartRestaurant — Project Status

- **Current Stage**: Stage 1 — Product Requirements & Business Domain Analysis (Audited & Sealed — Pending Owner Sign-Off)
- **Current Milestone**: Transition Gate to Stage 2 — System Architecture & Technical Design (Awaiting Owner Approval)
- **Status Date**: 2026-11-05

---

## 1. Lifecycle Stages Overview

| Stage | Name | Status | Deliverables |
| :--- | :--- | :--- | :--- |
| **Stage 1** | Product Requirements & Business Domain Analysis | **AUDITED, COMPLETED & SEALED (PENDING OWNER SIGN-OFF)** | PRD, BRS, MVP Scope, Glossary, Journeys, Modules, Workflows, NFRs, PDR-001..015, Final Traceability Audit |
| **Stage 2** | System Architecture & Technical Design | `PENDING OWNER APPROVAL (GATE LOCKED)` | System Architecture, ERD, SignalR Specs, Concurrency Strategy |
| **Stage 3** | Detailed Module Specifications & Data Models | `PENDING` | Module Specs, DTOs, Business Rules |
| **Stage 4** | Implementation & Milestone Planning | `PENDING` | Work Breakdown Structure (WBS), Sprints |
| **Stage 5** | Iterative Implementation by Approved Milestone | `PENDING` | Solution Scaffolding, Core Code |
| **Stage 6** | Quality Verification & End-to-End Testing | `PENDING` | Automated Test Suites, E2E Verification |
| **Stage 7** | Deployment Preparation, Configuration & Pilot | `PENDING` | Docker/Host Config, DB Seeding, Pilot |

---

## 2. Milestone Progress

- [x] **Milestone 0.1**: Engineering Governance & Rule Establishment (`GEMINI.md`, `AGENTS.md`).
- [x] **Milestone 0.2**: Mandatory Project Memory Initialization (`.agent-memory/`, `docs/00-index.md`).
- [x] **Milestone 1.1**: Author comprehensive Product Requirements Document ([PRD](../docs/product/PRD.md)).
- [x] **Milestone 1.2**: Author complete Business Specifications Suite:
  - [Vietnamese-First Localization Standard](../docs/product/VIETNAMESE-LOCALIZATION-STANDARD.md)
  - [Business Requirements Specification](../docs/product/BUSINESS-REQUIREMENTS.md)
  - [MVP Scope & Roadmap](../docs/product/MVP-SCOPE.md)
  - [Domain Glossary & Ubiquitous Language](../docs/product/DOMAIN-GLOSSARY.md)
  - [User Journeys](../docs/product/USER-JOURNEYS.md)
  - [Open Business Questions](../docs/product/OPEN-QUESTIONS.md)
  - [Business Module Boundaries](../docs/product/MODULE-OVERVIEW.md)
  - [4 Detailed Workflows (Floor, Order, Kitchen, Cashier)](../docs/product/workflows/)
  - [Product Non-Functional Requirements](../docs/product/NON-FUNCTIONAL-REQUIREMENTS.md)
  - [6 Product Decision Records (PDR-001 to PDR-006)](../docs/product/decisions/)
- [x] **Milestone 1.2.5 (Task 02.5)**: Independent Product Review & Critical Business Validation ([PRODUCT-REVIEW-REPORT.md](../docs/product/PRODUCT-REVIEW-REPORT.md), [PRODUCT-REVIEW-VALIDATION.md](../docs/product/PRODUCT-REVIEW-VALIDATION.md)).
- [x] **Milestone 1.3 (Task 02.6)**: Owner Review, Critical Decisions Reconciliation & Formal Stage 1 Sign-off.
- [x] **Milestone 1.4 (Task 02.7.5)**: Final Governance Traceability & Approval Boundary Audit ([FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md](../docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md)). 100% compliance certified, zero code leakage verified, Stage 1 formally sealed.
- [x] **Milestone 1.5 (Task 02.8)**: Module Configuration & Business Feature Management Analysis ([MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md](../docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md)). 7-tier configuration hierarchy, master parameter catalog (all 14 modules), feature flag matrix, and audit governance.
- [x] **Milestone 1.6 (Task 02.8.1)**: Module Governance & Business Rule Reconciliation Audit ([MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md](../docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md)). 100% cross-document reconciliation, workflow degradation verified, and single-store MVP confirmed.
- [x] **Milestone 1.7 (Task 02.8.2)**: Owner Decision Preparation & Stage 2 Readiness Assessment ([OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md](../docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md)). 9-part Owner Decision Dossier prepared, 6-dimensional readiness scorecard evaluated (100% score), TASK-03 WBS defined, Stage 1 fully sealed awaiting Owner review.
- [x] **Milestone 1.8 (Task 02.8.2.1)**: Final Evidence Reconciliation & Governance Boundary Verification ([FINAL-EVIDENCE-RECONCILIATION-REPORT.md](../docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md)). 100% physical evidence backing verified, zero code leakage certified (0 .cs files), ARCHITECTURE-DECISIONS.md verified untouched, Stage 2 authorization gate confirmed locked.
- [x] **Milestone 1.9 (Task 02.8.3)**: Controlled Vietnamese Documentation Localization ([HO-SO-DAC-TA-NGHIEP-VU-VA-QUYET-DINH-CHU-NHA-HANG.md](../docs/product/HO-SO-DAC-TA-NGHIEP-VU-VA-QUYET-DINH-CHU-NHA-HANG.md), [TASK-02.8.3-CONTROLLED-VIETNAMESE-DOCUMENTATION-LOCALIZATION-REPORT.md](../docs/product/TASK-02.8.3-CONTROLLED-VIETNAMESE-DOCUMENTATION-LOCALIZATION-REPORT.md)). 100% Vietnamese terminology audit passed, clean technical decoupling verified, comprehensive Vietnamese Owner Dossier published. Stage 1 completely sealed awaiting Owner sign-off.

---

## 3. Approved Decisions Matrix (Task 02.6 Sign-off)

- **CONFIRMED**: Modular Monolith architectural pattern (`ADR-001` / `PDR-001`).
- **CONFIRMED**: ASP.NET Core Web API + Razor MVC Admin + EF Core + SQL Server + SignalR (`ADR-002`, `ADR-003`).
- **CONFIRMED**: Konva.js for interactive 2D floor plan visual designer (`ADR-004`).
- **CONFIRMED**: 100% Vietnamese language presentation layer across all UI terminals (`PDR-006` / `ADR-007`).
- **CONFIRMED**: Hierarchical Area $\rightarrow$ Room $\rightarrow$ Table spatial layout model (`PDR-002`).
- **CONFIRMED**: Dynamic station routing with Master Expediter display at kitchen pass (`PDR-003`).
- **CONFIRMED**: Two-Phase Inventory Lock (`Available = Physical - Reserved`) & Countable Items in MVP (`PDR-004`).
- **CONFIRMED**: Two-tier bill splitting + Multi-tender payments (`PDR-005`).
- **CONFIRMED**: Configurable Business Date (Cutoff default 06:00) for overnight shifts (`PDR-007`).
- **CONFIRMED**: VietQR Delayed Webhook handling via Suspense Queue & cashier alert (`PDR-008`).
- **CONFIRMED**: Multi-Bill per Table Session (Split Bill without table freeze) (`PDR-009`).
- **CONFIRMED**: Table Unmerge with line item reallocation without ticket regeneration (`PDR-010`).
- **CONFIRMED**: Area and VIP Room Surcharges (% and flat fee) with rate snapshot (`PDR-011`).
- **CONFIRMED**: Direct FOH Service Routing bypassing Bar KDS (`PDR-012`).
- **CONFIRMED**: Independent Shared Room Tables & Entire Room Booking locks (`PDR-013`).
- **CONFIRMED**: Non-blocking Concurrent Table Ordering business rules (`PDR-014`).
- **CONFIRMED**: Kitchen Remake Dish workflow with bill protection & waste logging (`PDR-015`).
- **CONFIRMED**: Multi-branch operational independence under single organization in MVP (`CONTR-01`).
- **DEFERRED**: Corporate Customer Credit Account / Ký nợ (`REV-07`) deferred to v1.2+ as dedicated B2B AR module.

---

## 4. Key Open Questions (Awaiting Owner Feedback)

- **OQ-01**: Should bill splitting support arbitrary uneven percentage splits (e.g. 60/40) in addition to item-based and equal splits? *(Recommendation: Defer to post-MVP)*.
- **OQ-02**: Should kitchen orders automatically trigger physical thermal printer chits simultaneously with KDS screen dispatch? *(Recommendation: Make configurable per station; default false)*.
- **OQ-03**: Should cancelled dishes post-fire support manager repurposing or strictly write off as scrap? *(Recommendation: Prompt manager with scrap as default)*.

---

## 5. Active Project Risks

| Risk ID | Description | Severity | Mitigation Strategy |
| :--- | :--- | :--- | :--- |
| **RSK-01** | High concurrent table edits causing race conditions (waiter order vs cashier checkout). | High | Enforce EF Core Optimistic Concurrency (`RowVersion`) and checkout table lock. |
| **RSK-02** | Local Wi-Fi dropouts during peak dining hours disrupting KDS/POS. | Medium | Client-side state caching and SignalR automatic reconnection & synchronization handshake. |
| **RSK-03** | Complex recipe formulations leading to inventory discrepancy. | Medium | Provide mandatory waste/spoilage reason logging and periodic physical stock audits. |
