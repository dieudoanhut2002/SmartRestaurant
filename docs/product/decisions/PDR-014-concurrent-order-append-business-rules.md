# Product Decision Record: PDR-014 — Concurrent Waiter Ordering Business Rules

- **Decision ID**: PDR-014
- **Title**: Business Rules for Concurrent Waiter Operations on Shared Table Sessions
- **Status**: `CONFIRMED` (Business Requirements Approved; Technical Implementation Mechanics: `PROPOSED`)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
During rush hours, multiple waiters often attend to different guests at the same large banquet table simultaneously. Locking the table exclusively (Pessimistic Table Lock) stalls operations, while basic optimistic concurrency risks overwriting cart changes or discarding newly entered dishes when multiple servers submit orders close together.

---

## 2. Approved Business Rules (Confirmed by Project Owner)

### 2.1. Non-Blocking Shared Table Access:
- Multiple waitstaff members must be permitted to open and draft orders for the same physical table concurrently without triggering blocking UI lockouts.

### 2.2. Data Loss Prevention:
- One waiter's order submission must NEVER overwrite, delete, or discard line items submitted concurrently by another waiter.

### 2.3. Conflict Detection for Item Modification / Cancellation:
- If Waiter A attempts to cancel or modify a specific dish line item while Waiter B is also modifying the same item, the system must detect the conflict, prevent partial corruption, and present a clear prompt reflecting the current state.

### 2.4. Idempotency & Duplicate Prevention:
- The system must provide robust client-side and server-side idempotency safeguards (e.g., submission idempotency keys) to prevent duplicate kitchen tickets or duplicate order lines caused by rapid tapping, double clicking, or network retry loops.

---

## 3. Technical Implementation Status (Solution Architecture Scope)
- **Status**: `PROPOSED`
- **Architectural Directive**: The previously suggested "Append-Only Delta Firing" pattern is an architectural proposal, not a mandated technical solution. The **Solution Architect** in Stage 2 is tasked with analyzing and selecting the optimal technical mechanism (e.g., event sourcing delta append, atomic row-level command queues, or distributed optimistic locking) to satisfy these business rules.

---

## 4. Consequences
- **Positive**: High throughput and responsive waiter UX during peak hours; zero lost orders.
- **Negative**: Technical architecture must design robust concurrency conflict resolution and idempotency handling.
