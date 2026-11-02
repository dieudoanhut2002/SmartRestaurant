# Product Decision Record: PDR-009 — Multi-Bill Table Session and Flexible Split Billing

- **Decision ID**: PDR-009
- **Title**: Multi-Bill per Table Session and Flexible Split Billing Policy (Without Table Locking)
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
When dining parties request to split a check, some guests pay their portion and depart early while others remain seated and continue ordering. A previous proposal suggested freezing the entire table during split billing, but this was rejected because it halts normal guest service and table turnover.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Multi-Bill Table Session Model:
- A single active dining session (`TableSession`) can host multiple independent sub-bills (`SubBill` / `Invoice`).
- Splitting a bill distributes existing line items across sub-bills without locking the table.

### 2.2. Uninterrupted Ordering with Explicit Target Assignment:
- **No Table Freeze**: Servers can continue to add dishes or drinks to the active table session while split billing is in progress.
- **Explicit Item Assignment**: When adding new dishes, the server tablet must explicitly prompt for or display the target unpaid sub-bill (e.g., Sub-Bill 2, Sub-Bill 3, or "Create New Sub-Bill"). The system NEVER assigns new items arbitrarily.

### 2.3. Independent Settlement:
- Each sub-bill can be settled independently with its own payment method (Cash, Card, VietQR).
- Settling Sub-Bill 1 changes its status to `Paid` and prints an individual receipt. The physical table remains `Occupied` because Sub-Bill 2 remains unpaid.

### 2.4. Cancellation of Bill Split:
- The cashier is permitted to cancel a split bill operation and consolidate all items back into a single invoice **only if no sub-bill in that session has incurred payment**.
- Once any sub-bill has been partially or fully settled, the split structure is locked against wholesale rollback.

### 2.5. Table Release Conditions (Zero Open Obligations):
A table session is eligible to be released to `CleaningNeeded` or `Available` ONLY when:
1. All sub-bills in the session have attained `Paid` (or legitimately `Cancelled`) status.
2. All ordered dishes have reached terminal preparation and service states (`Served` or cancelled with waste audit).
3. No open culinary or financial obligations remain on the table session.

### 2.6. Audit Logging:
- System logs full history of item allocation across sub-bills, cashier transfers, settlement events, and any split cancellation actions.

---

## 3. Consequences
- **Positive**: Seamless guest experience; patrons who stay can keep ordering without friction; mathematically watertight checkout.
- **Negative**: Data model must support 1-to-N relationship between `TableSession` and `Invoices`.
