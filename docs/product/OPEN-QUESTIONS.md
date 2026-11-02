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

### OQ-03: Waste Attribution on Post-Fire Dish Cancellations & Remakes
- **Context**: If a customer cancels a dish after entering `Cooking` status or if a dish is remade due to kitchen error.
- **Resolution**: **RESOLVED in Task 02.6 (PDR-004 & PDR-015)**. Post-fire cancellations and remakes convert reserved stock to `WasteStock` in the `WasteLog` table with mandatory reason code and manager authorization. Customer bill is protected.
- **Status**: `CONFIRMED`

---

### OQ-07: Handling Extreme Long-Duration Sessions (> 24 Hours Spanning Multiple Cutoffs)
- **Context**: In rare cases (e.g., 24/7 airport branches, karaoke-restaurant hybrid, or staff forgetting to close an abandoned table session), a table session may remain open for more than 24 hours, spanning multiple consecutive `BusinessDate` cutoff boundaries.
- **The Question**: When an order stays open across 2 or more cutoff triggers, should the system automatically alert managers to force-close or partition the bill, or should it silently retain the original `BusinessDate` until manual checkout?
- **Recommended Default**: System retains original `BusinessDate` for accounting consistency, but flags the table with a high-visibility "Phiên bàn kéo dài > 24h" warning banner on the Manager dashboard to prompt operational audit.
- **Status**: `OPEN QUESTION` (Under Operational Evaluation)

---

### OQ-08: Concurrency Strategy Selection for Shared Table Ordering
- **Context**: Multiple waiters draft and fire orders on the same active table concurrently (PDR-014).
- **The Question**: Which technical mechanism should the Solution Architect select in Stage 2 to achieve non-blocking order append, idempotency, and conflict detection?
  - Option A: Event Sourcing / Delta Command Append.
  - Option B: Atomic SQL Stored Procedure / Table Session Delta Transactions with RowVersion concurrency checks.
  - Option C: Distributed In-Memory Command Queue (Redis / Channel-based background worker).
- **Status**: `OPEN QUESTION` (Delegated to Stage 2 Solution Architecture)

---

### OQ-09: Policy for Corrections, Voids, and Refunds on Paid Bills
- **Context**: In accordance with Owner Decision OD-13 and PDR-009, paid bills (`PaidInvoices`) are strictly immutable and cannot be directly edited by waitstaff or cashiers.
- **The Question**: When an operational error occurs post-settlement (e.g., incorrect billing amount detected, duplicate payment, guest dispute after checkout, or returned dish post-payment), what is the authorized correction, void, or refund workflow?
  - Option A: Hard Immutability without In-App Mutation (Recommended for MVP). The settled invoice remains permanently locked in POS. Any cash return or dispute adjustment is handled off-system via manager paper receipt and recorded as a variance during blind shift closing.
  - Option B: Formal In-App Invoice Void & Credit Note Workflow. Requires Restaurant Manager PIN to void the invoice, generates an audited negative offsetting ledger entry, and restores/reallocates the dining session items.
- **Recommended Default**: **Option A for MVP**. Maintain strict database immutability for settled bills; defer in-app void/refund state machines until formal Owner approval.
- **Status**: `OPEN QUESTION` (OD-14 — Awaiting Project Owner Confirmation)

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

---

### OQ-10: Multi-Branch Entity Ownership & Data Partitioning Boundaries
- **Context**: Per Owner Decision OD-01, the MVP business scope is strictly one independent restaurant at one physical business location. Existing technical foundations (such as `OrganizationId` and `BranchId` keys) are retained where technically reasonable to prevent rework.
- **The Question**: How should the Solution Architect partition entity ownership across the database schema during Stage 2?
  - Catalog entities (`Category`, `MenuItem`, `RecipeBOM`) vs. runtime operational entities (`DiningArea`, `RestaurantTable`, `TableSession`, `Order`, `Invoice`, `ShiftSession`).
  - Which entities require both `OrganizationId` and `BranchId`, and which require only `BranchId`?
- **Recommended Default**: Delegate to Stage 2 Solution Architecture review. Ensure no redundant dual-key overhead is forced onto entities where a single boundary is sufficient, while safeguarding data isolation.
- **Status**: `OPEN QUESTION` (Delegated to Stage 2 Solution Architecture)
