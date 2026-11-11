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
├── src/                        # Core Application Source Code (.NET 8 LTS)
│   ├── SmartRestaurant.Domain/         # Clean POCO domain entities, enums, domain exceptions
│   ├── SmartRestaurant.Application/    # Use cases, DTOs, service interfaces & implementations
│   ├── SmartRestaurant.Infrastructure/ # EF Core DbContext, entity configs, LocalDB seeder
│   └── SmartRestaurant.Web/            # ASP.NET Core MVC Manager Application, Razor views, Konva.js
├── tests/                      # Automated Unit & Integration Test Suites
│   ├── SmartRestaurant.Domain.Tests/   # Domain invariant & state transition unit tests
│   └── SmartRestaurant.Application.Tests/ # Service contracts, PIN authorization & shift tests
├── SmartRestaurant.slnx        # Solution file
├── .gitignore                  # .NET & IDE exclusion rules
├── AGENTS.md                   # Engineering standards & permanent instructions
├── GEMINI.md                   # Permanent project instructions & governance
└── README.md                   # Public repository overview
```

---

## 2. Active Solution Architecture

```
SmartRestaurant.slnx
├── src/
│   ├── SmartRestaurant.Domain/
│   │   ├── Common/ (BaseEntity, IAggregateRoot)
│   │   ├── Entities/ (Branch, DiningArea, DiningRoom, RestaurantTable, KitchenStation, MenuItem, MenuItemVariant, ModifierGroup, ModifierOption, InventoryItem, InventoryTransaction, StaffMember, WorkShift, ShiftClosingVarianceLog, ManagerPinAuditLog, KitchenWasteLog, DiningSession)
│   │   ├── Enums/ (AreaType, TableShape, TableStatus, PrepStation, StaffRole, ShiftStatus, DiningSessionStatus, InventoryTransactionType)
│   │   └── Exceptions/ (DomainException, DomainValidationException, DomainNotFoundException, InvalidTableStateTransitionException)
│   ├── SmartRestaurant.Application/
│   │   ├── Common/Interfaces/ (IAppDbContext)
│   │   ├── DTOs/ (FloorPlanDtos, MenuDtos, StationDtos, InventoryDtos, SettingDtos, StaffAndShiftDtos, DashboardAndReportDtos)
│   │   ├── Interfaces/ (IFloorPlanService, IMenuService, IKitchenStationService, IInventoryService, ISettingService, IStaffService, IShiftService, IDashboardService, IReportService)
│   │   ├── Services/ (FloorPlanService, MenuService, KitchenStationService, InventoryService, SettingService, StaffService, ShiftService, DashboardService, ReportService)
│   │   └── DependencyInjection.cs
│   ├── SmartRestaurant.Infrastructure/
│   │   ├── Data/ (AppDbContext, DatabaseSeeder)
│   │   ├── Data/Configurations/ (EntityConfigurations)
│   │   └── DependencyInjection.cs
│   └── SmartRestaurant.Web/ (Manager Application Portal)
│       ├── Controllers/ (DashboardController, FloorPlanController, MenuController, StationController, InventoryController, SettingController, StaffController, ShiftController, ReportController)
│       ├── Views/ (Dashboard, FloorPlan, Menu, Station, Inventory, Setting, Staff, Shift, Report, Shared)
│       ├── wwwroot/ (css/admin.css, js/floorplan-designer.js, js/admin.js)
│       └── Program.cs
└── tests/
    ├── SmartRestaurant.Domain.Tests/ (DiningAreaTests, RestaurantTableTests)
    └── SmartRestaurant.Application.Tests/ (FloorPlanServiceTests, MenuServiceTests, KitchenStationServiceTests, InventoryServiceTests, SettingServiceTests, StaffAndShiftServiceTests)
```

---

## 3. Manager Application Features & Endpoints

| Feature Domain | Primary Controller | Primary View / UI | Core Capabilities |
| :--- | :--- | :--- | :--- |
| **Dashboard** | `DashboardController` | `Views/Dashboard/Index.cshtml` | KPIs (Revenue, Occupancy, Open Shifts, Low Stock), Chart.js hourly revenue, Top selling dishes, Recent PIN/Waste activity |
| **Sơ đồ bàn 2D** | `FloorPlanController` | `Views/FloorPlan/Index.cshtml` | Konva.js visual canvas, area tabs, dining room & VIP surcharge management, table drag & drop position persistence, quick table status toggles, add/edit table |
| **Thực đơn & Món** | `MenuController` | `Views/Menu/Index.cshtml` | Category tabs, dish CRUD, portion variants (S/M/L), modifier groups & options (mức cay, đường đá, topping), 1-click "86" toggle, stock adjustment |
| **Trạm Chế Biến** | `StationController` | `Views/Station/Index.cshtml` | Kitchen stations catalog (`HotKitchen`, `ColdKitchen`, `Bar`, `Bakery`), printer IPs, dish routing matrix, unified KDS support |
| **Quản Lý Kho** | `InventoryController` | `Views/Inventory/Index.cshtml` | Countable items tracking, 4 states (`Physical`, `Reserved`, `Consumed`, `Waste`) + invariant `Available = Physical - Reserved`, stock receipt, stock adjustment, audit log |
| **Cấu Hình Nhà Hàng** | `SettingController` | `Views/Setting/Index.cshtml` | Business day cutoff (`06:00`), VIP room surcharges (% and fixed amount), 4 MVP optional module toggles, 3 onboarding presets (`Cafe`, `Alacarte`, `BeerPub`) |
| **Nhân viên & PIN** | `StaffController` | `Views/Staff/Index.cshtml` | 6 standard RBAC roles, staff directory, 4-digit PIN management & verification, manager PIN authorization tester |
| **Ca & Két tiền** | `ShiftController` | `Views/Shift/Index.cshtml` | Open shift with cash float, blind cash close modal (`CloseShiftBlind`), variance calculation, variance audit log table |
| **Báo cáo & Kiểm toán** | `ReportController` | `Views/Report/Index.cshtml` | Daily sales by `BusinessDate` (06:00 cutoff), payment tender breakdown (Cash/VietQR/Card), `WasteLog`, `ManagerPinAuditLog` |

---

## 4. Key Contracts & Documentation Links
- **Master Product Baseline**: [docs/PRODUCT-BASELINE-v1.0.0.md](../docs/PRODUCT-BASELINE-v1.0.0.md)
- **Project Governance**: [docs/PROJECT-GOVERNANCE.md](../docs/PROJECT-GOVERNANCE.md)
- **Documentation Index**: [docs/00-index.md](../docs/00-index.md)
- **Engineering Instructions**: [AGENTS.md](../AGENTS.md) / [GEMINI.md](../GEMINI.md)
