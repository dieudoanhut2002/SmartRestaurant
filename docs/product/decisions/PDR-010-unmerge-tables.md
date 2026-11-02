# Product Decision Record: PDR-010 — Table Unmerge and Item Reassignment Policy

- **Decision ID**: PDR-010
- **Title**: Policy for Unmerging Previously Merged Tables with Item Reallocation
- **Status**: `CONFIRMED` (Business Requirements Approved; Technical Implementation Mechanics: `PROPOSED`)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
When two or more tables have been merged into a single master dining session (e.g., Table 04 and Table 05), real-world dining scenarios often require unmerging them (e.g., accidental merge by staff, or part of a party seated at Table 05 decides to leave earlier and settle separately).

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Unmerge Capability:
- The system supports unmerging joined tables back into independent physical table sessions at any stage of service.

### 2.2. Item Allocation & Ticket Integrity:
- **Prior History Preservation**: When unmerging, the cashier or manager can select which ordered items belong to the detached table (e.g., Table 05).
- **No Duplicate Ticket Generation**: The unmerge action NEVER regenerates kitchen tickets or sends duplicate preparation orders to KDS for dishes that were already fired.
- **State Integrity**: All existing item preparation states (`Cooking`, `ReadyToServe`, `Served`), kitchen station assignments, and timestamp histories are strictly preserved without disruption.
- **Handling Pre-existing Items**: If Table 05 had its own active items prior to being merged with Table 04, those original items and tickets must be cleanly reassigned back to Table 05's restored session.

### 2.3. Independent Table Lifecycle:
- Once unmerged, Table 05 becomes an independent `TableSession` capable of taking new orders, printing provisional checks, settling bills, and releasing to `CleaningNeeded` without impacting Table 04.

---

## 3. Consequences
- **Positive**: High operational flexibility for banquet and group dining; prevents manual workarounds that corrupt sales statistics.
- **Negative**: Requires cashier interface for intuitive drag-and-drop or checklist reallocation of line items between unmerged tables.

> [!NOTE] Scope Classification & Confirmation Boundaries
> - **Confirmed Business Requirements**: Per Owner decision OD-06 (REV-05), unmerging joined tables at any stage of service, preserving existing item preparation states, zero duplicate kitchen ticket generation, and independent table session restoration are **CONFIRMED**.
> - **Technical Proposal**: The specific entity reallocation schema and UI drag-and-drop mechanics are **PROPOSED** technical designs subject to Stage 2 Solution Architect review.
