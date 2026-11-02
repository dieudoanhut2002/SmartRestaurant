# SmartRestaurant — Permanent Project Instructions & Engineering Rules

Welcome to the **SmartRestaurant** codebase. This document is the authoritative, permanent set of rules and architectural standards that any AI coding assistant or engineer MUST follow when designing, developing, refactoring, and verifying code in this repository.

---

## 1. Project Vision & Domain Overview

**SmartRestaurant** is a modern, enterprise-ready Restaurant Management & Digital Operations System designed to digitize end-to-end food service operations with high reliability, real-time synchronization, and smooth user experience.

### Core Business Subsystems
1. **Table & Floor Management**:
   - Visual floor plan layout categorized by zones/floors (Indoor, Outdoor, VIP, Rooftop, Bar).
   - Real-time table states: `Available` (Trống), `Occupied` (Có khách), `Reserved` (Đã đặt trước), `CleaningNeeded` (Cần dọn dẹp).
   - Table operations: Split bill (Tách hóa đơn), merge tables (Gộp bàn), transfer table (Chuyển bàn).
2. **Digital Menu & Kitchen-Display Catalog**:
   - Multi-category hierarchy (Appetizers, Mains, Hotpot/BBQ, Desserts, Drinks, Combos).
   - Menu item customizers: Variants, portion sizes, add-on toppings, cooking preferences, spicy/sugar ice level, allergen flags.
   - Dynamic pricing (Happy hour, holiday surcharges, seasonal discounts).
3. **Point of Sale (POS) & Billing**:
   - High-speed order entry optimized for both touchscreens (waiter tablets) and desktop (cashier terminals).
   - Bill calculation engine: Subtotal, item-level discounts, order vouchers, service charge, configurable VAT (e.g., 8% or 10%).
   - Multi-channel payments: Cash, Card (POS terminal), and dynamic VietQR / Napas 24/7 QR payment generation with automatic order reconciliation.
4. **Kitchen Display System (KDS) & Bar Station**:
   - Real-time FIFO order queue separated by preparation stations (Hot Kitchen, Cold Kitchen, Bar, Bakery).
   - Preparation stages: `Pending` (Chờ xử lý) -> `Cooking` (Đang chế biến) -> `ReadyToServe` (Đã xong, chờ bưng) -> `Served` (Đã phục vụ).
   - Color-coded wait-time alerts (green: <10m, yellow: 10-20m, red: >20m overdue).
5. **Real-Time Synchronization (SignalR)**:
   - Zero-latency broadcast of table status, new order tickets, dish readiness, and payment updates across POS, KDS, and Waiter apps.
6. **Inventory & Recipe Formulation (Bill of Materials - BOM)**:
   - Raw ingredient catalog with unit conversions (kg, g, liter, ml, pcs).
   - Standard recipe BOM per dish/variant; automatic raw material deduction upon order preparation.
   - Low-stock alerts, waste tracking (hỏng hóc, hủy món có lý do), and stocktake/import history.
7. **Staff & Shift Operations (RBAC & Cash Reconciliation)**:
   - Strict role-based access: `SystemAdmin`, `RestaurantManager`, `Cashier`, `Waiter`, `KitchenStaff`, `InventoryStaff`.
   - Shift management: Opening cash float, recorded cash drops, shift closing balance, and discrepancy reporting.
8. **Reporting & Real-Time Analytics**:
   - Revenue, profit margin, top-selling items, hourly peak load, table turnover rate, and average ticket size.

---

## 2. Technology Stack & Framework Standards

- **Language & Runtime**: C# .NET 8 / .NET 9 LTS (C# 12 / 13 features enabled: primary constructors, collection expressions, pattern matching).
- **Architecture**: Clean Architecture / Modular Monolith:
  - `SmartRestaurant.Domain`: Enterprise entities, Value Objects, Domain Enums, Domain Exceptions. Zero third-party or framework dependencies.
  - `SmartRestaurant.Application`: Application logic, CQRS/Service interfaces, DTOs, Business Rules, FluentValidation validators.
  - `SmartRestaurant.Infrastructure`: Entity Framework Core DbContext, Migrations, SignalR Hubs implementation, Background Workers, Payment integration (VietQR/VietQR API), File storage, Email/Notification providers.
  - `SmartRestaurant.Web` (or `SmartRestaurant.API`): ASP.NET Core MVC / Web API, Razor Views, Controllers, Middlewares, SignalR endpoints, ViewModels.
- **ORM & Data Access**: Microsoft Entity Framework Core with Microsoft SQL Server (LocalDB in development, SQL Server in staging/production).
- **Real-Time Engine**: ASP.NET Core SignalR.
- **UI Framework & Design System**: Modern, clean, responsive UI built with Bootstrap 5.3+, Bootstrap Icons / FontAwesome 6, SweetAlert2 (toast & modal confirmations), DataTables.net, and Chart.js.
- **Logging**: Serilog (structured console and rolling file logs).
- **Background Jobs**: Hosted `BackgroundService` or Hangfire for recurring jobs (e.g., daily sales summaries, inventory threshold checks).
- **Testing**: xUnit, FluentAssertions, Moq.

---

## 3. Architecture & Code Design Rules

### 3.1. Clean Separation of Concerns
- **Domain Layer**:
  - Keep domain entities clean and POCO.
  - All entities inherit from a shared `BaseEntity` (containing `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`).
  - Do NOT clutter domain entities with EF Core or UI attributes (e.g., avoid `[Table]`, `[Column]`, `[Required]` in Domain; use Fluent API in Infrastructure instead).
- **Application Layer**:
  - Keep business logic inside Application services or Command/Query handlers.
  - Use DTOs/ViewModels for data exchange; NEVER expose raw Domain entities directly to MVC Views or API responses.
  - Validate all inputs using FluentValidation or model validation before hitting business logic.
- **Infrastructure Layer**:
  - Encapsulate EF Core mappings inside dedicated `IEntityTypeConfiguration<T>` classes located in `Infrastructure/Data/Configurations/`.
  - Use Global Query Filters on `BaseEntity` to implement transparent Soft Deletes (`IsDeleted == false`).
- **Web / Presentation Layer**:
  - Controllers must remain lightweight (thin controllers): extract parameters, call service/mediator, return result.
  - Use standard response formats (`ApiResponse<T>` or `Result<T>`) for API and clean ViewModels for Razor Views.

### 3.2. Concurrency & Data Integrity
- Restaurant ordering has high concurrent writes (multiple waiters adding dishes to the same table, cashier checking out simultaneously).
- Use **Optimistic Concurrency Control** (`[Timestamp]` / `RowVersion` / `byte[] ConcurrencyStamp`) on `Order` and `Table` entities.
- Wrap order submission, bill splitting, and table merging in EF Core database transactions (`IDbContextTransaction`) to prevent partial state corruption.

### 3.3. Asynchronous Execution
- All database queries, file I/O, external network requests (VietQR, notifications), and SignalR broadcasts MUST be `async`/`await`.
- Pass `CancellationToken` through all asynchronous call chains to allow graceful request cancellation.
- NEVER block threads using `.Result`, `.Wait()`, or `Thread.Sleep()`.

---

## 4. Coding Standards & Conventions

1. **Naming Conventions**:
   - `PascalCase`: Classes, interfaces (`IOrderService`), methods, properties, public constants, enums, record types.
   - `camelCase`: Method parameters, local variables, private fields with underscore prefix (`_dbContext`, `_logger`).
   - Use descriptive, intention-revealing names (`CalculateFinalBillAmountAsync` instead of `CalcBill`).
2. **Error Handling**:
   - Use custom domain exceptions (`TableAlreadyOccupiedException`, `InsufficientStockException`, `OrderAlreadyPaidException`).
   - Implement a centralized Exception Handling Middleware that logs errors with correlation IDs and returns user-friendly messages.
   - Never catch general `Exception` and swallow it silently (`catch { }` is strictly prohibited).
3. **Database Migrations & Seed Data**:
   - Always keep EF Core Migrations consistent.
   - Provide an automatic, idempotent Database Seeder that runs on startup if the database is uninitialized. Seed data MUST include:
     - Default roles (`Admin`, `Manager`, `Cashier`, `Waiter`, `Chef`).
     - Realistic admin and staff test accounts.
     - Representative dining areas (Tầng 1 - Sân vườn, Tầng 2 - Phòng lạnh, Tầng 3 - Phòng VIP) and tables with diverse capacities (2, 4, 6, 10 pax).
     - Rich menu categories and dishes with realistic names, prices, descriptions, and mock/placeholder image paths.

---

## 5. Security & Safety Protocols

- **Cross-Site Request Forgery (CSRF)**: Enforce `[ValidateAntiForgeryToken]` or `[AutoValidateAntiforgeryToken]` on all POST, PUT, DELETE endpoints.
- **SQL Injection Prevention**: Always use EF Core LINQ queries or parameterized queries. Raw string concatenation in SQL is strictly forbidden.
- **Cross-Site Scripting (XSS)**: Rely on Razor's automatic HTML encoding. When rendering raw HTML, sanitize inputs thoroughly.
- **Authentication & RBAC**:
  - Protect all administration, cashier, and kitchen endpoints with explicit `[Authorize(Roles = "...")]` or policy attributes.
  - Ensure users cannot access or tamper with orders outside their restaurant/tenant boundaries.
- **Secrets Management**:
  - Never commit real passwords, API keys, or production connection strings to Git. Use `appsettings.json` with placeholders and override via environment variables or User Secrets.

---

## 6. Implementation Quality Mandate (Strictly Enforced)

1. **No Stubs or Mock Placeholders**:
   - When writing code, write COMPLETE, PRODUCTION-READY implementations.
   - Do NOT leave `// TODO: Implement later`, empty methods, or `throw new NotImplementedException()`.
   - All forms must validate inputs, submit data, handle errors, and show feedback.
2. **Build Verification Rule**:
   - After creating or editing code, ALWAYS run `dotnet build` to verify there are zero build errors.
   - If tests exist, run `dotnet test` to ensure all tests pass.
3. **Documentation Integrity**:
   - Keep user manuals, API documentation, and setup instructions up-to-date in `README.md` and `HUONGDAN.txt`.
