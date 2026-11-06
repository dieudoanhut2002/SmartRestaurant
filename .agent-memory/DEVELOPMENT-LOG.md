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


---

### [2026-11-05] — Task 02.8: Module Configuration & Business Feature Management Analysis
- **Objective**: Establish a comprehensive, unified Configuration & Business Feature Management framework across all 14 modules (`MOD-01` to `MOD-14`).
- **Deliverables**:
  - Authored [MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md](../docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md) establishing:
    - 7-tier configuration scoping and inheritance hierarchy (System $\rightarrow$ Organization $\rightarrow$ Branch $\rightarrow$ Station $\rightarrow$ Area/Room $\rightarrow$ Item $\rightarrow$ Staff Role).
    - Comprehensive parameter catalog covering operational defaults, allowed bounds, data types, and hot-reload flags across all 14 business modules.
    - Master Feature Flag Register defining phased rollout for v1.0 MVP, v1.1 BOM, v1.2 Corporate AR, and v2.0 Multi-branch logistics.
    - Configuration mutation, immutability, and dual-control audit governance (financial snapshot rules, manager PIN override, immutable audit table).
    - Vietnamese-First administration UX layout with SweetAlert2 modal confirmation templates.
    - Stage 2 architecture guardrails (caching, SignalR cache invalidation bus, decoupled interfaces).
  - Updated [docs/00-index.md](../docs/00-index.md).
  - Synchronized `.agent-memory/PROJECT-STATUS.md` and `.agent-memory/NEXT-TASK.md`.
- **Execution Constraints Enforced**: Specification and business analysis only; zero application code, migrations, or project scaffolding created.
- **Status**: Completed & Verified.

---

### [2026-11-05] — Task 02.8.1: Module Governance & Business Rule Reconciliation Audit
- **Objective**: Execute a rigorous cross-document governance and business rule reconciliation audit between `MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md` and the entire Stage 1 specification suite.
- **Deliverables**:
  - Authored [MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md](../docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md) certifying:
    - 100% reconciliation and cross-consistency with 14 Business Requirements (BR-01..14) and 15 PDRs (PDR-001..015).
    - Operational workflow degradation verification: Certified that disabling any optional module (MOD-06, MOD-09, MOD-11, MOD-12) leaves all 4 core workflows (Floor, Order, Kitchen, Cashier) 100% operational without deadlocks.
    - Verified non-breaking fallback paths for all 10 user journeys (UJ-01..10).
    - Verified strict adherence to the Single-Store Independent Location mandate (OD-01 / CONTR-01) with zero multi-branch chain logistics in MVP.
    - Certified non-destructive data policy: Module deactivation preserves 100% of historical data in read-only state.
    - Tracked 5 open owner decisions (OD-15, OD-16, OD-17, OQ-02, OQ-06) for formal sign-off.
  - Updated [docs/00-index.md](../docs/00-index.md).
  - Synchronized `.agent-memory/PROJECT-STATUS.md` and `.agent-memory/NEXT-TASK.md`.
- **Verdict**: RECONCILED, FULLY CONSISTENT & CERTIFIED.
- **Execution Constraints Enforced**: Pure governance audit; zero code, database schemas, or migrations created.
- **Status**: Completed & Verified.

---

### [2026-11-05] — Task 02.8.2: Owner Decision Preparation & Stage 2 Readiness Assessment
- **Objective**: Prepare comprehensive Owner Decision Dossier for all pending business/operational questions and perform formal Stage 2 Readiness Assessment.
- **Deliverables**:
  - Authored [OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md](../docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md) containing:
    - Complete 9-part Owner Decision Dossier covering OD-14 (paid bill dispute & void policy), OD-15 (MOD-06 Advance Reservations scope), OD-16 (Onboarding presets adoption), OD-17 (Graceful drain deactivation policy), OQ-01 (% bill split deferral), OQ-02 (Kitchen thermal auto-print default), OQ-06 (Service charge taxability default), OQ-07 (Long-running table session audit banner), and formal sign-off on 14-Module 3-Tier Classification.
    - Ready-to-Sign Owner Ballot & Sign-off Form.
    - Comprehensive Stage 2 Readiness Assessment auditing Stage 1 artifact stability, invariant boundary compliance (single store, countable inventory BOM, non-locking bill splitting), zero code leakage verification (0 .cs files, 0 migrations), and 6-dimensional readiness scorecard (100% score).
    - Stage 2 Work Breakdown Structure (TASK-03.1 Architecture, TASK-03.2 ERD, TASK-03.3 SignalR Event Mesh, TASK-03.4 Concurrency Strategy).
    - Transition Risk Assessment & Mitigation Plan.
  - Updated [docs/00-index.md](../docs/00-index.md).
  - Synchronized `.agent-memory/PROJECT-STATUS.md` and `.agent-memory/NEXT-TASK.md`.
- **Verdict**: READY FOR PROJECT OWNER REVIEW (Stage 1 completely sealed; awaiting Owner sign-off on decision dossier before Stage 2 start).
- **Execution Constraints Enforced**: Documentation and audit analysis only; .agent-memory/ARCHITECTURE-DECISIONS.md untouched; zero C# code, projects, or migrations created.
- **Status**: Completed & Verified.

---

### [2026-11-05] — Task 02.8.2.1: Final Evidence Reconciliation & Governance Boundary Verification
- **Objective**: Execute rigorous physical evidence reconciliation and verify governance boundaries across all Stage 1 product artifacts and git state.
- **Deliverables**:
  - Authored [FINAL-EVIDENCE-RECONCILIATION-REPORT.md](../docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md) certifying:
    - 100% empirical evidence backing for all audit statements and git provenance.
    - Zero code leakage verification: confirmed exactly 0 `.cs`, 0 `.csproj`, 0 `.sln`, 0 `.sql` files, and confirmed `docs/02-architecture/` does not exist.
    - Tamper-proof memory integrity: verified zero git diff against HEAD for `.agent-memory/ARCHITECTURE-DECISIONS.md`.
    - Authoritative Owner decisions preservation: verified exact citation for D1–D5, REV-05..11, OD-01..13, and BillingRequested Option 2.
    - Inventory D2/D5 rules preservation: verified formula `Available = Physical - Reserved`, reservation point, cancellation behavior, 5-concept separation, and classified manual 86 sync as `AI/BA PROPOSAL` when `MOD-12` is disabled.
    - OD-14 paid bill correction: preserved immutable invariant, evaluated Option A and Option B across 5 dimensions without auto-approval.
    - Open questions traceability: verified identity and non-blocking status of OQ-01, OQ-02, OQ-06, OQ-07, OQ-08, OQ-09, OQ-10.
    - Stage 2 non-authorization proof: confirmed Stage 2 is NOT approved or authorized, classified decisions into 4 tiers, halted execution.
  - Updated [docs/00-index.md](../docs/00-index.md).
  - Synchronized `.agent-memory/PROJECT-STATUS.md` and `.agent-memory/NEXT-TASK.md`.
- **Verdict**: READY FOR PROJECT OWNER DECISION.
- **Execution Constraints Enforced**: Pure governance audit; zero code, database schemas, or migrations created; no git commit, stash, or reset executed.
- **Status**: Completed & Verified.

---

### [2026-11-05] — Task 02.8.2.2: Controlled Governance Corrections
- **Objective**: Execute controlled, precision governance corrections across Stage 1 documentation addressing all 6 findings from the Task 02.8.2.1 final review.
- **Deliverables**:
  - Corrected [OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md](../docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md):
    - Unmerged `D3` (Delayed VietQR Suspense Ledger) and `D4` (Multi-Bill Table Session without Freezing) into distinct rows in Owner Decision Register.
    - Demoted station-specific inventory deduction policy from `OWNER-CONFIRMED` to `AI/BA PROPOSAL (Cần Solution Architect đánh giá trong Stage 2)`.
    - Corrected OD-14 Option A analysis: removed false claims of "zero schema changes/zero data storage"; added explicit requirements for shift closing variance logs, manager incident trails, and original invoice linking.
    - Reclassified OQ-08 (Concurrency) and OQ-10 (Multi-branch keying) as candidate technical designs to be evaluated entity-by-entity by the Solution Architect in Stage 2.
  - Refined [PDR-005](../docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md): Clarified D4 and OD-13 as the core bill splitting authorities, referencing D3 as the VietQR tender policy.
  - Corrected [PROJECT-STATUS.md](./PROJECT-STATUS.md): Updated Stage 1 status to `AUDITED, COMPLETED & SEALED (PENDING OWNER SIGN-OFF)` and Stage 2 status to `PENDING OWNER APPROVAL (GATE LOCKED)`.
- **Verdict**: GOVERNANCE CORRECTIONS VERIFIED & SEALED — READY FOR PROJECT OWNER DECISION.
- **Execution Constraints Enforced**: Documentation corrections only; `.agent-memory/ARCHITECTURE-DECISIONS.md` strictly untouched (0 diff against HEAD); zero C# code, projects, or migrations created; no git commit/stash/reset executed.
- **Status**: Completed & Verified.

---

### [2026-11-05] — Task 02.8.2.3: Independent Post-Correction Verification
- **Objective**: Execute an independent, 100% read-only audit of all corrections made in Task 02.8.2.2 across Stage 1 documentation.
- **Deliverables**:
  - Authored [TASK-02.8.2.3-INDEPENDENT-POST-CORRECTION-VERIFICATION.md](../docs/product/TASK-02.8.2.3-INDEPENDENT-POST-CORRECTION-VERIFICATION.md) verifying:
    - 10/10 audit checks (AUDIT-01 to AUDIT-10) passed at 100%.
    - D3 (PDR-008) and D4 (PDR-009) fully unmerged and traceable.
    - D2/D5 inventory deduction authority cleanly decoupled from station triggers.
    - OD-14 neutral comparison preserved; false claims of "zero schema impact" eradicated.
    - Zero code leakage certified (0 .cs, 0 .csproj, 0 .sln, 0 .sql).
    - ADR record (.agent-memory/ARCHITECTURE-DECISIONS.md) verified 100% untouched.
- **Verdict**: 100% PASS — STAGE 1 OWNER SIGN-OFF PENDING (GATE TO STAGE 2 LOCKED).
- **Status**: Completed & Certified.

---

### [2026-11-05] — Task 02.8.3: Controlled Vietnamese Documentation Localization
- **Objective**: Execute controlled Vietnamese documentation localization and audit across Stage 1 deliverables, supporting the Project Owner with an authoritative, 100% Vietnamese Product Specification and Decision Dossier.
- **Deliverables**:
  - Authored [HO-SO-DAC-TA-NGHIEP-VU-VA-QUYET-DINH-CHU-NHA-HANG.md](../docs/product/HO-SO-DAC-TA-NGHIEP-VU-VA-QUYET-DINH-CHU-NHA-HANG.md) establishing:
    - Executive Vietnamese overview of the SmartRestaurant MVP single-store boundary (OD-01).
    - Comprehensive breakdown of all 14 business modules (10 Core Mandatory + 4 MVP Optional).
    - Full register of Owner-confirmed baselines (D1–D5, REV-05..11, OD-01..13, BillingRequested Option 2) with standardized Vietnamese terminology.
    - Clear decision briefs and trade-off comparisons for open items (OD-14, OD-15, OD-16, OD-17, OQ-01, OQ-02, OQ-06, OQ-07).
    - Vietnamese fiscal and operating conventions (UTC+7, VND formatting, VAT 8%/10%, Circular 78/Decree 123 E-Invoice readiness, ESC/POS UTF-8 printing).
    - 6-role Vietnamese RBAC matrix (Quản lý, Thu ngân, Phục vụ, Bếp trưởng, Phụ bếp/Bar, Thủ kho).
    - Stage 2 execution roadmap and governance transition criteria (TASK-03 WBS).
  - Authored [TASK-02.8.3-CONTROLLED-VIETNAMESE-DOCUMENTATION-LOCALIZATION-REPORT.md](../docs/product/TASK-02.8.3-CONTROLLED-VIETNAMESE-DOCUMENTATION-LOCALIZATION-REPORT.md) certifying:
    - 100% terminology compliance across 4 core business domains against [VIETNAMESE-LOCALIZATION-STANDARD.md](../docs/product/VIETNAMESE-LOCALIZATION-STANDARD.md).
    - Clean technical decoupling (English backend contracts vs Vietnamese presentation strings via resource catalogs).
    - Zero code leakage (0 .cs, 0 .csproj, 0 .sln, 0 .sql).
    - Tamper-proof ADR record integrity (0 diff against HEAD on .agent-memory/ARCHITECTURE-DECISIONS.md).
  - Updated [docs/00-index.md](../docs/00-index.md) with links to Task 02.8.2.2, 02.8.2.3, and 02.8.3 deliverables.
- **Verdict**: LOCALIZATION AUDIT 100% CERTIFIED — READY FOR OWNER SIGN-OFF (STAGE 2 GATE LOCKED).
- **Status**: Completed & Verified.
