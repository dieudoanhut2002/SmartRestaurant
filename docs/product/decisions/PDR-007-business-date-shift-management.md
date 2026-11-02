# Product Decision Record: PDR-007 — Business Date for Overnight Shift Management

- **Decision ID**: PDR-007
- **Title**: Adoption of Configurable Business Date (Ngày Kinh Doanh) for Overnight Shifts and Financial Accounting
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Restaurants, bars, beer clubs, and late-night dining venues in Vietnam frequently operate past midnight (e.g., 17:00 to 02:00 or 04:00 AM). Binding operational records directly to calendar dates (`DateTime.UtcNow` converted to `dd/MM/yyyy`) splits overnight shifts at 00:00 midnight, fragmenting revenue, category sales, staff shift blind balancing, and inventory consumption across two disconnected calendar days.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Configurable Branch-Level Cutoff Time:
- Each branch has an independently configurable cutoff time property: `BusinessDayCutoffTime` (default: `06:00:00` AM).
- Operating cycle for Business Date $T$ spans from `CutoffTime` on day $T$ to `CutoffTime - 1 second` on day $T+1$ (e.g., `06:00:00` 10/10/2026 to `05:59:59` 11/10/2026).

### 2.2. Immutable Order Business Date Binding:
- When a dining session or order is initialized, the system evaluates the branch's current `BusinessDate` at creation time and binds it statically to the `Order` record.
- **Orders spanning cutoff**: An order opened before cutoff (e.g., 23:30) and settled after cutoff (e.g., 06:45 AM) remains strictly attributed to the `BusinessDate` assigned at creation.
- **Audit Timestamp Distinction**: The system rigorously separates financial partition date (`BusinessDate: DateOnly`) from physical UTC timestamps (`CreatedAt: DateTimeOffset`, `PaidAt: DateTimeOffset`).
- **Configuration Change Immutability**: If a manager modifies `BusinessDayCutoffTime`, existing orders, settled invoices, and closed shifts NEVER change their assigned `BusinessDate` retrospectively.

### 2.3. Shift & Financial Alignment:
- Staff shifts (`ShiftSession`) capture blind cash drawers against all transactions sharing the shift's operational boundary.
- Daily financial reporting and Z-Reports aggregate strictly by `BusinessDate`.

---

## 3. Open Operational Scenarios & Architectural Notes
- **Multi-day / continuous dining sessions (> 24 hours)**: In the rare event a table session remains open across multiple cutoff transitions, it retains its original creation `BusinessDate`, but generates an operational alert for manager review to prevent stale session accumulation.
- **Architectural Impact**: Database indexes on `Orders`, `Payments`, and `ShiftSessions` must include `(BranchId, BusinessDate)` to optimize daily operational queries.

---

## 4. Consequences
- **Positive**: 100% accurate financial reporting for late-night venues; flawless cash drawer balancing without calendar cutoff friction.
- **Negative**: Requires reporting queries to aggregate by `BusinessDate` instead of SQL `CAST(CreatedAt AS DATE)`.
