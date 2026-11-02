# Product Decision Record: PDR-003 — Independent Station Kitchen Routing & Expediter Synchronization

- **Decision ID**: PDR-003
- **Title**: Dynamic Kitchen Station Routing with Master Expediter Synchronization
- **Status**: `PROPOSED` (Pending User Approval)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
Restaurants operate specialized kitchen departments: Hot Kitchen (grill, sauté, fry), Cold Station (salads, raw appetizers), Bar Station (cocktails, beers, soft drinks), and Bakery/Dessert. A single customer order frequently contains dishes routed to multiple departments. We need an architectural mechanism to route items accurately without hardcoding station names into the software.

---

## 2. Options Considered

### Option A: Hardcoded Fixed Kitchen Stations
- System hardcodes 4 fixed stations: "Kitchen", "Bar", "Grill", "Pastry".
- *Pros*: Simple to code initially.
- *Cons*: Cannot accommodate diverse restaurant styles (e.g. an Asian restaurant with BBQ Station + Dim Sum Station + Hotpot Prep vs an Italian bistro with Wood-fired Pizza Oven + Pasta Station).

### Option B: Monolithic Single-Screen Kitchen Display
- All stations share one giant KDS screen displaying all items for all orders.
- *Pros*: No routing logic needed.
- *Cons*: Total visual chaos during rush hours; bartenders see hotpots; grill cooks see wine bottles; unacceptable cognitive overload.

### Option C: Configurable Station Routing Engine with Master Expediter Screen (Recommended)
- Restaurant managers define custom `KitchenStation` records per branch.
- Each menu item (or variant) is assigned to a target `KitchenStation`.
- When an order is fired, the routing engine decomposes the order into station-specific tickets for line cooks.
- A **Master Expediter Display** at the kitchen pass displays the consolidated table ticket with real-time status badges for all child items, enabling the pass lead to coordinate table-ready serving.
- *Pros*: Maximum operational flexibility; scales to any restaurant cuisine or kitchen layout; prevents cold dishes sitting at the pass.
- *Cons*: Requires managing multiple display terminals and real-time state synchronization across stations.

---

## 3. Recommendation & Decision
**Adopt Option C (Configurable Station Routing with Master Expediter)**.

---

## 4. Consequences
- **Positive**: Complete flexibility for any restaurant business model; cooks see only what they need to prepare; runners receive clear alerts when an entire table order is ready.
- **Negative**: Adds configuration requirements during branch onboarding.

---

## 5. Questions & Assumptions
- *Assumption*: Every branch will have at least two station displays (e.g. 1 Food KDS + 1 Bar KDS) and one Master Expediter view.
