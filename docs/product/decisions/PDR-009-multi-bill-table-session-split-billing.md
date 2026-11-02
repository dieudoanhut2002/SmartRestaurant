# Product Decision Record: PDR-009 — Multi-Bill Table Session and Flexible Split Billing

- **Decision ID**: PDR-009
- **Title**: Multi-Bill per Table Session and Flexible Split Billing Policy (Without Table Locking)
- **Status**: `CONFIRMED` (Business Requirements Approved; Database & Concurrency Mechanics: `PROPOSED`)
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

### 2.2. Uninterrupted Ordering, Editable `BillingRequested`, and Default Appending:
- **No Table Freeze**: Tables undergoing check presentation (`BillingRequested`) or split billing are NEVER frozen.
- **Editable Unpaid Bills**: When a table is in `BillingRequested` status, the existing unpaid bill remains fully editable. Staff may add, remove, or modify items, subject to standard permission controls and audit logging.
- **Default Appending vs Sub-Bill Creation**: When guests order additional items after the pre-bill has been printed:
  - **Default Behavior**: Newly ordered items append directly to the existing open unpaid bill.
  - **Explicit Sub-Bill Creation**: Staff may explicitly choose to route new items into a newly created sub-bill (e.g., Sub-Bill 2) when separate billing is requested.
- **Multiple Concurrent Sub-Bills**: Multiple sub-bills can coexist within the same table session and be settled independently.

### 2.3. Independent Settlement:
- Each sub-bill can be settled independently with its own payment method (Cash, Card, VietQR).
- Settling Sub-Bill 1 changes its status to `Paid` and prints an individual receipt. The physical table remains `Occupied` or `BillingRequested` as long as any sub-bill remains unpaid.

### 2.4. Cancellation of Bill Split:
- The cashier is permitted to cancel a split bill operation and consolidate all items back into a single invoice **only if no sub-bill in that session has incurred payment**.
- Once any sub-bill has been partially or fully settled, the split structure is locked against wholesale rollback.

### 2.5. Table Release Conditions (Zero Open Obligations):
A table session is eligible to be released to `CleaningNeeded` or `Available` ONLY when:
1. All sub-bills in the session have attained `Paid` (or legitimately `Cancelled`) status.
2. All ordered dishes have reached terminal preparation and service states (`Served` or cancelled with waste audit).
3. No open culinary or financial obligations remain on the table session.

### 2.6. Strict Prohibition of Direct Modification on Paid Bills:
- **Paid Bill Immutability**: Under no circumstances does this decision permit directly modifying or altering line items on bills that have already attained `Paid` status.
- Once a bill/sub-bill is paid, it is financially immutable. Any post-settlement correction or refund must follow formal supervisor void/refund protocols.

### 2.7. Audit Logging:
- System logs full history of item allocation across sub-bills, modifications made during `BillingRequested`, cashier transfers, settlement events, and any split cancellation actions.

---

## 3. Consequences
- **Positive**: Seamless guest experience; patrons who stay can keep ordering without friction; mathematically watertight checkout.
- **Negative**: Data model must support 1-to-N relationship between `TableSession` and `Invoices`.

> [!NOTE] Scope Classification & Confirmation Boundaries
> - **Confirmed Business Requirements**: Per Owner decision OD-13 (D4 / REV-04), multi-bill table session, zero table freezing during check presentation or split, editable unpaid bills during `BillingRequested`, newly ordered items appending to existing bill by default or creating a new sub-bill, paid bill immutability, and zero open obligation table release are **CONFIRMED**.
> - **Technical Proposal**: Relational sub-bill transaction isolation, database concurrency tokens, and cashier multi-check management screens are **PROPOSED** technical designs subject to Stage 2 Solution Architect review.
