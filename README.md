# SmartRestaurant — Hệ Thống Quản Lý Nhà Hàng Thông Minh

Hệ thống quản lý và vận hành nhà hàng số hóa toàn diện (Smart Restaurant Management System) được xây dựng trên nền tảng **.NET 8 / .NET 9 (C#)**, **Entity Framework Core**, **SQL Server** và **SignalR Real-Time**.

---

## 1. Giới Thiệu & Phân Hệ Chức Năng

- **Quản lý Sơ đồ Bàn & Khu vực (Floor & Table Management)**: Trực quan hóa sơ đồ bàn theo tầng/khu vực (Sân vườn, Phòng lạnh, Phòng VIP), cập nhật trạng thái bàn thời gian thực (Trống, Đang phục vụ, Đã đặt, Cần dọn).
- **Điểm Bán Hàng & Tính Tiền (POS & Billing)**: Gọi món siêu tốc cho thu ngân và nhân viên phục vụ; hỗ trợ tách/gộp bàn, chuyển bàn; tính thuế VAT, chiết khấu và tích hợp thanh toán VietQR Napas 24/7 động.
- **Màn Hình Bếp / Bar (Kitchen Display System - KDS)**: Điều phối đơn chế biến theo trật tự FIFO, phân chia trạm bếp/pha chế, cảnh báo trễ món theo thời gian thực.
- **Thực Đơn Số & Tự Gọi Món (Digital Menu & QR Order)**: Khách hàng quét mã QR tại bàn để xem thực đơn và gọi món trực tiếp từ điện thoại.
- **Đồng Bộ Dữ Liệu Thời Gian Thực (SignalR)**: Mọi thao tác đặt bàn, gọi món, bếp hoàn thành và thanh toán đều được đồng bộ tức thời giữa các thiết bị.
- **Quản Lý Định Lượng & Kho Nguyên Liệu (BOM & Inventory)**: Quản lý công thức món ăn, tự động trừ kho nguyên liệu khi món được chế biến, cảnh báo tồn kho tối thiểu.
- **Quản Lý Ca Làm Việc & Phân Quyền (Staff & Shift Management)**: Quản lý ca làm việc, đối soát tiền mặt đầu/cuối ca, phân quyền vai trò (Admin, Manager, Cashier, Waiter, Chef).
- **Báo Cáo & Thống Kê (Analytics & Reports)**: Doanh thu theo ca/ngày/tháng, món bán chạy, hiệu suất bàn.

---

## 2. Kiến Trúc Kỹ Thuật

Dự án áp dụng mô hình **Clean Architecture / Modular Monolith**:
- `SmartRestaurant.Domain`: Định nghĩa Entities, Value Objects, Enums, Domain Exceptions. Không phụ thuộc thư viện ngoài.
- `SmartRestaurant.Application`: Nghiệp vụ ứng dụng, CQRS/Services, DTOs, FluentValidation.
- `SmartRestaurant.Infrastructure`: EF Core DbContext, Migrations, Fluent API Mapping, SignalR Hubs, VietQR Service, Background Workers.
- `SmartRestaurant.Web`: Giao diện ASP.NET Core MVC / Web API, Controllers, Razor Views, SignalR Endpoints.

---

## 3. Quy Chuẩn Dự Án Cho AI & Kỹ Sư

Mọi trợ lý AI và lập trình viên tham gia dự án phải tuân thủ nghiêm ngặt các hướng dẫn tại:
- [AGENTS.md](./AGENTS.md): Quy tắc kỹ thuật, tiêu chuẩn mã nguồn, bảo mật và chỉ thị triển khai chi tiết.
- [GEMINI.md](./GEMINI.md): Chỉ dẫn Antigravity / Gemini cho workspace.
