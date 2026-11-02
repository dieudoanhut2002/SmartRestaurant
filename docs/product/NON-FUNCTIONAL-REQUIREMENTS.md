# Product Non-Functional Requirements (NFR) — SmartRestaurant

- **Document Version**: 1.0.0
- **Document Status**: `CONFIRMED` (Explicitly approved by Project Owner in Task 02.6)
- **Related Documents**: [PRD.md](./PRD.md), [BUSINESS-REQUIREMENTS.md](./BUSINESS-REQUIREMENTS.md), [MODULE-OVERVIEW.md](./MODULE-OVERVIEW.md)

---

## 1. Security & Authorization

### NFR-SEC-01: Role-Based Access Control (RBAC) & Principle of Least Privilege
- **Requirement**: The system must enforce strict role-based authorization across all presentation endpoints and application services.
- **Roles**: `SystemAdmin`, `RestaurantManager`, `FloorCaptain`, `Waiter`, `Chef`, `Cashier`, `Stockkeeper`.
- **Enforcement**: Servers cannot access billing totals or financial reports; cashiers cannot modify recipes or branch tax rates; chefs cannot void bills.

### NFR-SEC-02: Manager Override PIN Authorization
- **Requirement**: High-risk operational actions (bill voids, post-fire dish cancellations, manual discounts, and shift discrepancy overrides) must prompt for a secure 4-digit Manager PIN.
- **Auditability**: Every PIN authorization must log the authorizing Manager ID, timestamp, target entity, and justification reason into an immutable audit table.

### NFR-SEC-03: Protection Against Common Web Vulnerabilities (OWASP Top 10)
- **Anti-CSRF**: Mandatory CSRF token validation (`[ValidateAntiForgeryToken]`) across all mutative HTTP requests (POST, PUT, DELETE).
- **SQL Injection**: Strictly parameterized queries using Entity Framework Core LINQ. Raw string SQL concatenation is completely prohibited.
- **XSS Prevention**: Automatic HTML encoding on Razor views and client-side escaping on Konva.js canvas labels.

---

## 2. Multi-Tenant & Multi-Branch Data Isolation

### NFR-ISO-01: Logical Tenant & Branch Partitioning
- **Requirement**: All transactional tables (`Orders`, `OrderItems`, `Tables`, `DiningSessions`, `Invoices`, `Payments`, `InventoryTransactions`, `Shifts`) must carry `OrganizationId` and `BranchId`.
- **Query Filter Enforcement**: Global query filters in Entity Framework Core must guarantee that queries automatically scope to the active authenticated user's branch context, preventing accidental cross-branch data leakage.
- **Assumption**: Initial deployments operate on a single independent database installation. Multi-branch chain expansion in v2.0 will leverage the already enforced `OrganizationId` and `BranchId` partition keys.

---

## 3. Performance & Latency Targets

| ID | Operational Scope | Target Latency / Throughput | Assumptions & Verification Method |
| :--- | :--- | :--- | :--- |
| **NFR-PERF-01** | POS Touch UI Responsiveness | $\le 100\text{ ms}$ for item tap, modifier modal open, and cart update. | Measured on standard Android/iPad tablets over local 5GHz Wi-Fi. |
| **NFR-PERF-02** | SignalR Order-to-KDS Propagation | $\le 500\text{ ms}$ from waiter tapping "Fire" to appearance on KDS screen. | Measured in typical local restaurant LAN environment with $\le 50$ connected devices. |
| **NFR-PERF-03** | Bill Calculation Engine | $\le 50\text{ ms}$ to compute subtotal, vouchers, service charge, and VAT. | In-memory calculation for bills with up to 100 line items. |
| **NFR-PERF-04** | Floor Plan Live Table Status Sync | $\le 300\text{ ms}$ to reflect color-coded status badge changes across all terminals. | Sub-second visual state broadcast via SignalR `TableHub`. |
| **NFR-PERF-05** | High-Load Peak Concurrency | Sustain up to $50$ concurrent orders and $200$ active tables per branch without degradation. | *Target requiring stress-test confirmation* — [OPEN QUESTION]. |

---

## 4. Concurrency & Data Consistency

### NFR-CONC-01: Optimistic Concurrency Control
- **Requirement**: All critical mutable entities (`Table`, `DiningSession`, `Order`, `Invoice`, `Shift`) must include a byte-array concurrency token (`RowVersion` / `ConcurrencyStamp`).
- **Conflict Handling**: If two staff members attempt to update the same table session simultaneously (e.g. waiter adding an item while cashier requests checkout), the second transaction fails with an explicit concurrency exception, forcing a state refresh rather than silently overwriting data.

### NFR-CONC-02: Atomic Transaction Boundaries
- **Requirement**: Order firing, table moving, table merging, bill splitting, and shift closing must execute within atomic database transactions (`IDbContextTransaction`).
- **All-or-Nothing Guarantee**: If a database write fails during order firing (e.g., ticket creation succeeds but inventory deduction fails), the entire transaction rolls back, preventing orphaned tickets or corrupted billing accounts.

---

## 5. Idempotency & Fault Tolerance

### NFR-IDEM-01: Idempotent Order Dispatch & Payment Webhooks
- **Requirement**: The system must enforce idempotency keys on order submission and payment webhook processing.
- **Duplicate Suppression**: If a waiter taps "Fire Order" multiple times due to a laggy touchscreen or Wi-Fi retry, the backend verifies the idempotency token and ignores duplicate submissions, guaranteeing that the kitchen never receives duplicate tickets for a single intended order.

### NFR-IDEM-02: Automatic SignalR Reconnection & State Resynchronization
- **Requirement**: FOH tablets and KDS screens must implement automated SignalR reconnection routines with exponential backoff (0s, 2s, 5s, 10s).
- **Handshake Resync**: Upon reconnection, the client automatically sends a `SyncStateRequest(LastKnownTimestamp)` to retrieve any table status changes or kitchen tickets missed during the disconnection window.

---

## 6. Audit Logging & Compliance

### NFR-AUD-01: Tamper-Evident Security Audit Trail
- **Requirement**: All destructive, supervisory, or financial actions must generate an unalterable audit log entry containing:
  - Timestamp (UTC + local offset).
  - Actor ID, User Full Name, and Operational Role.
  - Client Terminal IP and Device Fingerprint.
  - Action Category: `BillVoid`, `PostFireCancellation`, `TableMerge`, `ManualDiscount`, `DrawerDiscrepancyOverride`.
  - Old State vs New State (JSON delta).
  - Justification reason code and optional free-form note.

---

## 7. Usability & Device Form Factors

### NFR-USE-01: Form Factor Optimization
- **Responsive Layouts**:
  - **Cashier Terminal**: Optimized for desktop monitors ($1920 \times 1080$ or $1366 \times 768$) with keyboard shortcuts (F1-F12), barcode scanner input, and thermal receipt printer support.
  - **Waiter Tablet POS**: Optimized for 10-inch to 12-inch touch tablets ($1920 \times 1200$ or $1024 \times 768$) in landscape mode. Minimum touch target size: $48 \times 48\text{ px}$ with high visual contrast.
  - **KDS Monitors**: Optimized for 15-inch to 24-inch kitchen touchscreens or HDMI monitors with USB bump bars. High-contrast dark theme designed to be readable from a 2-meter distance in high-glare kitchen environments.

### NFR-USE-02: Accessibility & Audio-Visual Feedback
- Visual color-coded SLA timers must be accompanied by distinct non-jarring audio chimes for new tickets, rush orders, and cancellations to ensure cooks notice alerts even when facing away from the screen.

---

## 8. Localization & Regional Conventions (Vietnamese-First)

### NFR-LOC-01: 100% Vietnamese Language User Experience (CONFIRMED)
- **Mandate**: The entire presentation tier across all client interfaces (Admin Portal, Waiter Tablet POS, Kitchen Display System - KDS, Bar Station, Cashier Terminal, and Customer QR interface) must be presented 100% in natural, professional Vietnamese.
- **Scope**:
  - All navigation menus, forms, buttons, labels, tooltips, placeholders, and table grid headers.
  - All SweetAlert2 toast notifications, confirmation prompts, empty states, and validation feedback.
  - All operational table and kitchen statuses (e.g., `Trống`, `Có khách`, `Đã đặt trước`, `Đang thanh toán`, `Cần dọn dẹp`, `Chờ chế biến`, `Đang nấu`, `Đã xong - Chờ bưng`).
  - All printed materials (Pre-bills / "Phiếu tạm tính", VAT receipts / "Hóa đơn thanh toán", Shift Z-Reports / "Báo cáo chốt ca").
- **Quality Standard**: Zero untranslated English text; strict adherence to [VIETNAMESE-LOCALIZATION-STANDARD.md](./VIETNAMESE-LOCALIZATION-STANDARD.md).

### NFR-LOC-02: Technical Localization & Future Multilingual Readiness
- **Separation of Concerns**: User-facing text must be decoupled from application code and business logic. No hardcoded Vietnamese string literals in C# business services or controllers.
- **Resource Bundles**: Presentation strings managed via strongly-typed ASP.NET Core `.resx` resource files or structured frontend JSON i18n dictionaries.
- **English Engineering Baseline**: Domain entities, database tables, column names, enums (`TableState.Occupied`), API endpoints, and Git logs remain in standard English to maintain Clean Architecture conventions.
- **Extensibility**: System defaults to `vi-VN` (`CultureInfo("vi-VN")`), but allows future introduction of secondary languages (English, Korean, Japanese, Chinese) without refactoring business logic.

### NFR-LOC-03: Vietnam Regional Formats, Timezone & Unicode Input
- **Currency**: Vietnamese Đồng (`₫` / `VND`), formatted with dot thousands separator (`#,,### ₫`), no decimal fractions.
- **Date & Time**: Date formatted as `dd/MM/yyyy`, time formatted in 24h `HH:mm:ss`.
- **Timezone**: Default timezone fixed to `Asia/Ho_Chi_Minh` (UTC+7). Database persists UTC and renders local time.
- **Vietnamese Input & Encoding**: Full UTF-8 support across web, API, and SQL Server (`NVARCHAR` with Vietnamese collation). Smooth compatibility with Telex and VNI input methods across mobile and desktop.

### NFR-LOC-04: Usability for Non-Technical Restaurant Staff
- **Simplicity**: Short, concise Vietnamese labels; avoid jargon and technical exception messages.
- **Touch Ergonomics**: Minimum touch target size $\ge 48 \times 48\text{ px}$ with high visual contrast for fast-paced, greasy, or low-light dining environments.
- **Iconography**: Text paired with universally recognizable icons (e.g., printer icon for "In tạm tính", arrow icon for "Chuyển bàn").

---

## 9. Backup, Recovery & Operational Continuity

### NFR-BCP-01: Disaster Recovery & Automated Backups
- **Database Backups**:
  - Daily full backup + hourly differential backups + 15-minute transaction log backups during operating hours.
  - Target Recovery Point Objective (RPO): $\le 15\text{ minutes}$.
  - Target Recovery Time Objective (RTO): $\le 2\text{ hours}$.
- **Hardware Failure Replacement**: If a waiter tablet or KDS screen breaks, staff can replace the physical tablet with a spare, navigate to the web URL, enter their staff PIN, and immediately resume operations with zero local data loss.
