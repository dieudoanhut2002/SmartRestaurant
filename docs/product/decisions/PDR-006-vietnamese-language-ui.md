# Product Decision Record: PDR-006 — 100% Vietnamese Language User Interface

- **Decision ID**: PDR-006
- **Title**: 100% Vietnamese Language Presentation Layer and User Interfaces
- **Status**: `CONFIRMED` (Explicitly mandated by Project Owner)
- **Date**: 2026-11-02
- **Author**: Senior Product Architect & Business Analyst

---

## 1. Context
The target operating environment for SmartRestaurant is primarily the Vietnamese food and beverage (F&B) hospitality industry. Front-of-house servers, busboys, bartenders, line cooks, cashiers, and restaurant managers in Vietnam require an interface in Vietnamese to ensure rapid operations, zero onboarding friction, and clear, unambiguous communication during high-pressure service rushes.

The Project Owner has explicitly instructed: **"Tôi muốn thực hiện toàn bộ giao diện của dự án này hoàn toàn bằng tiếng Việt"** (The entire user interface of this project must be completely in Vietnamese).

---

## 2. Decision & Scope of Enforcement

### 2.1. In-Scope for 100% Vietnamese UI:
All user-facing surfaces across all client applications and presentation layers MUST be displayed entirely in natural, professional Vietnamese:
1. **Admin & Management Portal (MVC / Razor)**:
   - Navigation menus, settings, dashboards, staff rosters, reports, audit logs.
2. **Visual Floor Plan Designer (Konva.js)**:
   - Canvas controls, area tabs, room labels, table statuses: `Trống` (Available), `Có khách` (Occupied), `Đã đặt` (Reserved), `Đang thanh toán` (BillingRequested), `Cần dọn` (CleaningNeeded), `Ngừng sử dụng` (OutOfService).
3. **Waiter Tablet POS**:
   - Touch categories, dish titles, modifier options (e.g., "Ít đá", "Không hành", "Cay vừa"), custom notes, cart actions, table transfer/merge dialogs.
4. **Kitchen Display System (KDS) & Bar Station**:
   - Department titles: "Bếp Nóng", "Bếp Nguội / Salad", "Quầy Pha Chế", "Bếp Bánh / Tráng Miệng", "Bàn Điều Phối Ra Món (Expediter)".
   - Item statuses: "Chờ chế biến", "Đang nấu", "Đã xong (Chờ bưng)", "Đã phục vụ", "Đã hủy món".
   - Time alerts: "Quá giờ chế biến (> 20 phút)".
5. **Cashier Terminal & Settlement**:
   - Bill preview, discount forms, split-bill screens ("Tách theo món", "Tách đều"), cash drawer modals, shift closing ("Mở ca", "Chốt ca", "Kiểm tiền đầu ca", "Phiếu Z-Report").
6. **Physical Thermal Prints**:
   - Pre-bill chits ("Phiếu tạm tính"), VAT invoices ("Hóa đơn thanh toán"), kitchen order tickets ("Phiếu báo chế biến").
7. **System Feedback & Notifications**:
   - SweetAlert2 modal dialogs, error messages, form validation messages, and audio-visual alert notifications.

### 2.2. Engineering & Codebase Boundaries:
- Technical specifications, architecture records, C# class names, database column names, and internal code comments remain in **English** (as per [GEMINI.md](../../GEMINI.md)) to maintain software engineering standards and clean code conventions.
- All user-facing strings will be organized cleanly in localization resource files (`.resx`) or dedicated UI localization dictionaries to avoid hardcoding strings directly into C# business logic.

---

## 3. Consequences
- **Positive**:
  - Maximum usability and zero language barrier for Vietnamese waitstaff, chefs, and cashiers.
  - Aligns with Vietnamese fiscal and accounting receipt requirements.
  - Consistent domain terminology using industry-standard Vietnamese culinary and F&B terms.
- **Negative**:
  - Requires diligent management of Vietnamese UTF-8 character encoding (diacritics) across thermal printers, SMS/push templates, and canvas text rendering.

---

## 4. Confirmation
- **Status**: `CONFIRMED` on 2026-11-02 by Project Owner.
