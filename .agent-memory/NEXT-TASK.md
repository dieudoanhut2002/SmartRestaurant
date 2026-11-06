# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-03 (LOCKED / NOT AUTHORIZED)
- **Task Title**: Stage 2 Technical Architecture Design (Awaiting Explicit Owner Authorization)
- **Lifecycle Stage**: Stage 2 — System Architecture & Technical Design
- **Current Status**: **STAGE 1 = OWNER-APPROVED; STAGE 2 = NOT AUTHORIZED / LOCKED**. Chủ Dự Án đã chính thức phê duyệt nghiệm thu Stage 1 vào ngày 2026-11-06 nhưng chưa ủy quyền khởi động Stage 2. Tuyệt đối không được khởi động Task 03 nếu chưa có ủy quyền riêng của Chủ Dự Án. Toàn bộ hệ thống duy trì chế độ chờ (standby).

---

## 1. Objective
Following formal sign-off of Stage 1 (Product Discovery & Specifications Suite `v1.0.0-FROZEN`), transition to **Stage 2: System Architecture & Technical Design** ONLY upon explicit instruction from the Project Owner. When instructed, define the Modular Monolith solution structure, Entity Framework Core relational data models (ERD), multi-tenant/branch isolation patterns, and real-time SignalR hub contracts.

---

## 2. Scope (To be executed ONLY upon explicit Owner trigger)
- Reconcile user feedback on Open Questions (`OQ-01`, `OQ-02`, `OQ-03`).
- Author `docs/02-architecture/system-architecture.md` (Modular Monolith layers, project structure, inter-module interfaces, domain events).
- Author `docs/02-architecture/data-models-and-erd.md` (Relational schema, entity definitions, indexes, concurrency tokens, foreign key cascades).
- Author `docs/02-architecture/realtime-signalr-mesh.md` (Hub contracts, event payload definitions, client listener interfaces for POS, KDS, Floor Plan).
- Update `.agent-memory/` records accordingly.
- **Strict Boundary**: Do not write application source code, scaffold .NET projects, or apply database migrations during this task. Currently in STRICT STANDBY: zero architectural files to be written until specifically triggered by the Project Owner.

---

## 3. Required Inputs
- Approved [Product Requirements Document Suite](../docs/product/).
- Certified [Final Governance Traceability & Approval Boundary Audit](../docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md).
- [Module Configuration & Business Feature Management Analysis](../docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md).
- Certified [Module Governance & Business Rule Reconciliation Audit](../docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md).
- Approved [Owner Decision Dossier & Stage 2 Readiness Assessment](../docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md).
- Certified [Final Evidence Reconciliation & Governance Boundary Verification](../docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md).
- Certified [Independent Vietnamese Documentation Quality Verification](../docs/product/TASK-02.8.4-INDEPENDENT-VIETNAMESE-QUALITY-VERIFICATION.md).
- Approved [Project Owner Decision Consolidation Record](../docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md) (100% of open questions `OD-14`..`OD-17`, `OQ-01`..`OQ-07`, and 14 Module Framework officially confirmed).
- Certified [Owner Decision Implementation Report](../docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md) (Stage 1 documentation 100% synchronized; Stage 2 Gate NOT AUTHORIZED / LOCKED per Owner directive).
- Sealed [Product Specification Baseline Suite](../docs/product/PRODUCT-SPECIFICATION-BASELINE.md) (Master Baseline Specification `v1.0.0-FROZEN`).
- Official [Stage 2 Product Handoff Package](../docs/product/STAGE-2-PRODUCT-HANDOFF.md) for Solution Architect.
- Architectural guidelines established in [GEMINI.md](../GEMINI.md) and [AGENTS.md](../AGENTS.md).

---

## 4. Relevant Documentation
- [docs/00-index.md](../docs/00-index.md)
- [docs/product/PRODUCT-SPECIFICATION-BASELINE.md](../docs/product/PRODUCT-SPECIFICATION-BASELINE.md)
- [docs/product/STAGE-2-PRODUCT-HANDOFF.md](../docs/product/STAGE-2-PRODUCT-HANDOFF.md)
- [docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md](../docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md)
- [docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md](../docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md)
- [docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md](../docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md)
- [docs/product/OWNER-DECISION-WORKSHOP.md](../docs/product/OWNER-DECISION-WORKSHOP.md)
- [docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md](../docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md)
- [docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md](../docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md)
- [docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md](../docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md)
- [docs/product/PRD.md](../docs/product/PRD.md)
- [docs/product/MODULE-OVERVIEW.md](../docs/product/MODULE-OVERVIEW.md)
- [docs/product/decisions/](../docs/product/decisions/)

---

## 5. Expected Deliverables
1. `docs/02-architecture/system-architecture.md`
2. `docs/02-architecture/data-models-and-erd.md`
3. `docs/02-architecture/realtime-signalr-mesh.md`
4. Updated `.agent-memory/` tracking records.

---

## 6. Acceptance Criteria
- Complete architectural blueprint strictly enforces Modular Monolith separation.
- Database ERD covers all entities identified across the 14 business modules.
- SignalR contracts specify strongly-typed hub client interfaces.
- Architecture design integrates Vietnamese-First localization decoupling (.resx resource catalogs, CultureInfo `vi-VN` default, zero hardcoded UI strings in C# domain/application layers).
- Zero business code or dummy classes created.

---

## 7. Approval Requirements
- **Gatekeeper Requirement**: Certified Stage 1 Sign-Off and Master Baseline Freeze via Task 02.7.5, Task 02.8.5, Task 02.8.6, Task 02.9 (`v1.0.0-FROZEN`), Task 02.9.2, and Task 02.8.6.1 satisfied (`STAGE 1 = OWNER-APPROVED`). Stage 2 transition gate is **`NOT AUTHORIZED / LOCKED`**. Commencing Task 03 requires explicit, separate authorization from the Project Owner. Entire system remains on STRICT STANDBY. Zero architectural files or code permitted until authorized.
