# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-PILOT-ROLLOUT-01
- **Task Title**: On-Premise Pilot Store Deployment, Production Database Migration & Live Dry-Run
- **Lifecycle Stage**: Stage 7 — Deployment Preparation, Configuration & Pilot
- **Current Status**: **STAGE 5 & STAGE 6 COMPLETED & VERIFIED**:
  - Task 01: Core Domain Foundation (Completed)
  - Task 02: Manager Web Application Portal (Completed)
  - License Cryptographic Security Engine (Completed)
  - Task 03: POS Waiter Tablet Web Application (`SmartRestaurant.Pos.Web`) (Completed)
  - Task 04: Kitchen Display System & Expediter Pass (`SmartRestaurant.Kds.Web`) (Completed)
  - Task 05: Cashier Terminal, Split Bill, VietQR 24/7 Gateway & Blind Shift Close (`SmartRestaurant.Cashier.Web`) (Completed)
  - Stage 6 Multi-Terminal E2E Integration (`TASK-INTEGRATION-E2E-01`) (Completed, 107/107 tests passing)
- **Automated Test Results**: **108/108 tests PASSING (100% pass rate, 0 failed, 0 skipped)**. Full solution builds with **0 errors, 0 warnings** across all 9 projects.
- **Approval Requirements**: Authorized by Project Owner under full implementation autonomy mandate ("Agent chủ động thực hiện -> Agent tự kiểm tra -> Agent tự khắc phục -> Agent báo cáo kết quả").

---

## 1. Objective
Prepare the final on-premise installation package, configure production SQL Server connection strings, establish Windows Service / IIS reverse proxy or Kestrel self-hosting scripts, and conduct live dry-run verification in a realistic pilot restaurant environment.

---

## 2. Scope
1. **Host Environment & Network Configuration**:
   - Verify LAN binding (`0.0.0.0` or local static IP) so waiter tablets (iPads/Android tablets) and kitchen monitors can access `SmartRestaurant.Pos.Web` and `SmartRestaurant.Kds.Web` via local Wi-Fi.
   - Configure Windows Firewall rules for ports 5000, 58329, 5098, 5099.
2. **Production Database Preparation**:
   - Verify automated seed execution on fresh SQL Server instance.
   - Verify initial master license generation and activation for the pilot store.
3. **Hardware Integration Check**:
   - Thermal printer ESC/POS or browser print dialog verification for 80mm receipts.
   - Cash drawer kick signal or cashier workflow alignment.
4. **Staff Training & Operations Handover**:
   - Pilot operational testing with Manager, Waiter, Chef, and Cashier roles.

---

## 3. Required Inputs
- [`HUONGDAN.txt`](../HUONGDAN.txt) (Deployment manual).
- [`run-all.ps1`](../run-all.ps1) (Multi-host launcher).
- [`docs/PRODUCT-BASELINE-v1.0.0.md`](../docs/PRODUCT-BASELINE-v1.0.0.md).

---

## 4. Expected Deliverables
1. Production deployment profile / batch publish scripts (`dotnet publish`).
2. LAN firewall configuration script.
3. Pilot store sign-off and operational handover.
