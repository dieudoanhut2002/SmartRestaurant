# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-03.0.1 (PENDING OWNER APPROVAL OF PLAN TASK 03.0)
- **Task Title**: Execute Stage 1 Document Consolidation & Archival
- **Lifecycle Stage**: Transition between Stage 1 and Stage 2
- **Current Status**: **STAGE 1 = OWNER-APPROVED (`v1.0.0-FROZEN`); STAGE 2 = NOT AUTHORIZED / LOCKED**. Kế hoạch hợp nhất và lưu trữ tài liệu Giai đoạn 1 đã hoàn tất tại [TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md](../docs/product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md). Chờ Chủ Dự Án phê duyệt phương án kiến trúc lưu trữ để tiến hành thực thi chuyển tệp bằng `git mv` và kiểm thử tự động liên kết. Cổng Giai đoạn 2 tiếp tục duy trì trạng thái KHÓA (LOCKED).

---

## 1. Objective
Upon Owner review and approval of the Consolidation & Archive Plan (Task 03.0), execute the physical relocation of 24 historical audit, verification, and workshop reports into `docs/product/archive/` using `git mv`, update the 191 cross-links, generate `docs/product/archive/README.md`, streamline `docs/00-index.md`, and run automated link verification to guarantee 0 broken links.

---

## 2. Scope
1. **Directory Scaffolding**: Create `docs/product/archive/01-reviews-and-reconciliation/`, `02-owner-decision-workshops/`, and `03-baseline-and-governance-audits/`.
2. **Git Relocation**: Execute `git mv` on all 24 historical reports and the legacy scaffolding PRD draft per the 1:1 migration matrix in Task 03.0. Clean up empty `docs/01-requirements/`.
3. **Index Creation**: Author `docs/product/archive/README.md` cataloging all archived records with audit verdicts and context.
4. **Link Normalization**: Surgically update 191 cross-links across the codebase to point to new archive paths.
5. **Index Streamlining**: Update `docs/00-index.md` to present a lean, clean active catalog.
6. **Automated Verification**: Run PowerShell link verification script; certify **0 broken links**.
7. **Strict Boundary**: Zero C# code, zero .NET scaffolding, zero database migrations. Stage 2 Gate remains strictly LOCKED until explicit Owner instruction.

---

## 3. Required Inputs
- Approved [Stage 1 Document Consolidation & Archive Plan](../docs/product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md).
- Master Baseline Suite (`v1.0.0-FROZEN`) at [docs/product/PRODUCT-SPECIFICATION-BASELINE.md](../docs/product/PRODUCT-SPECIFICATION-BASELINE.md).
- Architectural guidelines established in [GEMINI.md](../GEMINI.md) and [AGENTS.md](../AGENTS.md).

---

## 4. Relevant Documentation
- [docs/product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md](../docs/product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md)
- [docs/00-index.md](../docs/00-index.md)
- [docs/product/PRODUCT-SPECIFICATION-BASELINE.md](../docs/product/PRODUCT-SPECIFICATION-BASELINE.md)
- [docs/product/STAGE-2-PRODUCT-HANDOFF.md](../docs/product/STAGE-2-PRODUCT-HANDOFF.md)

---

## 5. Expected Deliverables
1. Reorganized repository structure with `docs/product/archive/` hierarchy containing 24 archived reports.
2. Authoritative index `docs/product/archive/README.md`.
3. Streamlined `docs/00-index.md`.
4. Automated verification test results certifying 0 broken links.
5. Task Completion Report (`TASK-03.0.1-EXECUTION-REPORT.md`).

---

## 6. Acceptance Criteria
- 100% of the 24 historical files preserved via `git mv` (zero data loss, full git history retained).
- All 191 links updated and verified; automated test returns `PASS (0 broken links)`.
- Active working set in `docs/product/` reduced from 41 files down to 18 active living documents.
- The 8 Baseline documents (`v1.0.0-FROZEN`) retain 100% invariant meaning.
- Zero code written; Stage 2 Gate remains `NOT AUTHORIZED / LOCKED`.

---

## 7. Approval Requirements
- Owner confirmation of Option 1 (In-Place Structured Archive) in Task 03.0.
- Commencing Stage 2 Technical Architecture (TASK-03) remains strictly on STANDBY / LOCKED pending separate explicit Owner directive.
