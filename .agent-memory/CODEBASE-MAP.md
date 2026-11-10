# SmartRestaurant — Codebase Map

This document provides a navigational map of the **SmartRestaurant** repository structure, modules, entry points, and architectural boundaries.

---

## 1. Physical Directory Structure

```
SmartRestaurant/
├── .agent-memory/              # Persistent AI Agent project memory files
│   ├── README.md               # Memory system index & rules
│   ├── PROJECT-STATUS.md       # Stage, milestone, and risk tracking
│   ├── ARCHITECTURE-DECISIONS.md # Formal ADRs
│   ├── CODEBASE-MAP.md         # Repository map & module catalog
│   ├── DEVELOPMENT-LOG.md      # Chronological task audit log
│   ├── NEXT-TASK.md            # Next approved task specification
│   └── KNOWN-ISSUES.md         # Active technical debt & risks
├── docs/                       # Formal project documentation hierarchy
│   ├── 00-index.md             # Master documentation index
│   ├── PROJECT-GOVERNANCE.md   # Authoritative project governance & stage boundaries
│   ├── PRODUCT-BASELINE-v1.0.0.md # Sole authoritative Stage 1 product specification
│   ├── product/                # Component specifications, workflows, and decisions
│   │   ├── decisions/          # Product Decision Records (PDR-001..015)
│   │   └── workflows/          # Core operational workflows (FOH, POS, BOH, Cashier)
│   └── 99-archive/             # Sealed historical archive (strictly read-only)
│       └── stage-1-history/
│           ├── README.md       # Archive repository index
│           ├── task-reports/   # 14 task execution reports
│           ├── audit-reports/  # 10 independent audit reports
│           └── superseded-documents/ # 5 superseded early drafts
├── .gitignore                  # .NET & IDE exclusion rules
├── AGENTS.md                   # Engineering standards & permanent instructions
├── GEMINI.md                   # Permanent project instructions & governance
└── README.md                   # Public repository overview
```

---

## 2. Planned Solution Architecture (Stage 5 Target)

```
SmartRestaurant.sln
├── src/
│   ├── SmartRestaurant.Domain/         # Core POCO Entities, Value Objects, Enums, Exceptions
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Exceptions/
│   ├── SmartRestaurant.Application/    # Use Cases, DTOs, Service Interfaces, FluentValidation
│   │   ├── Common/
│   │   ├── Features/
│   │   └── Interfaces/
│   ├── SmartRestaurant.Infrastructure/ # EF Core DbContext, Migrations, SignalR Hubs, External APIs
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── Configurations/
│   │   ├── Hubs/
│   │   └── Services/
│   └── SmartRestaurant.Web/            # ASP.NET Core MVC / Web API, Razor Views, Controllers
│       ├── Controllers/
│       ├── ViewModels/
│       ├── Views/
│       └── wwwroot/
└── tests/
    ├── SmartRestaurant.Domain.Tests/
    ├── SmartRestaurant.Application.Tests/
    └── SmartRestaurant.Infrastructure.Tests/
```

---

## 3. Core Module Boundaries & Responsibilities

| Module | Primary Responsibility | Key Entities (Target) |
| :--- | :--- | :--- |
| **Organization & Branches** | Multi-branch tenant configuration & branch-specific rules. | `Organization`, `Branch` |
| **Floor & Tables** | Dining areas, Konva.js visual coordinates, table state transitions. | `DiningArea`, `RestaurantTable`, `TableSession` |
| **Menu & BOM Catalog** | Hierarchical menu, dish variants, modifier groups, raw material BOM. | `Category`, `MenuItem`, `ModifierGroup`, `RecipeBOM` |
| **Orders & Table Service** | Rapid order capture, table order lifecycle, line item prep status. | `Order`, `OrderItem`, `OrderItemModifier` |
| **Kitchen Display (KDS)** | Station ticket queues, timer SLAs, preparation acknowledgement. | `KitchenTicket`, `KitchenStation` |
| **Billing & Payments** | Subtotal, discounts, tax calculation, VietQR payments, split/merge bills. | `Invoice`, `Payment`, `Shift`, `CashDrawer` |
| **Inventory & Purchasing** | Raw materials, unit conversion, stock deduction, waste tracking. | `Ingredient`, `InventoryTransaction`, `WasteLog` |
| **Identity & RBAC** | Staff accounts, roles, granular permission claims. | `AppUser`, `AppRole` |

---

## 4. Key Contracts & Documentation Links
- **Master Product Baseline**: [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md)
- **Project Governance**: [docs/PROJECT-GOVERNANCE.md](../docs/PROJECT-GOVERNANCE.md)
- **Documentation Index**: [docs/00-index.md](../docs/00-index.md)
- **Engineering Instructions**: [AGENTS.md](../AGENTS.md) / [GEMINI.md](../GEMINI.md)
