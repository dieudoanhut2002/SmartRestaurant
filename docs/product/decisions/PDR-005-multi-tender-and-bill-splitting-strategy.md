# Product Decision Record: PDR-005 — Multi-Tender and Bill Splitting Strategy

- **Decision ID**: PDR-005
- **Title**: Multi-Tender Settlement and Two-Tier Bill Splitting Engine
- **Status**: PROPOSED
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Group dining checkout is one of the highest-friction moments in restaurant operations. Guests frequently ask to split bills, pay with combinations of cash and electronic banking (VietQR/Napas 24/7), or split checks item-by-item. The billing engine must handle fractional calculations, itemized taxes, and multi-tender accounting without rounding leakage or cashier frustration.

---

## 2. Options Considered

### Option A: Monolithic Single-Tender Settlement Only
- A bill must be paid in full by a single tender (100% Cash or 100% Card) with no bill splitting.
- *Pros*: Trivial to develop.
- *Cons*: Totally unacceptable in modern hospitality; guarantees customer complaints and cashier bottlenecks.

### Option B: Support Split by Items + Split Evenly ($1/N$) + Multi-Tender (Recommended for MVP)
- Support two robust splitting models:
  1. **Split by Selected Items**: Move specific dishes to independent sub-bills with proportional taxes and discounts.
  2. **Split Evenly ($1/N$)**: Divide total balance into $N$ equal checks with rounding absorbed into the final sub-bill.
- Support **Multi-Tender Payments**: Allow settling any bill using multiple payment records (e.g. 500k Cash + 500k VietQR).
- *Pros*: Covers >98% of actual restaurant dining checkout requests; prevents complex arbitrary percentage math edge-cases; legally compliant with Vietnamese VAT rounding rules.
- *Cons*: Cashier UI requires clean multi-check management screens.

### Option C: Complex Arbitrary Uneven Percentage & Arbitrary Dong Splits
- Support arbitrary uneven percentage splits (e.g., Guest A pays 63.5%, Guest B pays 36.5%) or arbitrary partial amounts down to the exact Dong.
- *Pros*: Maximum academic flexibility.
- *Cons*: Extreme complexity in item-level tax and discount proportional reconciliation; high probability of rounding discrepancies; rarely requested in actual dining operations.

---

## 3. Recommendation & Decision
**Adopt Option B (Split by Items + Split Evenly + Multi-Tender)** for MVP. Defer Option C to future releases if a compelling business use-case emerges.

> [!NOTE] Scope Classification & Confirmation Boundaries
> - **Confirmed Business Requirements**: Per Owner decisions REV-07 (Delayed VietQR settlement & suspense reconciliation), REV-10 (Multi-bill table session & independent sub-bill settlement), and the BillingRequested Final Decision (unpaid bills remain editable, new orders append to existing bill or create new sub-bill, paid bills are immutable), the business rules for checkout and bill splitting are **CONFIRMED**.
> - **Technical Proposal**: The specific mathematical rounding algorithm (absorbing residual fractional Dong into the final sub-bill), UI workflows, and transaction boundaries are **PROPOSED** technical designs subject to Stage 2 Solution Architect review.

---

## 4. Consequences
- **Positive**: Rapid, reliable checkout; mathematically sound; zero sub-Dong rounding discrepancies; seamless VietQR integration.
- **Negative**: Cashiers must guide guests to split either by dishes or evenly into equal parts.

---

## 5. Questions & Assumptions
- *Question*: `OQ-01` tracks whether arbitrary percentage splits should be reconsidered post-MVP.
