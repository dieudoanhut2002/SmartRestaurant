# Product Decision Record: PDR-004 — Inventory Recipe BOM Deduction Trigger Policy

- **Decision ID**: PDR-004
- **Title**: Timing Policy for Raw Ingredient Inventory Deduction (BOM)
- **Status**: `PROPOSED` (Pending User Approval)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Restaurants track raw materials using Recipe Bill of Materials (BOM). When a customer orders a dish, its raw ingredients must be deducted from the inventory balance. The fundamental question is: **At what exact point in the order lifecycle should raw materials be deducted from stock?**

---

## 2. Options Considered

### Option A: Immediate Deduction on Waiter Order Firing
- Deduct stock immediately when the server taps "Fire to Kitchen".
- *Pros*: Simple to implement; immediate stock availability reflection.
- *Cons*: High error rate if an order is cancelled or modified before cooking begins. If cancelled, inventory must be reverse-engineered and re-credited, leading to phantom stock discrepancies and inaccurate waste tracking.

### Option B: Deduction on Cashier Bill Settlement
- Deduct stock only when the guest pays their bill at checkout.
- *Pros*: Only paid food is accounted for.
- *Cons*: Fails completely to account for food consumed during long dining sessions; completely ignores food cooked but voided/wasted due to guest walkouts or kitchen errors.

### Option C: Deduction on Kitchen Readiness (`ReadyToServe`) with Scrap Attribution (Recommended)
- Raw ingredient deduction triggers at the moment the kitchen station marks the dish `ReadyToServe` (or upon entering `Cooking` state).
- If an item is cancelled while in `Pending` (before cooking starts), zero stock is deducted and no waste occurs.
- If an item is cancelled after entering `Cooking` or `ReadyToServe`, the raw materials remain deducted and are automatically logged into the `WasteLog` table as scrap loss.
- *Pros*: Reflects physical culinary reality with 100% accuracy; accurately captures Cost of Goods Sold (COGS) vs Operational Spoilage/Waste.
- *Cons*: Requires tight coupling between KDS status transitions and the inventory engine.

---

## 3. Recommendation & Decision
**Adopt Option C (Deduction on Kitchen Readiness with Scrap Attribution)**.

---

## 4. Consequences
- **Positive**: Eliminates phantom inventory counts; provides accurate operational loss reporting; gives owners actionable insight into kitchen waste causes.
- **Negative**: Cooks must reliably acknowledge dishes on the KDS screen to keep inventory synchronized.

---

## 5. Questions & Assumptions
- *Question*: `OQ-03` asks whether cancelled dishes can occasionally be repurposed by managers rather than written off as pure scrap.
