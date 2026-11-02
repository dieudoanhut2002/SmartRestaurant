# Product Decision Record: PDR-012 — Direct Front-of-House Service Items Policy

- **Decision ID**: PDR-012
- **Title**: Routing Policy for Direct Front-of-House Service Items Bypassing Kitchen Display Systems
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Packaged cold drinks (canned beer, bottled water, soft drinks) or pre-packaged grab-and-go items are stored directly in dining floor refrigerators and fetched immediately by front-of-house (FOH) servers. Routing these simple items to the Bar or Kitchen KDS clutters prep station screens with unnecessary tickets and delays service.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Direct Service Configuration:
- Menu items or item categories can be configured with the routing property: `ServiceRouting = DirectFOHPick` (Phục vụ trực tiếp tại sảnh) vs `StationRouting` (Gửi trạm chế biến Bếp/Bar KDS).

### 2.2. Operational & Financial Safeguards:
- **No KDS Routing**: When a server fires an order containing direct service items, these items bypass the Bar KDS preparation queue entirely.
- **Immediate State Transition**: Direct items transition directly to `ReadyToServe / Dispensed` upon order confirmation. If floor printing is configured, an item pick slip prints at the local floor service station.
- **Strict Financial & Inventory Inclusion**: Bypassing KDS preparation DOES NOT bypass billing or inventory control. Direct service items are strictly included in order calculation, customer invoices, revenue reporting, and standard inventory reservation/deduction rules.

---

## 3. Consequences
- **Positive**: De-clutters Bar KDS screens; significantly speeds up beverage service during rush hours.
- **Negative**: Requires servers to be disciplined in fetching items promptly without relying on KDS ready notifications.
