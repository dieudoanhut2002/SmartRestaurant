# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-03
- **Task Title**: Stage 1 Sign-Off & Stage 2 Technical Architecture Design
- **Lifecycle Stage**: Stage 2 — System Architecture & Technical Design

---

## 1. Objective
Following user review and formal approval of the Stage 1 Product Requirements Suite (`docs/product/`), transition to **Stage 2: System Architecture & Technical Design**. Define the Modular Monolith solution structure, Entity Framework Core relational data models (ERD), multi-tenant/branch isolation patterns, and real-time SignalR hub contracts.

---

## 2. Scope
- Reconcile user feedback on Open Questions (`OQ-01`, `OQ-02`, `OQ-03`).
- Author `docs/02-architecture/system-architecture.md` (Modular Monolith layers, project structure, inter-module interfaces, domain events).
- Author `docs/02-architecture/data-models-and-erd.md` (Relational schema, entity definitions, indexes, concurrency tokens, foreign key cascades).
- Author `docs/02-architecture/realtime-signalr-mesh.md` (Hub contracts, event payload definitions, client listener interfaces for POS, KDS, Floor Plan).
- Update `.agent-memory/` records accordingly.
- **Strict Boundary**: Do not write application source code, scaffold .NET projects, or apply database migrations during this task.

---

## 3. Required Inputs
- Approved [Product Requirements Document Suite](../docs/product/).
- Certified [Final Governance Traceability & Approval Boundary Audit](../docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md).
- [Module Configuration & Business Feature Management Analysis](../docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md).
- Certified [Module Governance & Business Rule Reconciliation Audit](../docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md).
- Approved [Owner Decision Dossier & Stage 2 Readiness Assessment](../docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md).
- Certified [Final Evidence Reconciliation & Governance Boundary Verification](../docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md).
- User confirmation on open questions (`OD-14`..`OD-17`, `OQ-01`..`OQ-07`).
- Architectural guidelines established in [GEMINI.md](../GEMINI.md) and [AGENTS.md](../AGENTS.md).

---

## 4. Relevant Documentation
- [docs/00-index.md](../docs/00-index.md)
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
- **Gatekeeper Requirement**: Certified Stage 1 Sign-Off via Task 02.7.5 Final Audit (`FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md`) satisfied. Ready for immediate owner authorization to execute.
