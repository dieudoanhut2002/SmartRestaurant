# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-CUSTOMER-QR-01
- **Task Title**: Customer Self-Service Dynamic Table QR Ordering & Guest Portal (Phase 2 Roadmap)
- **Lifecycle Stage**: Stage 8 / Phase 2 — Customer Self-Service & Digital Expansion
- **Current Status**: **STAGE 1 THROUGH STAGE 7 OFFICIALLY COMPLETED & VERIFIED**:
  - Stage 1: Product Requirements & Business Domain Analysis (Approved Baseline v1.0.0)
  - Stage 2: System Architecture & Technical Design (Modular Monolith, SignalR, Clean Architecture)
  - Stage 3: Detailed Module Specifications & Data Models (POCO Entities, EF Core Mappings, Seeder)
  - Stage 4: Milestone Planning & Task Breakdown (5 Iterative Implementation Milestones)
  - Stage 5: Iterative Implementation (Manager Portal, Waiter POS, KDS, Cashier Terminal, VietQR, License Engine)
  - Stage 6: Quality Verification & End-to-End Testing (108/108 Tests, E2E Cross-Terminal Tests)
  - Stage 7: On-Premise Pilot Store Deployment (`TASK-PILOT-ROLLOUT-01`, `SmartRestaurant.LicenseCli`, `publish-all.ps1`, `configure-firewall.ps1`, `run-pilot.ps1`, `pilot-store-license.lic`, 109/109 tests passing)
- **Automated Test Results**: **109/109 tests PASSING (100% pass rate, 0 failed, 0 skipped)** across all test suites. 0 warnings, 0 errors across 10 projects.
- **Approval Requirements**: Requires Project Owner authorization before initiating Phase 2 implementation.

---

## 1. Objective
Design and implement the Customer Self-Service QR Ordering Web Application allowing dining guests to scan dynamic table QR codes with their smartphones, browse the visual digital menu with live topping/modifier customization, submit order items directly to the Waiter POS / Kitchen KDS, and track food preparation progress in real time.

---

## 2. Scope
1. **Dynamic Table QR Code Generation**:
   - Generate secure, signed table QR codes containing encrypted session/table tokens preventing URL tampering.
2. **Guest Mobile Web Ordering Portal**:
   - Responsive mobile-first interface optimized for iOS Safari and Android Chrome.
   - Real-time stock status (auto-disables 86'd items).
   - Customizer: Sugar, ice, spicy levels, side dishes, cooking preferences.
3. **Waiter POS Verification & Auto-Fire Policy**:
   - Configurable mode: Waiter must approve before firing to kitchen OR direct auto-fire with buzzer notification.
4. **Guest Bill Tracking & Split QR View**:
   - View live bill total, breakdown, and dynamic VietQR for direct table-side payment.

---

## 3. Required Inputs
- [`docs/PRODUCT-BASELINE-v1.0.0.md`](../docs/PRODUCT-BASELINE-v1.0.0.md).
- [`docs/product/workflows/ORDER-WORKFLOW.md`](../docs/product/workflows/ORDER-WORKFLOW.md).
- [`HUONGDAN.txt`](../HUONGDAN.txt).

---

## 4. Expected Deliverables
1. Architectural Decision Record for Customer QR ordering security tokens.
2. New Web application project or route for Customer Guest Mobile Portal.
3. Unit and integration tests covering guest order submission, validation, and SignalR notification.
