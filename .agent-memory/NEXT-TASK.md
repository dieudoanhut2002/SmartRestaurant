# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-CUSTOMER-LOYALTY-01
- **Task Title**: Customer Loyalty, Membership Tiering & Points Accrual Wallet
- **Lifecycle Stage**: Stage 8 / Phase 2 — Digital Expansion & Customer Relationship Management (CRM)
- **Current Status**: **STAGE 1 THROUGH EXECUTIVE BI ANALYTICS (TASK-ANALYTICS-DASHBOARD-01) COMPLETED & VERIFIED**:
  - Task 01: Core Domain Foundation (Completed)
  - Task 02: Manager Portal & Floor Designer (Completed)
  - License Foundation: Cryptographic Licensing & Offline Engine (Completed)
  - Task 03: POS Waiter Tablet Web Application (Completed)
  - Task 04: Kitchen Display System & Expediter Pass (Completed)
  - Task 05: Cashier Terminal, Split Bill, VietQR & Blind Shift Close (Completed)
  - Task 06: Pilot Rollout & Automation Scripts (Completed)
  - Task 07: Customer Self-Service Table QR Ordering & Guest Portal (Completed)
  - Task 08: Executive BI Analytics, Sales Heatmap & BCG Menu Engineering (Completed)
- **Automated Test Results**: **120/120 tests PASSING (100% pass rate, 0 failed, 0 skipped)**. Full solution builds with **0 errors, 0 warnings** across all 10 projects.
- **Approval Requirements**: Authorized by Project Owner.

---

## 1. Objective
Design and implement an integrated Customer Loyalty & Membership Management Subsystem allowing dining guests to look up or register membership via phone number directly inside the Guest Mobile QR Portal (`/Guest`) or Cashier POS, accrue points based on eligible bill spend (e.g., 1 điểm per 10,000 VNĐ), advance through tiers (Thành Viên Đồng, Bạc, Vàng, Kim Cương), and redeem points for direct invoice discounts.

---

## 2. Scope
1. **Domain Foundation**:
   - `CustomerMember`: Full POCO entity with Phone, FullName, Email, CurrentPoints, LifetimePoints, MembershipTier (`MemberTier` enum: Bronze, Silver, Gold, Diamond), JoinedAt.
   - `MemberPointsTransaction`: Audit ledger entity tracking point additions, deductions, reasons (Spend, Redemption, Expiration, Manual adjustment), reference order/invoice ID.
2. **Business Rules**:
   - Points accrual rate: 1 point per 10,000 VNĐ paid bill total (configurable in Settings).
   - Points redemption: 100 points = 10,000 VNĐ discount on invoice.
   - Tier upgrade rules: Silver (> 500 points), Gold (> 1,500 points), Diamond (> 3,000 points).
3. **Application & Infrastructure Services**:
   - `ILoyaltyService` / `LoyaltyService`: Register, search by phone, compute points on invoice settlement, redeem points.
   - Event subscription: Auto-accrue points upon `InvoiceSettled` event.
4. **User Interface Integration**:
   - Guest Mobile Portal (`SmartRestaurant.Pos.Web` `/Guest`): Membership phone input, live points balance display, discount redemption checkbox.
   - Cashier Terminal (`SmartRestaurant.Cashier.Web` `/Cashier`): Search member by phone, apply member discount to bill calculation.
   - Manager Portal (`SmartRestaurant.Web` `/Customer`): Customer member directory, spending history, manual point adjustments with manager PIN verification.
5. **Automated Verification**:
   - Unit tests covering point accrual, tier upgrading, redemption balance constraints, and transaction ledger idempotency.

---

## 3. Required Inputs
- [`docs/PRODUCT-BASELINE-v1.0.0.md`](../docs/PRODUCT-BASELINE-v1.0.0.md).
- [`src/SmartRestaurant.Domain/Entities/Order.cs`](../src/SmartRestaurant.Domain/Entities/Order.cs).
- [`src/SmartRestaurant.Application/Services/CashierService.cs`](../src/SmartRestaurant.Application/Services/CashierService.cs).
- [`src/SmartRestaurant.Pos.Web/Views/Guest/Index.cshtml`](../src/SmartRestaurant.Pos.Web/Views/Guest/Index.cshtml).

---

## 4. Expected Deliverables
1. `CustomerMember` and `MemberPointsTransaction` domain entities with EF Core configurations.
2. `ILoyaltyService` application interface and implementation.
3. Controller actions and UI views for Guest Mobile Portal, Cashier, and Manager Portal.
4. Comprehensive automated test suite in `SmartRestaurant.Application.Tests`.
