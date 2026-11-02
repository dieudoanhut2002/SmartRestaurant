# Product Decision Record: PDR-011 — Area and VIP Room Surcharge Policy

- **Decision ID**: PDR-011
- **Title**: Configuration and Application Rules for Dining Area and VIP Room Surcharges
- **Status**: `CONFIRMED` (Business Requirements Approved; Calculation Engine Architecture: `PROPOSED`)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Private VIP dining rooms, rooftop terraces, or scenic garden zones in Vietnamese restaurants frequently carry service premiums or room rental surcharges. The system requires clear rules on how surcharges are configured, calculated, and displayed on bills.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Supported Surcharge Types in MVP:
- **Percentage Surcharge**: A configurable percentage (e.g., +10% VIP service fee) calculated against the dining subtotal.
- **Fixed Fee Surcharge**: A flat monetary amount (e.g., 200,000 ₫ private room fee) added to the invoice.
- **Scope Exclusion**: Hourly room rate billing (e.g., 100,000 ₫/hour) is explicitly deferred to future releases and is NOT mandatory in MVP.

### 2.2. Configuration Hierarchy:
- Surcharges can be configured at either the **Area level** (e.g., Rooftop Lounge) or the **Room level** (e.g., VIP Room 01, VIP Room 02).

### 2.3. Surcharge Snapshot & Table Transfer Rules:
- **Immutable Snapshot**: The applicable surcharge rate/fee is snapshotted onto the table session and order upon activation.
- **Table Transfer Policy**: When guests transfer from a standard table to a VIP room, the system DOES NOT automatically recalculate or inflate surcharges on dishes that were already served prior to the transfer. Surcharges apply according to explicit manager selection upon transfer.
- **Invoice Transparency**: Surcharges must be displayed as distinct, transparent line items on both provisional checks and official VAT/sales invoices (never buried secretly inside individual dish prices).

---

## 3. Consequences
- **Positive**: Complies with restaurant business models in Vietnam; clear guest transparency prevents billing disputes.
- **Negative**: Cashier billing calculation engine must incorporate room/area surcharge rules alongside VAT and service charge.

> [!NOTE] Scope Classification & Confirmation Boundaries
> - **Confirmed Business Requirements**: Per Owner decision OD-07 (REV-06), percentage and fixed fee surcharges for VIP rooms/areas, surcharge snapshotting, and non-retroactive transfer policy are **CONFIRMED**.
> - **Technical Proposal**: The specific billing engine calculation pipeline, entity surcharge snapshot schema, and tax-on-surcharge configuration switches are **PROPOSED** technical designs subject to Stage 2 Solution Architect review.
