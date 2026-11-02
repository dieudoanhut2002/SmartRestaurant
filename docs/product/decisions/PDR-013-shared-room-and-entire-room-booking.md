# Product Decision Record: PDR-013 — Shared Room Multi-Table and Entire Room Booking Policy

- **Decision ID**: PDR-013
- **Title**: Lifecycle Management for Shared Multi-Table Dining Rooms and Entire Room Bookings
- **Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Large banquet rooms or dining halls often contain multiple discrete tables. The system must support both scenarios: allowing unrelated parties to share tables in the same room, as well as reserving or occupying the entire room for a single private party.

---

## 2. Decision & Operational Rules (Approved by Project Owner)

### 2.1. Default Independent Table Operation:
- Tables located within the same room container operate completely independently by default. Each table maintains its own dining session, orders, sub-bills, and table lifecycle states (`Available`, `Occupied`, `Reserved`).

### 2.2. Entire Room Booking (Bao Trọn Phòng):
- A manager or host can execute an **"Bao trọn phòng" (Book Entire Room)** operation for a private party or banquet.
- **Exclusive Room Lock**: When activated, the system blocks external guest seating and transitions all tables in that room into a reserved/occupied state linked to the single master room reservation. No walk-in guests or other parties can be assigned to any table in that room.

### 2.3. Standard Room Session Release:
- When the entire-room dining session concludes, individual tables within the room must be checked for fulfilled obligations and released strictly in accordance with standard table release lifecycle (verifying 100% bill settlement and all items served) before transitioning to `CleaningNeeded` or `Available`.

---

## 3. Consequences
- **Positive**: Accurately mirrors hospitality room management; maximizes revenue by supporting both shared banquet seating and exclusive private rentals.
- **Negative**: Floor plan visual designer and POS must render both room boundaries and individual table statuses clearly.
