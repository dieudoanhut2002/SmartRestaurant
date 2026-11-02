# Product Decision Record: PDR-002 — Hierarchical Area, Room, and Table Model

- **Decision ID**: PDR-002
- **Title**: Hierarchical Area, Room, and Table Spatial Model
- **Status**: `PROPOSED` (Pending User Approval)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Restaurants feature diverse spatial layouts. Some venues have open dining areas, while others have separate rooms, outdoor terraces, and private VIP suites (VIP 1, VIP 2, VIP 3). Furthermore, guests may occupy a single table, multiple merged tables, an entire room, or a private VIP suite. We need a spatial domain model that flexibly accommodates all these operational arrangements.

---

## 2. Options Considered

### Option A: Flat Table List
- Tables are simply named "Table 1", "Table 2", "VIP 1" in a flat list with no spatial hierarchy.
- *Pros*: Extremely simple database schema.
- *Cons*: Cannot group tables by floor or area; cannot render multi-area visual canvases; cannot support whole-room reservations or bulk table status operations.

### Option B: Hierarchical Model (Area $\rightarrow$ Room $\rightarrow$ Table) with Multi-Table Session Binding
- Establish an explicit hierarchy:
  - `Branch` has one or more `DiningArea`s (e.g. Ground Floor, Garden Terrace).
  - `DiningArea` contains zero or more `Room`s (including VIP suites) and zero or more standalone `Table`s.
  - `Room` contains one or more `Table`s.
  - A `DiningSession` can bind to one primary table, multiple linked tables (merge), or an entire room.
- *Pros*: Perfectly models physical reality; enables independent 2D floor plan canvases per area; supports whole-room booking; allows easy table grouping and zone-based server assignments.
- *Cons*: Slightly more complex relational queries and UI navigation.

---

## 3. Recommendation & Decision
**Adopt Option B (Hierarchical Area $\rightarrow$ Room $\rightarrow$ Table Model)**.

---

## 4. Consequences
- **Positive**: Floor plan designer can easily render tabs/sections per dining area; managers can book entire VIP rooms with a single action; FOH staff can intuitively locate tables.
- **Negative**: POS UI must provide smooth navigation to switch between areas without excessive clicks during peak service.

---

## 5. Questions & Assumptions
- *Assumption*: Every table belongs to exactly one Area or Room; tables cannot float across multiple areas simultaneously.
