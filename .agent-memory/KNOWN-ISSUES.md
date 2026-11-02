# SmartRestaurant — Known Issues & Technical Debt

This file tracks active bugs, technical debt, operational caveats, and unresolved architectural risks for **SmartRestaurant**.

---

## 1. Unresolved Architectural Risks & Open Questions

| Item ID | Category | Summary | Related Modules | Status | Workaround / Mitigation |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **ISSUE-01** | Open Question | Bill splitting: Clarification needed whether arbitrary percentage-based splits (e.g. 60/40) are required in addition to item-based and equal splits (`OQ-01`). | Billing & Payments | `OPEN` | Current PRD supports item-based and equal ($1/N$) splits. |
| **ISSUE-02** | Open Question | Multi-branch inventory: Clarification needed whether centralized central warehouse stock transfer workflows should be supported in Phase 1 (`OQ-02`). | Inventory & Purchasing | `OPEN` | PRD currently isolates warehouse records per branch. |
| **ISSUE-03** | Operational Risk | Local network latency or packet drops during peak dining hours in brick-and-mortar restaurant environments (`RSK-02`). | SignalR Real-Time, KDS, POS | `TRACKING` | SignalR client-side auto-reconnect and reconnect handshake state synchronization will be architected in Stage 2. |
| **ISSUE-04** | Data Integrity | Concurrency collisions if multiple waitstaff and cashier update an open table simultaneously (`RSK-01`). | Orders, Tables, Billing | `TRACKING` | EF Core `RowVersion` optimistic concurrency tokens and database transactions specified in PRD. |

---

## 2. Technical Debt & Defects
- *No code defects or technical debt currently logged (Project is in Stage 1 Requirements Phase).*
