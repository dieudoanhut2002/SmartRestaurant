# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-04.0 (PENDING EXPLICIT OWNER AUTHORIZATION FOR STAGE 2)
- **Task Title**: Stage 2 System Architecture & Technical Design Kickoff
- **Lifecycle Stage**: Transition to Stage 2 (System Architecture & Technical Design)
- **Current Status**: **STAGE 1 = OWNER-APPROVED (`v1.0.0-FROZEN`) & FULLY CONSOLIDATED; STAGE 2 = NOT AUTHORIZED / LOCKED**. Nhiệm vụ hợp nhất và lưu trữ tài liệu Giai đoạn 1 (Task 03.1) đã hoàn thành xuất sắc với 100% bằng chứng lịch sử được lưu trữ an toàn, 0 liên kết hỏng, và 2 tài liệu chuẩn tắc tối thượng ([docs/PROJECT-GOVERNANCE.md](../docs/PROJECT-GOVERNANCE.md) và [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md)) đã sẵn sàng. Toàn bộ hệ thống hiện đang ở trạng thái **CHỜ LỆNH (STRICT STANDBY)**, chờ Chủ Dự Án chính thức phát lệnh mở khóa Cổng Giai đoạn 2 (Stage 2 Gate).

---

## 1. Objective
Upon explicit written authorization from the Project Owner to unlock Stage 2, commence Stage 2 Technical Architecture & System Design for SmartRestaurant based strictly upon the canonical specifications in `docs/PRODUCT-BASELINE-v1.0.0.md` and `docs/PROJECT-GOVERNANCE.md`.

---

## 2. Scope
1. **Architecture Blueprinting**: Design high-level Modular Monolith solution architecture (`SmartRestaurant.Domain`, `SmartRestaurant.Application`, `SmartRestaurant.Infrastructure`, `SmartRestaurant.Web`).
2. **Domain Entity & Relationship Modeling**: Formulate core domain entities, Value Objects, and ERD schemas adhering to clean architecture rules in `AGENTS.md`.
3. **SignalR Real-Time Contract Design**: Design hub contracts for Table status broadcasting, KDS multi-station routing, and POS updates.
4. **Integration Protocols**: Formulate VietQR payment callback, printer spooling abstractions, and background worker job scheduling.
5. **Technical Decision Formalization**: Document Architectural Decision Records (ADRs) for pending technical proposals (`TECH-01` through `TECH-07`, including `TECH-02` Inventory Deduction Trigger).
6. **Strict Boundary**: Zero application code (.cs), zero project scaffolding (.csproj, .sln), zero database migrations (.sql) until Stage 2 technical design is reviewed and approved by the Project Owner.

---

## 3. Required Inputs
- [docs/PROJECT-GOVERNANCE.md](../docs/PROJECT-GOVERNANCE.md) (Governance, authority boundaries, source-of-truth rules).
- [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md) (Sole authoritative Stage 1 product specification).
- [GEMINI.md](../GEMINI.md) & [AGENTS.md](../AGENTS.md) (Engineering standards and architectural rules).
- Explicit written instruction from the Project Owner authorizing Stage 2 commencement.

---

## 4. Relevant Documentation
- [docs/00-index.md](../docs/00-index.md)
- [docs/PROJECT-GOVERNANCE.md](../docs/PROJECT-GOVERNANCE.md)
- [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md)
- [docs/99-archive/stage-1-history/README.md](../docs/99-archive/stage-1-history/README.md) (Archive reference if forensic lookup needed).

---

## 5. Expected Deliverables
1. Stage 2 System Architecture Specification under `docs/02-architecture/`.
2. Domain Entity & ERD Data Model Specification.
3. SignalR Real-Time Hub & Protocol Specifications.
4. Formulated ADRs for `TECH-01` through `TECH-07`.
5. Stage 2 Kickoff Report and updated project memory.

---

## 6. Acceptance Criteria
- Explicit Owner directive authorizing Stage 2 received prior to creating any Stage 2 documents.
- 100% conformance to Master Baseline `PRODUCT-BASELINE-v1.0.0.md` (no deviation from approved business rules).
- Clean separation of concerns per Modular Monolith guidelines in `AGENTS.md`.
- Zero code leakage during technical architecture phase.

---

## 7. Approval Requirements
- **MANDATORY**: Explicit written authorization from the Project Owner to unlock Stage 2.
- Commencing Stage 2 without explicit Owner authorization is strictly prohibited by `GEMINI.md` and `PROJECT-GOVERNANCE.md`.
