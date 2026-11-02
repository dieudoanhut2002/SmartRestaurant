# Product Decision Record: PDR-015 — Remake Dish Workflow for Culinary Errors

- **Decision ID**: PDR-015
- **Title**: Remake Dish Workflow and Waste Logging Policy for Kitchen and Service Errors
- **Status**: `CONFIRMED` (Business Requirements Approved; KDS UI Presentation: `PROPOSED`)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
When a prepared dish is rejected at the table due to kitchen error (e.g., burned steak, over-salted soup, allergen oversight, wrong side dish), standard POS workflows forcing servers to cancel and re-order cause billing mistakes (customer charged twice) or distort kitchen inventory reports.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Customer Bill Protection:
- A remake dish operation for kitchen error NEVER incurs an additional charge on the customer's invoice. The line item quantity and subtotal on the guest bill remain unaffected.

### 2.2. Dedicated Remake Identification on KDS:
- Remake orders trigger a high-priority preparation ticket on the appropriate KDS station with prominent visual branding:
  - Label: **"LÀM LẠI DO LỖI BẾP"** (Remake - Kitchen Error).
  - Explicit reason displayed (e.g., Quá mặn, Khét, Dị vật, Nhầm món).

### 2.3. Waste Logging & Inventory Consumption:
- The system automatically logs the discarded portion into the **Sổ Hủy Hỏng (Waste Log)** to account for raw material consumption and kitchen accountability.
- Inventory reservation/consumption is recorded for the replacement portion to maintain physical inventory alignment.

### 2.4. Audit Trail & Distinction from Add-on Orders:
- System logs: Requester, Authorizing Manager, Timestamp, Error Category, and Responsible Kitchen Department.
- Remake operations are strictly distinguished from genuine customer re-orders or supplementary orders.

---

## 3. Consequences
- **Positive**: Transparent kitchen operations; protects customer bill accuracy; provides data for kitchen quality control.
- **Negative**: POS UI must provide a secure, governed action for remake entry with mandatory reason selection.

> [!NOTE] Scope Classification & Confirmation Boundaries
> - **Confirmed Business Requirements**: Per Owner decision OD-12 (REV-11), zero extra customer charge, high-priority remake ticket dispatch, scrap loss logging (`ScrapLoss`) with manager authorization, and audit distinction from add-on orders are **CONFIRMED**.
> - **Proposed UI Specification**: The specific visual theme (e.g., purple ticket card styling, flashing border, alert chime sound frequency) is an **AI UI proposal** subject to Stage 2 UI/UX design.
