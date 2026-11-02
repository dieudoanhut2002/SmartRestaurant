# SmartRestaurant — Project Status

- **Current Stage**: Stage 1 — Product Requirements & Business Domain Analysis
- **Current Milestone**: Milestone 1.2 — Product Discovery & Business Specifications Review
- **Status Date**: 2026-11-02

---

## 1. Lifecycle Stages Overview

| Stage | Name | Status | Deliverables |
| :--- | :--- | :--- | :--- |
| **Stage 1** | Product Requirements & Business Domain Analysis | **COMPLETED (Pending Sign-off)** | PRD, BRS, MVP Scope, Glossary, Journeys, Modules, Workflows, NFRs, PDRs |
| **Stage 2** | System Architecture & Technical Design | `PENDING` | System Architecture, ERD, SignalR Specs |
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
- [ ] **Milestone 1.3**: Owner Review, Feedback Reconciliation & Stage 1 Sign-off.

---

## 3. Approved & Proposed Decisions

- **CONFIRMED**: Modular Monolith architectural pattern (`ADR-001` / `PDR-001`).
- **CONFIRMED**: ASP.NET Core Web API + Razor MVC Admin + EF Core + SQL Server + SignalR (`ADR-002`, `ADR-003`).
- **CONFIRMED**: Konva.js for interactive 2D floor plan visual designer (`ADR-004`).
- **CONFIRMED**: 100% Vietnamese language presentation layer across all UI terminals (`PDR-006`).
- **PROPOSED**: Hierarchical Area $\rightarrow$ Room $\rightarrow$ Table spatial layout model (`PDR-002`).
- **PROPOSED**: Dynamic station routing with Master Expediter display at kitchen pass (`PDR-003`).
- **PROPOSED**: Recipe BOM inventory deduction on dish readiness (`ReadyToServe`) (`PDR-004`).
- **PROPOSED**: Two-tier bill splitting (By items & Evenly) + Multi-tender payments (`PDR-005`).

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
