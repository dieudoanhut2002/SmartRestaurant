# SmartRestaurant — Architectural Decision Records (ADRs)

This file tracks the formal architectural and technical decisions made for **SmartRestaurant**.

---

### ADR-001: Modular Monolith Architecture
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: SmartRestaurant requires clear module boundaries, enterprise extensibility, and rapid low-latency local network communication without the DevOps complexity, network latency, and distributed transaction pitfalls of microservices.
- **Decision**: Build the application as a single deployable Modular Monolith using Clean Architecture principles (`SmartRestaurant.Domain`, `SmartRestaurant.Application`, `SmartRestaurant.Infrastructure`, `SmartRestaurant.Web`).
- **Consequences**:
  - Eliminates distributed RPC overhead and eventual consistency issues.
  - Enforces strict module isolation through public interfaces and domain events.
  - Allows easy extraction of microservices in the future if specific high-scale modules require independent scaling.

---

### ADR-002: Core Technology Stack Selection
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: The platform requires high throughput, type safety, long-term support, and seamless integration between web clients, background workers, and real-time sockets.
- **Decision**: Adopt C# .NET LTS (ASP.NET Core Web API + Razor MVC), Microsoft SQL Server with Entity Framework Core (Code-First), and modern HTML5/TypeScript/Bootstrap 5 frontend.
- **Consequences**:
  - Leverages high-performance asynchronous runtime and robust ORM features (Optimistic Concurrency, Global Query Filters).
  - Streamlines development by utilizing standard ASP.NET Core ecosystem tooling.

---

### ADR-003: Real-Time Synchronization via SignalR
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Floor managers, waitstaff, kitchen chefs, and cashiers require immediate state synchronization when orders are placed, tables change status, or dishes are marked ready.
- **Decision**: Centralize all real-time bidirectional messaging via ASP.NET Core SignalR hubs (`OrderHub`, `TableHub`, `KitchenHub`).
- **Consequences**:
  - Sub-second latency for KDS ticket updates and table status indicators.
  - Requires fallback polling and automatic reconnection logic for unreliable Wi-Fi environments.

---

### ADR-004: 2D Canvas Interactive Floor Plan Designer via Konva.js
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Restaurant managers need an intuitive visual tool to design custom restaurant floor plans with irregular shapes, diverse table capacities, and obstacles.
- **Decision**: Use Konva.js (HTML5 2D Canvas library) to provide drag-and-drop table layouts, grid snapping, zoom/pan, and normalized coordinate persistence.
- **Consequences**:
  - Delivers smooth client-side interaction without heavy third-party CAD dependencies.
  - Persists floor layouts as lightweight JSON coordinate data (`X`, `Y`, `Rotation`, `Shape`).

---

### ADR-005: Recipe BOM Inventory Deduction Timing
- **Status**: `SUPERSEDED` (Superseded by ADR-009 / PDR-004 in Task 02.6)
- **Date**: 2026-11-02
- **Context**: Deducting raw ingredients when an order is placed can cause stock inaccuracies if an order is cancelled before cooking begins.
- **Decision**: Superseded by Two-Phase Inventory Lock (ADR-009).

---

### ADR-006: Dynamic VietQR (Napas 24/7) Payment Integration
- **Status**: `CONFIRMED` (Formally approved in Task 02.6 via PDR-008 & ADR-010)
- **Date**: 2026-11-02
- **Context**: Fast, contactless, error-free mobile banking payment is essential in modern Vietnamese dining establishments.
- **Decision**: Implement dynamic VietQR code generation per bill containing exact payable amounts and unique payment reference codes, paired with asynchronous webhook processing and Suspense Ledger reconciliation.
- **Consequences**:
  - Eliminates cashier manual amount entry errors.
  - Asynchronous webhook handling with anti-duplicate guards.

---

### ADR-007: 100% Vietnamese Language Presentation Layer and User Interfaces
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: The primary operating market is Vietnam. Waitstaff, line cooks, bussers, cashiers, and restaurant managers require a native Vietnamese user experience with zero language barriers during high-pressure shifts.
- **Decision**: Mandate 100% Vietnamese language for all user-facing client applications (Admin Portal, Waiter Tablet POS, Kitchen Display System - KDS, Bar Station, Cashier Terminal, and receipts/chits). Technical code, C# entities, APIs, and architecture specifications remain in standard English with clean UI localization decoupling.
- **Consequences**:
  - Eliminates operational training friction for restaurant staff.
  - Aligns with Vietnamese commercial and tax receipt standards.
  - Requires UTF-8 character encoding support across thermal printers and canvas typography.

---

### ADR-008: Business Date Partitioning for Overnight Shift Financial Accounting
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Hospitality operations frequently cross midnight (00:00). Grouping sales by calendar date splits overnight shifts and causes accounting discrepancies.
- **Decision**: Introduce `BusinessDate` (Ngày Kinh Doanh) with configurable branch cutoff times (default 06:00 AM) as an invariant property across `ShiftSession`, `DiningSession`, `Order`, and `Invoice`. All orders in a shift inherit the shift's `BusinessDate` statically at creation.
- **Consequences**:
  - Financial consistency and exact cash reconciliation for overnight operations.
  - Requires database queries for daily revenue to filter by `BusinessDate` rather than simple UTC date.

---

### ADR-009: Two-Phase Inventory Lock with Phased BOM Rollout
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Deducting inventory only when a dish is ready causes overselling of limited dishes during cooking. Furthermore, complex gram/ml BOM setup risks delaying the MVP launch.
- **Decision**: In MVP v1.0, manage inventory directly at the portion/item level. Implement a Two-Phase Inventory Lock: Reserve stock (`Tạm giữ`) when the order is fired, and permanently deduct (`Xuất kho`) when marked `ReadyToServe`. Formula: `Available = Physical - Reserved`. Defer sub-gram raw material BOM to v1.1.
- **Consequences**:
  - Eliminates overselling during peak hours.
  - Reduces MVP setup complexity while maintaining operational inventory control.

---

### ADR-010: Resolution of Delayed VietQR Webhooks Following Cash Settlement
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Delayed bank webhooks arriving after a customer pays cash create duplicate payment accounting conflicts.
- **Decision**: Invalidate the dynamic QR reference when an invoice is settled via Cash. Route late-arriving bank webhook funds into a Suspense Ledger (`Giao dịch treo đối soát`) and trigger an immediate audio-visual alert on the cashier screen for governed manual review.
- **Consequences**:
  - Prevents untracked revenue discrepancies and preserves cashier drawer auditability.

---

### ADR-011: Multi-Bill Table Sessions for Flexible Split Billing
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Tables undergoing split checks must not freeze active table dining operations.
- **Decision**: Model table sessions as 1-to-N relationships with sub-bills (`TableSession` hosts multiple `SubBills`). Servers can continue adding new dishes to unpaid sub-bills with explicit target assignment. Physical table releases only when 100% of sub-bills are paid and all items served.
- **Consequences**:
  - Uninterrupted service during group dining checkouts.

---

### ADR-012: Table Unmerge and Line Item Reallocation
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Joined tables may need to be unmerged when dining parties depart separately.
- **Decision**: Support table unmerging with line item reallocation without re-generating kitchen tickets or disrupting existing dish preparation states.
- **Consequences**:
  - High operational flexibility for banquet service.

---

### ADR-013: Area and VIP Room Surcharges Snapshot Policy
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Premium dining spaces carry service premiums or room fees.
- **Decision**: Support percentage (%) and flat fee surcharges configured per area or room. Surcharges are snapshotted onto orders at creation; moving tables does not recalculate existing items retroactively.
- **Consequences**:
  - Billing transparency on provisional checks and invoices.

---

### ADR-014: Direct Front-of-House Beverage Routing
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Packaged cold drinks fetched directly from floor chillers should not clutter Bar KDS queues.
- **Decision**: Add routing property `ServiceRouting = DirectFOHPick` allowing packaged grab items to bypass Bar KDS while remaining strictly bound to billing and inventory reservation.
- **Consequences**:
  - Improves bar prep efficiency and speeds up beverage service.

---

### ADR-015: Non-Blocking Concurrent Table Ordering Business Rules
- **Status**: `CONFIRMED` (Technical Architecture delegated to Stage 2)
- **Date**: 2026-11-02
- **Context**: Multiple servers drafting orders on the same large table must not overwrite items or lock out terminals.
- **Decision**: Approve non-blocking shared ordering business rules with conflict detection and anti-duplicate idempotency. Technical mechanism (e.g. event sourcing delta append, atomic SQL procedures) is assigned to Stage 2 Solution Architecture.
- **Consequences**:
  - Fluid waiter user experience during rush hours.

---

### ADR-016: Remake Dish Workflow for Culinary Errors
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Prepared dishes rejected due to kitchen mistakes must not double-charge customers.
- **Decision**: Implement a dedicated Remake Dish action with Manager PIN and mandatory error categorization. Triggers a high-priority purple KDS card without increasing customer bill total; logs scrap to the kitchen Waste Register.
- **Consequences**:
  - Customer bill accuracy and transparent kitchen error auditing.
