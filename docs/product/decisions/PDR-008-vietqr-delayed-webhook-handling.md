# Product Decision Record: PDR-008 — VietQR Delayed Webhook Resolution and Suspense Reconciliation

- **Decision ID**: PDR-008
- **Title**: Resolution Workflow for Delayed VietQR Webhooks Following Cash Settlement
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
During peak checkout periods, banking switch latency (Napas 24/7) or mobile network issues can delay VietQR confirmation webhooks. If an impatient customer opts to switch to Cash settlement and leaves, a bank webhook arriving minutes later creates a complex multi-tender discrepancy.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Cash Fallback upon QR Timeout:
- If a VietQR payment request remains unconfirmed past the configured threshold, the cashier is permitted to accept Cash payment and finalize the bill.
- The pending QR payment request is transitioned to `CancelledByCashier / CashFallback`.

### 2.2. Suspense Account Routing for Delayed Webhooks:
- When a bank webhook confirms receipt of funds referencing a cancelled QR token or a cash-settled invoice, the transaction is **routed to a "Giao dịch treo đối soát" (Suspense Reconciliation Queue)**.
- **Immediate Audio-Visual Alert**: The system broadcasts an instant high-priority alert to Cashier and Manager screens.

### 2.3. Anti-Duplicate & Verification Controls:
- **Reference & Bank Txn Matching**: Every incoming webhook must be validated using the unique bank transaction ID (`BankTransactionId`) and internal payment reference (`PaymentRef`) to prevent duplicate crediting.
- **No Automatic Double Crediting**: System NEVER automatically adds duplicate funds to an invoice that has already been closed with cash.
- **No Automatic Unverified Refunds**: System NEVER automatically initiates bank refunds solely based on an unverified webhook payload without manual review.

### 2.4. Manual Resolution Workflow with Full Audit Trail:
- Authorized managers review suspense transactions with four governed resolution actions:
  1. **Manual Match (Gán vào hóa đơn)**: Associate with an unpaid or underpaid invoice if payment was misallocated.
  2. **Unallocated Revenue (Ghi nhận tiền chưa phân bổ)**: Record in suspense ledger pending customer claim.
  3. **Approved Cash/Bank Refund (Hoàn tiền theo quy trình)**: Execute refund with documented manager approval and reason.
- All webhook payloads, matching attempts, and reconciliation actions must be logged in an immutable audit table.

---

## 3. Consequences
- **Positive**: Eliminates cash drawer discrepancies; prevents financial leakage and double taxation; ensures regulatory auditability.
- **Negative**: Requires cashier/manager training for suspense ledger reconciliation.
