# SmartRestaurant — Development Log

This file maintains a chronological, tamper-evident audit trail of executed tasks, verified deliverables, and changes across the codebase.

---

### [2026-11-02] — Task 01: Establish Permanent Project Instructions
- **Objective**: Establish permanent engineering governance and project instructions.
- **Deliverables**:
  - Authored [GEMINI.md](../GEMINI.md) defining long-term architectural principles, memory strategy, documentation standards, and 7-stage lifecycle.
  - Authored [AGENTS.md](../AGENTS.md) covering engineering conventions, clean architecture rules, concurrency management, and zero-stub implementation mandate.
  - Configured [.gitignore](../.gitignore) for .NET, Visual Studio, and local environment secrets.
  - Updated [README.md](../README.md) with system domain overview.
- **Build & Test Results**: Documentation and governance baseline only; no code compiled.
- **Status**: Completed & Verified.

---

### [2026-11-02] — Task 02: Product Discovery & Product Requirements Specification (PRD) Suite
- **Objective**: Execute comprehensive Stage 1 product discovery and produce complete, implementation-ready business specifications under `docs/product/`.
- **Deliverables**:
  - Initialized `.agent-memory/` repository with `README.md`, `PROJECT-STATUS.md`, `ARCHITECTURE-DECISIONS.md`, `CODEBASE-MAP.md`, `DEVELOPMENT-LOG.md`, `NEXT-TASK.md`, and `KNOWN-ISSUES.md`.
  - Authored complete Product Requirements Suite under `docs/product/`:
    - `PRD.md`: Executive summary, target customers, business objectives, KPIs, and scope.
    - `BUSINESS-REQUIREMENTS.md`: Detailed functional requirements, actor permissions, and business rules.
    - `MVP-SCOPE.md`: MoSCoW prioritization, release milestones, and explicit MVP exclusions.
    - `DOMAIN-GLOSSARY.md`: Standardized English & Vietnamese operational vocabulary.
    - `USER-JOURNEYS.md`: 10 real-world end-to-end actor journeys with Mermaid sequence diagrams.
    - `OPEN-QUESTIONS.md`: Tier 1 (MVP Blockers) and Tier 2 (Post-MVP) decision analysis.
    - `MODULE-OVERVIEW.md`: Boundaries, capabilities, and dependencies for 14 core business modules.
    - `NON-FUNCTIONAL-REQUIREMENTS.md`: Latency targets, security, data isolation, and concurrency rules.
    - `workflows/`: 4 deep-dive workflow specifications with Mermaid state and sequence diagrams:
      - `FLOOR-PLAN-WORKFLOW.md`
      - `ORDER-WORKFLOW.md`
      - `KITCHEN-WORKFLOW.md`
      - `CASHIER-PAYMENT-WORKFLOW.md`
    - `decisions/`: 5 Product Decision Records (PDR-001 through PDR-005).
  - Updated master documentation index at [docs/00-index.md](../docs/00-index.md).
- **Execution Constraints Enforced**:
  - Zero .NET solutions or C# source files created.
  - Zero database schemas or EF Core entities written.
  - Zero packages installed.
  - Stopped immediately upon completing Stage 1 specifications.
- **Status**: Completed; Submitted for Owner Review and Approval.

---

### [2026-11-02] — Task 02.1: Author Vietnamese-First Localization Standard
- **Objective**: Establish the definitive product localization standard enforcing 100% Vietnamese presentation tier across all client terminals.
- **Deliverables**:
  - Authored [VIETNAMESE-LOCALIZATION-STANDARD.md](../docs/product/VIETNAMESE-LOCALIZATION-STANDARD.md) establishing:
    - Standard regional formats: Currency (`#,,### ₫`), Dates (`dd/MM/yyyy`), Times (`HH:mm:ss`), Phone numbers, and VAT ("Thuế GTGT").
    - Complete subsystem domain dictionaries (Floor plan, Menu, POS, KDS, Cashier).
    - ESC/POS thermal printing standards (UTF-8, Windows-1258, and ASCII unaccented fallback).
    - SweetAlert2 notification and validation dialog templates.
  - Authored decision record [PDR-006](../docs/product/decisions/PDR-006-vietnamese-language-ui.md) (`CONFIRMED`).
  - Updated [NON-FUNCTIONAL-REQUIREMENTS.md](../docs/product/NON-FUNCTIONAL-REQUIREMENTS.md) with `NFR-LOC-01`.
  - Updated [docs/00-index.md](../docs/00-index.md), [PRD.md](../docs/product/PRD.md), [PROJECT-STATUS.md](./PROJECT-STATUS.md), and [ARCHITECTURE-DECISIONS.md](./ARCHITECTURE-DECISIONS.md).
- **Status**: Completed & Verified.

---

### [2026-11-02] — Task 02.5: Product Review & Business Validation
- **Objective**: Conduct formal architectural review and business validation of the entire Stage 1 Product Specification Suite.
- **Deliverables**:
  - Authored [PRODUCT-REVIEW-VALIDATION.md](../docs/product/PRODUCT-REVIEW-VALIDATION.md) covering:
    - Cross-document consistency and requirement traceability matrix across 10 specifications, 4 workflows, and 6 decision records.
    - Persona stress-testing under operational restaurant rush hours (Waitstaff, Line Cooks, Expediter, Cashier, Manager).
    - Edge-case scenario resolutions (concurrent checkout lock, Wi-Fi reconnection handshake, post-fire dish cancellation with scrap attribution).
    - Technical and architectural readiness certification for Stage 2 (Modular Monolith layers, ERD modeling, SignalR contracts).
    - Validation sign-off checklist certifying 100% Vietnamese-First UI, regional formatting, and clean architecture alignment.
  - Updated [docs/00-index.md](../docs/00-index.md) and [.agent-memory/PROJECT-STATUS.md](./PROJECT-STATUS.md).
- **Execution Constraints Enforced**: Documentation review and business validation only; zero application code or database schema written.
- **Status**: Completed & Verified.

---

### [2026-11-02] — Task 02.5 (Phần 2): Independent Product Review & Critical Business Validation
- **Objective**: Conduct an independent, critical product review of SmartRestaurant specifications, challenging assumptions, identifying operational gaps, and auditing 12 complex restaurant scenarios.
- **Deliverables**:
  - Authored [PRODUCT-REVIEW-REPORT.md](../docs/product/PRODUCT-REVIEW-REPORT.md) detailing:
    - 4 CRITICAL issues: Business Date for overnight shifts (`REV-01`), Two-Phase Inventory Lock for BOM overselling prevention (`REV-02`), VietQR delayed webhook race conditions (`REV-03`), Order freeze during bill splitting (`REV-04`).
    - 6 HIGH severity issues: Unmerging tables (`REV-05`), Area/VIP surcharges (`REV-06`), Accounts Receivable / Corporate charge accounts (`REV-07`), Concurrency delta append-only ordering (`REV-10`), Network print spools, and Document contradictions (`CONTR-01`, `CONTR-02`).
    - Strategic MVP scope recommendations: Postponing deep gram/ml BOM stock deduction to v1.1 to protect MVP delivery schedule.
    - Identification of 5 strategic decisions requiring project owner approval.
  - Updated [docs/00-index.md](../docs/00-index.md) and [.agent-memory/PROJECT-STATUS.md](./PROJECT-STATUS.md).
- **Execution Constraints Enforced**: Pure product review; no code, database schemas, or automatic architecture changes made.
- **Status**: Completed; Standing by for Project Owner Decisions.

---

### [2026-11-02] — Task 02.6: Resolve Product Review Findings with Project Owner & Final Stage 1 Alignment
- **Objective**: Formally review and resolve all 5 strategic architectural decisions (D1–D5), operational findings (REV-05 to REV-11), and document contradictions (CONTR-01, CONTR-02) with the Project Owner.
- **Deliverables & Approved Decisions**:
  - **D1 (Business Date & Overnight Shifts)**: `CONFIRMED` (`PDR-007`). Branch-level configurable cutoff time (default 06:00). Immutable business date bound at order creation.
  - **D2 (Inventory Reservation)**: `CONFIRMED` (`PDR-004`). `Available = Physical - Reserved`. Reservation on order confirmation; deduction on prep/serve; waste log on post-prep cancel. Concurrency guard against negative stock.
  - **D3 (VietQR Delayed Webhook)**: `CONFIRMED` (`PDR-008`). Cash fallback allowed on timeout. Late webhook routed to Suspense Ledger with instant cashier alert. Anti-duplicate matching. Governed manual reconciliation.
  - **D4 (Split Bill - Multi-Bill per Table)**: `CONFIRMED` (`PDR-009`). Multi-bill table session. Tables are NEVER frozen during split billing. Waiters continue adding items with explicit target sub-bill assignment. Release only on 100% completion.
  - **D5 (MVP BOM Scope)**: `CONFIRMED` (`PDR-004`). MVP tracks direct saleable countable items (portions, bottles, cans). Sub-gram BOM recipe deduction deferred to v1.1 with modular extensible architecture.
  - **REV-05 (Unmerge Tables)**: `CONFIRMED` (`PDR-010`). Unmerge joined tables with line item reallocation without kitchen ticket re-firing.
  - **REV-06 (VIP/Area Surcharges)**: `CONFIRMED` (`PDR-011`). Surcharges by % and flat fee snapshotted at creation; explicit invoice line item; hourly fee deferred.
  - **REV-07 (Khách Ký Nợ)**: `DEFERRED`. Excluded from MVP; postponed to v1.2+ as a dedicated B2B AR & Credit Management module.
  - **REV-08 (Direct Beverage Pick)**: `CONFIRMED` (`PDR-012`). Direct grab items bypass Bar KDS queue while maintaining full billing and inventory tracking.
  - **REV-09 (Shared Room Lifecycle)**: `CONFIRMED` (`PDR-013`). Independent table lifecycle by default; entire room booking blocks all child tables; synchronous release on checkout.
  - **REV-10 (Concurrent Waiters)**: `CONFIRMED` (`PDR-014`). Non-blocking shared ordering business rules approved; technical mechanism delegated to Stage 2 Solution Architecture.
  - **REV-11 (Remake Dish)**: `CONFIRMED` (`PDR-015`). Remake workflow for kitchen error without guest surcharge; purple KDS card; scrap logged to Waste Register.
  - **CONTR-01 & CONTR-02**: Clarified multi-branch operational independence in MVP; standardized two-phase inventory lifecycle across all specs.
- **Specification Suite Synchronization**:
  - Updated `PDR-004`, `PDR-007`, `PDR-008`, and rewrote `PDR-009`.
  - Authored `PDR-010`, `PDR-011`, `PDR-012`, `PDR-013`, `PDR-014`, `PDR-015`.
  - Updated and aligned `PRD.md`, `BUSINESS-REQUIREMENTS.md`, `MVP-SCOPE.md`, `OPEN-QUESTIONS.md`, `PRODUCT-REVIEW-REPORT.md`, `docs/00-index.md`, and all 4 workflow specs.
  - Synchronized `.agent-memory/PROJECT-STATUS.md` and `.agent-memory/NEXT-TASK.md`.
- **Stage 1 Status**: 100% Formally completed, validated, reconciled, and signed off. Ready for Stage 2 (Technical Architecture & Solution Design).
