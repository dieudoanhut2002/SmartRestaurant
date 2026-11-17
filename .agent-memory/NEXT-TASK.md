# SmartRestaurant — Next Approved Task Specification

- **Task Identifier**: TASK-ANALYTICS-DASHBOARD-01
- **Task Title**: Advanced Business Intelligence, Real-Time Sales Heatmap & Recipe BOM Margin Analytics
- **Lifecycle Stage**: Stage 8 / Phase 2 — Digital Expansion & Business Intelligence
- **Current Status**: **STAGE 1 THROUGH STAGE 8 (CUSTOMER QR ORDERING) COMPLETED & VERIFIED**:
  - Task 01: Core Domain Foundation (Completed)
  - Task 02: Manager Portal & Floor Designer (Completed)
  - License Foundation: Cryptographic Licensing & Offline Engine (Completed)
  - Task 03: POS Waiter Tablet Web Application (Completed)
  - Task 04: Kitchen Display System & Expediter Pass (Completed)
  - Task 05: Cashier Terminal, Split Bill, VietQR & Blind Shift Close (Completed)
  - Task 06: Pilot Rollout & Automation Scripts (Completed)
  - Task 07: Customer Self-Service Table QR Ordering & Guest Portal (Completed)
- **Automated Test Results**: **116/116 tests PASSING (100% pass rate, 0 failed, 0 skipped)**. Full solution builds with **0 errors, 0 warnings** across all 10 projects.
- **Approval Requirements**: Authorized by Project Owner.

---

## 1. Objective
Enhance the Manager Portal with an Executive Business Intelligence Dashboard delivering real-time hourly revenue heatmaps, table turnover efficiency analysis, top-selling items Pareto breakdown, and recipe BOM gross margin reports.

---

## 2. Scope
1. **Real-Time Revenue Analytics**:
   - Hourly sales peak load graph (Chart.js visual).
   - Sales breakdown by tender method (Cash vs Card vs VietQR Napas).
2. **Operational Efficiency Metrics**:
   - Table turnover rate and average dining duration.
   - Waiter order speed and kitchen ticket preparation duration (SLA compliance).
3. **Menu Engineering & BOM Gross Margin**:
   - Cost of Goods Sold (COGS) vs selling price.
   - High-margin vs low-margin item classification.
4. **Data Export**:
   - Export revenue and shift reports to Excel / CSV format.

---

## 3. Required Inputs
- [`docs/PRODUCT-BASELINE-v1.0.0.md`](../docs/PRODUCT-BASELINE-v1.0.0.md).
- [`src/SmartRestaurant.Application/Services/ReportService.cs`](../src/SmartRestaurant.Application/Services/ReportService.cs).
- [`src/SmartRestaurant.Web/Controllers/ReportController.cs`](../src/SmartRestaurant.Web/Controllers/ReportController.cs).

---

## 4. Expected Deliverables
1. Analytics service extensions and DTOs.
2. Interactive dashboard views with Chart.js charts.
3. Unit and integration tests covering analytics aggregations.
