# SmartRestaurant — Mục Lục Tài Liệu Dự Án (Documentation Index)

Chào mừng bạn đến với mục lục tài liệu tập trung của dự án **SmartRestaurant**. Toàn bộ các đặc tả kỹ thuật, yêu cầu nghiệp vụ, thiết kế kiến trúc và sổ tay vận hành chính thức đều được tổ chức phân cấp tại đây.

---

## 1. Quản Trị Dự Án & Bộ Quy Tắc Chỉ Thị (Governance & Instructions)

| Tài Liệu | Tiêu Đề & Vai Trò | Phạm Vi Quản Trị | Trạng Thái |
| :--- | :--- | :--- | :---: |
| **[GEMINI.md](../GEMINI.md)** | Bộ quy tắc dự án có thẩm quyền cao nhất, quản trị kỹ thuật, chiến lược bộ nhớ và giao thức an toàn. | Quản trị Lõi Toàn Dự Án | `ACTIVE BASELINE` |
| **[AGENTS.md](../AGENTS.md)** | Chuẩn mực kỹ nghệ, quy tắc kiến trúc sạch (Clean Architecture), quy ước viết mã và chỉ thị đầu ra. | Quy Chuẩn Kỹ Nghệ & Chất Lượng | `ACTIVE BASELINE` |
| **[docs/PROJECT-GOVERNANCE.md](./PROJECT-GOVERNANCE.md)** | Quy chế quản trị dự án, ranh giới thẩm quyền, trạng thái giai đoạn (Stage 1/2) và chính sách truy cập archive. | Quy Chế Quản Trị Vòng Đời | `ACTIVE BASELINE` |

---

## 2. Đặc Tả Sản Phẩm Chính Thức Giai Đoạn 1 (Authoritative Stage 1 Product Specification)

> [!IMPORTANT]
> **NGUỒN SỰ THẬT DUY NHẤT (SINGLE SOURCE OF TRUTH):**
> Kể từ cột mốc Task 03.1, tệp tài liệu dưới đây là **nguồn sự thật nghiệp vụ duy nhất** cho toàn bộ Giai đoạn 1. Mọi AI Agent, Solution Architect và Kỹ sư bắt buộc sử dụng tài liệu này làm căn cứ nghiệp vụ chính thức.

| Tài Liệu | Tiêu Đề | Trạng Thái Quản Trị | Mô Tả Phạm Vi |
| :--- | :--- | :---: | :--- |
| **[docs/PRODUCT-BASELINE-v1.0.0.md](./PRODUCT-BASELINE-v1.0.0.md)** | **Hồ Sơ Cơ Sở Đặc Tả Sản Phẩm Chính Thức (Product Specification Baseline v1.0.0-FROZEN)** | **`OWNER-APPROVED & SEALED`** | **Bộ đặc tả nghiệp vụ hợp nhất toàn diện**: Tầm nhìn, bài toán thực tế, phạm vi MVP & loại trừ (`OD-01`), 23 Quyết định đã duyệt của Owner (`OD-01..17`, `D1-D5`, `OQ-01..07`, `REV-05..11`), 18 Quy tắc nghiệp vụ (`BR-RULE-01..18`), 15 Bất biến vận hành (`INV-01..15`), 56 Yêu cầu chức năng (`BR-FOH-001..BR-LOC-001`), 4 Quy trình vận hành cốt lõi, 10 Hành trình người dùng, Từ điển thuật ngữ, Chuẩn bản địa hóa 100% tiếng Việt, Ma trận truy xuất nguồn gốc 12 cột, và Danh mục đề xuất kỹ thuật Stage 2 (`TECH-01..07`). |

---

## 3. Bộ Hồ Sơ Thành Phần & Quy Trình Tham Chiếu (Component Specifications & Workflows)

Dành cho việc tra cứu chi tiết chuyên sâu từng phân hệ, các tệp thành phần cơ sở được lưu giữ có tổ chức tại `docs/product/`:

### 3.1. Quy Trình Vận Hành Thao Tác (Operational Workflows)
- [Quy Trình Quản Lý Sơ Đồ Mặt Bằng & Bàn Ăn](./product/workflows/FLOOR-PLAN-WORKFLOW.md)
- [Quy Trình Bán Hàng POS & Gọi Món](./product/workflows/ORDER-WORKFLOW.md)
- [Quy Trình Vận Hành Bếp & Màn Hình KDS](./product/workflows/KITCHEN-WORKFLOW.md)
- [Quy Trình Thu Ngân, Tính Tiền & Thanh Toán](./product/workflows/CASHIER-PAYMENT-WORKFLOW.md)

### 3.2. Biên Bản Quyết Định Sản Phẩm (Product Decision Records — PDRs)
- [PDR-001: Mô hình triển khai Modular Monolith](./product/decisions/PDR-001-modular-monolith-deployment-model.md)
- [PDR-002: Mô hình không gian phân cấp Khu vực - Phòng - Bàn](./product/decisions/PDR-002-hierarchical-area-room-table-model.md)
- [PDR-003: Điều phối trạm bếp động kèm Master Expediter](./product/decisions/PDR-003-independent-station-kitchen-routing.md)
- [PDR-004: Khóa tồn kho hai pha & Món đếm được trong MVP](./product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md)
- [PDR-005: Quyết toán đa kênh & Tách hóa đơn](./product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md)
- [PDR-006: Giao diện 100% tiếng Việt bản địa (Vietnamese-First)](./product/decisions/PDR-006-vietnamese-language-ui.md)
- [PDR-007: Ngày kinh doanh (Business Date 06:00) cho ca qua đêm](./product/decisions/PDR-007-business-date-shift-management.md)
- [PDR-008: Xử lý webhook VietQR đến muộn qua Sổ Treo Đối Soát](./product/decisions/PDR-008-vietqr-delayed-webhook-handling.md)
- [PDR-009: Phiên bàn đa hóa đơn & Tách bill không khóa bàn](./product/decisions/PDR-009-multi-bill-table-session-split-billing.md)
- [PDR-010: Chính sách hủy gộp bàn và tái phân bổ món](./product/decisions/PDR-010-unmerge-tables.md)
- [PDR-011: Phụ phí khu vực và phòng VIP](./product/decisions/PDR-011-area-and-vip-room-surcharges.md)
- [PDR-012: Điều phối mặt hàng phục vụ sảnh bỏ qua KDS Bar](./product/decisions/PDR-012-direct-service-items-foh.md)
- [PDR-013: Bàn độc lập phòng chung & Đặt bao trọn phòng](./product/decisions/PDR-013-shared-room-and-entire-room-booking.md)
- [PDR-014: Gọi món đồng thời trên cùng bàn không khóa chặn](./product/decisions/PDR-014-concurrent-order-append-business-rules.md)
- [PDR-015: Quy trình làm lại món lỗi bếp 0đ & Sổ hao phí WasteLog](./product/decisions/PDR-015-remake-dish-workflow.md)

---

## 4. Phân Hệ Hồ Sơ Lưu Trữ Lịch Sử Giai Đoạn 1 (Stage 1 Historical Archives)

Toàn bộ 29 báo cáo thực thi nhiệm vụ, biên bản kiểm toán độc lập và tài liệu bản thảo sơ khai đã được lưu trữ có cấu trúc và niêm phong tại:

| Phân Hệ Lưu Trữ | Đường Dẫn | Số Lượng Tệp | Mô Tả & Mục Đích |
| :--- | :--- | :---: | :--- |
| **Mục Lục Hồ Sơ Lưu Trữ** | **[docs/99-archive/stage-1-history/README.md](./99-archive/stage-1-history/README.md)** | 1 tệp | Sổ đăng ký kiểm kê toàn bộ hồ sơ lưu trữ lịch sử Stage 1. |
| **Báo Cáo Thực Thi Nhiệm Vụ** | `docs/99-archive/stage-1-history/task-reports/` | 14 tệp | Các báo cáo thực hiện Task 02.5 đến Task 03.0, biên bản biểu quyết workshop của Owner. |
| **Báo Cáo Kiểm Toán Độc Lập** | `docs/99-archive/stage-1-history/audit-reports/` | 10 tệp | Các báo cáo kiểm toán độc lập Task 02.7.5 đến Task 02.9.3, thẩm định tính toàn vẹn và Clean Pass. |
| **Tài Liệu Đã Bị Thay Thế** | `docs/99-archive/stage-1-history/superseded-documents/` | 5 tệp | Các bản thảo BRS, Scope, Module Overview, Open Questions cũ và bản thảo PRD sơ khai Task 01. |

*(Chính sách truy cập: AI Agent không đọc đệ quy thư mục này trong các phiên làm việc thông thường; chỉ tra cứu khi Chủ Dự Án yêu cầu điều tra bằng chứng lịch sử cụ thể per [docs/PROJECT-GOVERNANCE.md](./PROJECT-GOVERNANCE.md)).*

---

## 5. Kiến Trúc Kỹ Thuật & Thiết Kế Giải Pháp Giai Đoạn 2 (Stage 2 — Sắp Tới)

| Tài Liệu Dự Kiến | Tiêu Đề | Trạng Thái Quản Trị | Giai Đoạn Mục Tiêu |
| :--- | :--- | :---: | :--- |
| *docs/02-architecture/system-architecture.md* | Kiến Trúc Hệ Thống Modular Monolith & Thiết Kế Thành Phần | **`PLANNED / LOCKED`** | Giai đoạn 2 (Chờ chỉ thị riêng của Owner) |
| *docs/02-architecture/data-models-and-erd.md* | Thiết Kế Lược Đồ Quan Hệ & Mô Hình Thực Thể ERD | **`PLANNED / LOCKED`** | Giai đoạn 2 (Chờ chỉ thị riêng của Owner) |
| *docs/02-architecture/realtime-signalr-mesh.md* | Đặc Tả Các Hub SignalR Thời Gian Thực & Hợp Đồng Sự Kiện | **`PLANNED / LOCKED`** | Giai đoạn 2 (Chờ chỉ thị riêng của Owner) |
