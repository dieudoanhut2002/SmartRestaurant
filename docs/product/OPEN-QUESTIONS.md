# Open Business Questions & Decision Log — SmartRestaurant

- **Document Version**: 1.0.0
- **Document Status**: `OPEN`
- **Owner**: Senior Product Architect & Business Analyst

---

## 1. Executive Summary & Impact Classification

To maintain high development velocity while safeguarding architectural integrity, open questions are categorized into two explicit tiers:
- **TIER 1 — MVP DESIGN BLOCKERS**: Business logic decisions that directly affect core database entities, billing calculation algorithms, or state machines. Must be agreed upon before finalizing Stage 3 data models.
- **TIER 2 — NON-BLOCKING / POST-MVP INQUIRIES**: Decisions that influence future scaling, advanced edge cases, or release v1.1+ capabilities, which do not obstruct immediate MVP delivery.

---

## 2. Tier 1: MVP Design Blockers (Immediate Decision Required)

### OQ-01: Uneven Percentage-Based Bill Splitting
- **Context**: Group dining often involves splitting bills. SmartRestaurant MVP supports:
  1. *Split by Selected Items* (Guest A pays for dishes 1 & 2; Guest B pays for dish 3).
  2. *Split Evenly* (Total divided into $N$ equal checks, e.g. 50/50, 25/25/25/25).
- **The Question**: Does SmartRestaurant need to support arbitrary uneven percentage splits (e.g., 60% paid by Guest A, 40% paid by Guest B) or custom arbitrary monetary amount splits (e.g., Guest A pays 500k, Guest B pays the remaining 732k)?
- **Why It Matters**: Arbitrary monetary splits require sub-penny rounding reconciliation rules and complex tax recalculation algorithms across fractional items.
- **Recommended Default**: **Defer arbitrary percentage splits**. Support *Split by Item* and *Split Evenly ($1/N$)* in MVP. If guests want uneven amounts, the cashier can use *Split by Item* or process partial tender payments on a single bill (e.g., tender 500k Cash, then tender 732k VietQR on the same bill).
- **Status**: `OPEN QUESTION` (Awaiting Owner Confirmation)

---

### OQ-02: Simultaneous Physical Thermal Printing on KDS Order Fire
- **Context**: When a server fires an order from their tablet, items are pushed via SignalR to digital KDS screens.
- **The Question**: Should the system automatically trigger physical ESC/POS thermal printer chits in the kitchen simultaneously with every KDS ticket push, or should physical printing be an optional manual reprint fallback?
- **Why It Matters**: Dual-printing creates physical paper waste and can cause confusion (cooks checking both paper and screen). However, in high-heat or greasy kitchen sections (e.g. grill stations), some traditional chefs insist on physical paper chits pinned to order rails.
- **Recommended Default**: **Make physical kitchen ticket printing a configurable station setting** (`AutoPrintOnFire: true/false`). Default to `false` (pure digital KDS) with a 1-tap "Reprint Kitchen Chit" button on the KDS screen for emergency hardware fallback.
- **Status**: `OPEN QUESTION` (Awaiting Owner Confirmation)

---

### OQ-03: Waste Attribution on Post-Fire Dish Cancellations
- **Context**: If a customer cancels a dish after it has already entered `Cooking` or `ReadyToServe` status in the kitchen, the item is removed from the customer's payable bill.
- **The Question**: How should the system account for the already consumed raw materials? Should the system:
  - Option A: Automatically log the dish's recipe BOM into a `WasteRegister` as kitchen loss with a mandatory reason code?
  - Option B: Prompt the manager whether the dish can be repurposed (e.g. sold to another table, eaten by staff) or must be written off as scrap?
- **Why It Matters**: Directly impacts inventory variance and COGS (Cost of Goods Sold) accounting accuracy.
- **Recommended Default**: **Option B (Prompt with Smart Default)**. Default to logging as Kitchen Scrap/Waste, with an optional checkbox: "Item can be repurposed / Re-allocated to other table".
- **Status**: `OPEN QUESTION` (Awaiting Owner Confirmation)

---

## 3. Tier 2: Non-Blocking / Post-MVP Inquiries

### OQ-04: Multi-Branch Centralized Warehouse Supply Chain
- **Context**: In multi-branch restaurant chains, raw materials are often purchased in bulk at a central commissary or master warehouse, then transferred in smaller batches to individual branch kitchens.
- **The Question**: When multi-branch expansion occurs in v2.0, will branches support inter-branch inventory transfers, or will each branch manage independent supplier purchase orders?
- **Why It Matters**: Inter-branch transfers require shipping manifests, in-transit inventory statuses, and internal cost transfer accounting.
- **Recommended Default**: **Postpone to v2.0**. For v1.0 MVP, isolate raw ingredient catalogs and inventory balances per `BranchId`.
- **Status**: `OPEN QUESTION` (Non-blocking)

---

### OQ-05: Customer Self-Ordering Prepayment Policy (Phase 2)
- **Context**: In Phase 2, guests scan a QR code at their table to view the menu and place orders from their smartphone.
- **The Question**: Should customer QR orders require immediate digital prepayment (via VietQR or MoMo) before being sent to the kitchen, or can guests place orders freely and pay at the end of the meal at the cashier?
- **Why It Matters**: Immediate prepayment prevents "prank orders" or accidental fires from unauthorized guests, but creates friction if guests want to order multiple rounds of drinks throughout the night.
- **Recommended Default**: **Configurable by Restaurant Mode**:
  - Fast-casual / Counter style: Pay-at-order required.
  - Full-service / Table dining: Pay-at-checkout (Waitstaff verifies order or table session is unlocked by server).
- **Status**: `OPEN QUESTION` (Phase 2 Consideration)

---

### OQ-06: Handling Service Charge Taxability
- **Context**: Some restaurants apply a 5% service charge. In Vietnamese tax regulations, depending on the business registration type, VAT may apply to $(\text{Subtotal} + \text{Service Charge})$ or only to $\text{Subtotal}$.
- **The Question**: Should VAT be calculated on top of the service charge, or should VAT and service charge be calculated independently on the post-discount subtotal?
- **Why It Matters**: Affects the exact rounding and legal compliance of the billing calculation engine.
- **Recommended Default**: **Provide a branch-level configuration switch**:
  - `IsServiceChargeTaxable: true` $\rightarrow \text{VAT} = (\text{Subtotal} + \text{ServiceCharge}) \times \text{VAT Rate}$.
  - `IsServiceChargeTaxable: false` $\rightarrow \text{VAT} = \text{Subtotal} \times \text{VAT Rate}$.
  Default to `true` (standard Vietnamese hospitality practice).
- **Status**: `OPEN QUESTION` (Awaiting Owner Confirmation)
