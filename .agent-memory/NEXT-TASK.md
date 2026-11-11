# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-IMPLEMENTATION-03
- **Task Title**: POS Order Capture & Waiter Tablet Application
- **Lifecycle Stage**: Stage 5 — Iterative Implementation by Approved Milestone
- **Current Status**: **MANAGER APPLICATION (TASK 02) COMPLETED & FULLY VERIFIED**. All 7 core management domains (A: Dashboard, B: Spatial 2D Floor Plan & VIP Rooms, C: Menu Catalog & Modifiers, D: Kitchen Stations & KDS Routing, E: Staff & PIN RBAC, F: Inventory 4-State Tracking, G: Restaurant Config & Presets) have been built, compiled with 0 warnings/0 errors, verified HTTP 200 on all endpoints, and 45/45 automated unit/integration tests passing.
- **Approval Requirements**: Awaiting Project Owner review and authorization to proceed to Task 03 (POS Application) or Task 04 (KDS Application).

---

## 1. Objective
Implement the POS Waiter Tablet Application (Tiền sảnh FOH & Gọi món cảm ứng) allowing waitstaff to open dining sessions, select dishes and variants, enter cooking notes, and submit kitchen orders with real-time SignalR table state broadcasting.

---

## 2. Scope
1. **Dining Session & Ordering Domain Logic**:
   - Multi-round ordering (`OrderRound`, `OrderTicket`).
   - Line items with portion sizes, modifiers, and freeform kitchen notes ("Ít cay", "Không hành").
   - Two-Phase Inventory reservation (`D2`, `PDR-004`).
2. **SignalR Hub Real-Time Synchronization**:
   - `TableHub` broadcasting table state changes (`Occupied`, `BillingRequested`, `CleaningNeeded`).
   - `KitchenHub` routing tickets to target prep stations (`HotKitchen`, `ColdKitchen`, `Bar`, `Bakery`).
3. **Waiter Tablet Touch UI**:
   - Responsive touch-optimized POS screen for fast tablet/mobile ordering.
   - Quick table picker by floor/room.
   - Category navigation and search under 100ms (`BR-POS-002`).
   - Order review cart and "BÁO BẾP" action.
4. **Automated Unit & Integration Tests**:
   - Order submission tests, concurrent ordering non-blocking rules (`PDR-014`).

---

## 3. Required Inputs
- [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md) (Workflows §8.2, Requirements `BR-POS-001..007`, `BR-KIT-001..006`).
- [AGENTS.md](../AGENTS.md) & [GEMINI.md](../GEMINI.md).
- Existing solution foundation (`SmartRestaurant.slnx`).

---

## 4. Expected Deliverables
1. Order capture application services & SignalR hubs.
2. POS Waiter Tablet responsive UI.
3. Automated test suite for order placement & station routing.
4. Verification report with 100% passing tests.
