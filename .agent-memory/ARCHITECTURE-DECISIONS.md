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
- **Status**: `PROPOSED`
- **Date**: 2026-11-02
- **Context**: Deducting raw ingredients when an order is placed can cause stock inaccuracies if an order is cancelled before cooking begins.
- **Decision**: Trigger automatic raw ingredient deduction at the moment the kitchen station acknowledges the dish as `ReadyToServe` (or `Cooking`). If an order is cancelled after `Cooking` has started, it must be logged as kitchen waste rather than regular deduction.
- **Consequences**:
  - Prevents artificial phantom stock depletion.
  - Accurately captures waste vs standard consumption.
  - Requires sign-off from the user before finalizing.

---

### ADR-006: Dynamic VietQR (Napas 24/7) Payment Integration
- **Status**: `PROPOSED`
- **Date**: 2026-11-02
- **Context**: Fast, contactless, error-free mobile banking payment is essential in modern Vietnamese dining establishments.
- **Decision**: Implement dynamic VietQR code generation per bill containing the exact subtotal and an encrypted order reference code, paired with webhook/polling payment confirmation.
- **Consequences**:
  - Eliminates cashier manual amount entry errors.
  - Instant automatic checkout reconciliation.

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
- **Decision**: Introduce `BusinessDate` (Ngày Kinh Doanh) as an invariant property across `ShiftSession`, `DiningSession`, `Order`, and `Invoice`. All orders in a shift inherit the shift's `BusinessDate` until the shift is formally closed.
- **Consequences**:
  - Financial consistency and exact cash reconciliation for overnight operations.
  - Requires database queries for daily revenue to filter by `BusinessDate` rather than simple UTC date.

---

### ADR-009: Two-Phase Inventory Lock with Phased BOM Rollout
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Deducting inventory only when a dish is ready causes overselling of limited dishes during cooking. Furthermore, complex gram/ml BOM setup risks delaying the MVP launch.
- **Decision**: In MVP v1.0, manage inventory directly at the portion/item level. Implement a Two-Phase Inventory Lock: Reserve stock (`Tạm giữ`) when the order is fired, and permanently deduct (`Xuất kho`) when marked `ReadyToServe`. Defer sub-gram raw material BOM to v1.1.
- **Consequences**:
  - Eliminates overselling during peak hours.
  - Reduces MVP setup complexity while maintaining operational inventory control.

---

### ADR-010: Resolution of Delayed VietQR Webhooks Following Cash Settlement
- **Status**: `CONFIRMED`
- **Date**: 2026-11-02
- **Context**: Delayed bank webhooks arriving after a customer pays cash create duplicate payment accounting conflicts.
- **Decision**: Invalidate the dynamic QR reference when an invoice is settled via Cash. Route late-arriving bank webhook funds into an `UnallocatedCustomerDeposit` account and trigger an immediate audio-visual alert on the cashier screen to issue a cash refund.
- **Consequences**:
  - Prevents untracked revenue discrepancies and preserves cashier drawer auditability.
