# Ma Trận Truy Xuất Nguồn Gốc Yêu Cầu Hai Chiều (Requirement Traceability Matrix)
## DỰ ÁN SMARTRESTAURANT — GIAI ĐOẠN 1 (STAGE 1 BASELINE)

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Mã Tài Liệu**: `REQUIREMENT-TRACEABILITY-MATRIX`
- **Phiên Bản Cơ Sở**: `v1.0.0-FROZEN`
- **Trạng Thái Quản Trị**: `OWNER-CONFIRMED & SEALED`
- **Tài Liệu Căn Cứ**: [`BUSINESS-REQUIREMENTS-BASELINE.md`](./BUSINESS-REQUIREMENTS-BASELINE.md), [`BUSINESS-RULES-BASELINE.md`](./BUSINESS-RULES-BASELINE.md), [`MODULE-CAPABILITY-BASELINE.md`](./MODULE-CAPABILITY-BASELINE.md), [`USER-JOURNEYS.md`](./USER-JOURNEYS.md), [`workflows/`](./workflows/), [`decisions/`](./decisions/)

---

## 1. Nguyên Tắc Kiểm Soát Tính Truy Vết Hai Chiều (Traceability Principles)

1. **Nguồn Gốc Minh Bạch (Full Provenance)**: Mọi yêu cầu nghiệp vụ đều phải bắt nguồn từ tài liệu gốc (PRD/BRS) và các quyết định của Chủ Dự Án.
2. **Quy Tắc Đánh Dấu Cảnh Báo (Integrity Flags)**:
   - `UNTRACED`: Yêu cầu không có nguồn gốc tài liệu rõ ràng (Hiện tại: 0 yêu cầu).
   - `CONFLICT`: Yêu cầu phát sinh mâu thuẫn chéo với quy tắc khác (Hiện tại: 0 yêu cầu).
   - `OPEN`: Hạng mục chưa có quyết định cuối cùng.
   - `DEFERRED`: Hạng mục đã được Chủ Dự Án quyết định hoãn sang phiên bản sau.

---

## 2. Ma Trận Truy Xuất Nguồn Gốc Hai Chiều Toàn Diện (End-to-End Matrix)

| Requirement ID | Tóm Tắt Nội Dung Yêu Cầu | Nguồn Tài Liệu | Quyết Định Căn Cứ | PDR Liên Quan | Quy Tắc Nghiệp Vụ | Hành Trình Người Dùng | Quy Trình Vận Hành | Module ID | Phân Loại MVP | Trạng Thái Phê Duyệt | Vấn Đề Mở / Phụ Thuộc Nghiệp Vụ |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **BR-AREA-001** | Quản lý đa khu vực ăn uống riêng biệt | BRS §2.1 | `OD-01` | `PDR-002` | `BR-RULE-01` | `UJ-01, 07` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-002** | Phân cấp phòng độc lập & phòng VIP | BRS §2.1 | `OD-01` | `PDR-002` | `BR-RULE-01` | `UJ-06, 07` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-003** | Canvas sơ đồ bàn 2D Konva.js trực quan | BRS §2.1 | `ADR-004` | `PDR-002` | `BR-RULE-01` | `UJ-06, 07` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-004** | Đặt bao trọn phòng VIP & khóa bàn con | BRS §2.1 | `REV-09` | `PDR-013` | `BR-RULE-01` | `UJ-07, 08` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-005** | Mã màu trực quan hóa trạng thái bàn | BRS §2.1 | `PDR-002` | `PDR-002` | `BR-RULE-01, 06` | `UJ-01, 05, 07` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-006** | Phụ phí phòng VIP / khu vực (% & cố định)| BRS §2.1 | `REV-06` | `PDR-011` | `BR-RULE-13` | `UJ-05, 06` | Cashier Payment Workflow | `MOD-04, 10` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-AREA-007** | Bàn độc lập trong phòng dùng chung | BRS §2.1 | `REV-09` | `PDR-013` | `BR-RULE-01` | `UJ-01, 07` | Floor Plan Workflow | `MOD-04` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-001** | Cấu trúc danh mục thực đơn phân cấp | BRS §2.2 | `PRD §4.3` | `PDR-001` | — | `UJ-01, 06` | Order Workflow | `MOD-05` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-002** | Biến thể kích cỡ khẩu phần (S/M/L) | BRS §2.2 | `PRD §4.3` | `PDR-001` | — | `UJ-01` | Order Workflow | `MOD-05` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-003** | Nhóm tùy chọn món bắt buộc & tự do | BRS §2.2 | `PRD §4.3` | `PDR-001` | — | `UJ-01` | Order Workflow | `MOD-05` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-004** | Ghi chú chế biến đặc biệt cho bếp | BRS §2.2 | `PRD §4.3` | `PDR-003` | — | `UJ-01, 02` | Kitchen Workflow | `MOD-05, 07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-005** | Ánh xạ trạm bếp chế biến tự động | BRS §2.2 | `PDR-003` | `PDR-003` | `BR-RULE-14` | `UJ-02, 04` | Kitchen Workflow | `MOD-08` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-MENU-006** | Phân tuyến trực tiếp quầy FOH bỏ qua KDS| BRS §2.2 | `REV-08` | `PDR-012` | `BR-RULE-14` | `UJ-01, 04` | Order & Kitchen Workflow | `MOD-05, 08` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-001** | Mở phiên dùng bữa tại bàn theo số khách | BRS §2.3 | `PRD §4.3` | `PDR-002` | `BR-RULE-01` | `UJ-01` | Floor Plan Workflow | `MOD-07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-002** | Giao diện gọi món cảm ứng nhanh tại bàn | BRS §2.3 | `PRD §4.3` | `PDR-006` | — | `UJ-01` | Order Workflow | `MOD-07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-003** | Quản lý đợt gọi món (Order Rounds) | BRS §2.3 | `PRD §4.3` | `PDR-003` | — | `UJ-01, 02` | Order Workflow | `MOD-07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-004** | Gọi món đồng thời không khóa bàn | BRS §2.3 | `REV-10` | `PDR-014` | `BR-RULE-16` | `UJ-01` | Order Workflow | `MOD-07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-005** | Bàn tạm tính vẫn cho phép sửa/gọi món | BRS §2.3 | `OD-13 Op2`| `PDR-005` | `BR-RULE-06` | `UJ-01, 05` | Order & Cashier Workflow | `MOD-07, 10` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-006** | Chuyển bàn và gộp bàn | BRS §2.3 | `PRD §4.1` | `PDR-002` | `BR-RULE-04` | `UJ-01, 07` | Floor Plan Workflow | `MOD-04, 07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-POS-007** | Hủy gộp bàn tái phân bổ món không báo lại | BRS §2.3 | `REV-05` | `PDR-010` | `BR-RULE-12` | `UJ-01, 07` | Floor Plan Workflow | `MOD-04, 07` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-RES-001** | Tiếp nhận thông tin đặt bàn & cọc | BRS §2.4 | `OD-15` | `PDR-013` | — | `UJ-08` | Floor Plan Workflow | `MOD-06` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-04 |
| **BR-RES-002** | Khóa bàn đặt trước & đếm ngược giờ đến | BRS §2.4 | `OD-15` | `PDR-002` | `BR-RULE-01` | `UJ-07, 08` | Floor Plan Workflow | `MOD-06` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-04 |
| **BR-RES-003** | Khấu trừ cọc vào hóa đơn thanh toán | BRS §2.4 | `OD-15` | `PDR-005` | — | `UJ-05, 08` | Cashier Payment Workflow | `MOD-06, 10` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-10 |
| **BR-RES-004** | Bật/Tắt module Đặt bàn khớp nối lỏng | BRS §2.4 | `OD-15` | — | — | `UJ-10` | System Config | `MOD-06` | `MVP Optional` | `OWNER-CONFIRMED` | Thoát êm dịu OD-17 |
| **BR-KIT-001** | Hàng đợi thẻ món KDS trạm bếp | BRS §2.5 | `PDR-003` | `PDR-003` | — | `UJ-02, 04` | Kitchen Workflow | `MOD-09` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-08 |
| **BR-KIT-002** | Đồng hồ đếm ngược SLA thẻ món vàng/đỏ | BRS §2.5 | `PDR-003` | `PDR-003` | — | `UJ-02, 03` | Kitchen Workflow | `MOD-09` | `MVP Optional` | `OWNER-CONFIRMED` | Cấu hình phút SLA |
| **BR-KIT-003** | Cảm ứng bếp chuyển trạng thái món | BRS §2.5 | `PDR-003` | `PDR-003` | `BR-RULE-02` | `UJ-02, 03` | Kitchen Workflow | `MOD-09` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-14 |
| **BR-KIT-004** | Làm lại món lỗi bếp không tính tiền | BRS §2.5 | `REV-11` | `PDR-015` | `BR-RULE-15` | `UJ-02, 06` | Kitchen Workflow | `MOD-08, 09` | `MVP Core/Opt` | `OWNER-CONFIRMED` | WasteLog MOD-12 |
| **BR-KIT-005** | Tùy chọn in nhiệt bếp dự phòng | BRS §2.5 | `OQ-02` | `PDR-003` | — | `UJ-02` | Kitchen Workflow | `MOD-08, 09` | `MVP Optional` | `OWNER-CONFIRMED` | AutoPrintOnFire: false |
| **BR-KIT-006** | Báo tạm hết món (86) từ nhà bếp | BRS §2.5 | `PRD §4.4` | `PDR-004` | — | `UJ-02, 01` | Kitchen & Order Workflow | `MOD-09, 14` | `MVP Optional` | `OWNER-CONFIRMED` | Đồng bộ SignalR |
| **BR-BIL-001** | In phiếu tạm tính (Pre-bill) đối chiếu | BRS §2.6 | `PRD §4.6` | `PDR-005` | `BR-RULE-06` | `UJ-05` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-BIL-002** | Tách hóa đơn theo món chọn lọc | BRS §2.6 | `D4` | `PDR-005` | `BR-RULE-11` | `UJ-05` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Không khóa bàn |
| **BR-BIL-003** | Tách hóa đơn chia đều ($1/N$) | BRS §2.6 | `D4` | `PDR-005` | `BR-RULE-11` | `UJ-05` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Dồn tiền lẻ vào bill cuối |
| **BR-BIL-004** | Thanh toán đa phương thức (Multi-tender)| BRS §2.6 | `PDR-005` | `PDR-005` | — | `UJ-05` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-BIL-005** | Hóa đơn đã thanh toán là bất biến CSDL | BRS §2.6 | `OD-14` | `OD-14` | `BR-RULE-17` | `UJ-05, 06` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Lệch két giải quyết ở chốt ca |
| **BR-BIL-006** | Kiểm tiền mù cuối ca (Blind Shift Close)| BRS §2.6 | `PDR-007` | `PDR-007` | `BR-RULE-07` | `UJ-05, 06` | Cashier Payment Workflow | `MOD-10, 13` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-PAY-001** | Sinh mã VietQR động Napas 24/7 tự động | BRS §2.7 | `D3` | `PDR-008` | `BR-RULE-10` | `UJ-05` | Cashier Payment Workflow | `MOD-11` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-10 |
| **BR-PAY-002** | Nhận webhook Napas xác nhận tự động | BRS §2.7 | `D3` | `PDR-008` | `BR-RULE-10` | `UJ-05` | Cashier Payment Workflow | `MOD-11` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-14 |
| **BR-PAY-003** | Dự phòng tiền mặt khi QR timeout | BRS §2.7 | `D3` | `PDR-008` | `BR-RULE-10` | `UJ-05` | Cashier Payment Workflow | `MOD-11` | `MVP Optional` | `OWNER-CONFIRMED` | Hủy mã QR mở |
| **BR-PAY-004** | Sổ treo đối soát VietQR đến muộn | BRS §2.7 | `D3` | `PDR-008` | `BR-RULE-10` | `UJ-05, 06` | Cashier Payment Workflow | `MOD-11` | `MVP Optional` | `OWNER-CONFIRMED` | SuspenseLedger |
| **BR-INV-001** | Khóa tồn kho hai pha cho món đếm được | BRS §2.8 | `D2` | `PDR-004` | `BR-RULE-09` | `UJ-01, 09` | Order & Kitchen Workflow | `MOD-12` | `MVP Optional` | `OWNER-CONFIRMED` | Nghiên cứu trigger trừ kho Stage 2 |
| **BR-INV-002** | Quản lý món đếm được trong MVP | BRS §2.8 | `D5` | `PDR-004` | `BR-RULE-09` | `UJ-09` | Inventory Workflow | `MOD-12` | `MVP Optional` | `OWNER-CONFIRMED` | BOM gram/ml hoãn v1.1 |
| **BR-INV-003** | Ghi sổ hao phí hủy món WasteLog | BRS §2.8 | `PDR-004` | `PDR-004` | `BR-RULE-03` | `UJ-06, 09` | Kitchen Workflow | `MOD-12` | `MVP Optional` | `OWNER-CONFIRMED` | Gắn với mã PIN hủy món |
| **BR-INV-004** | Tự động khóa món (86) khi hết kho | BRS §2.8 | `PDR-004` | `PDR-004` | `BR-RULE-09` | `UJ-01, 09` | Order Workflow | `MOD-12` | `MVP Optional` | `OWNER-CONFIRMED` | Phụ thuộc MOD-14 |
| **BR-REP-001** | Báo cáo doanh thu theo Ngày kinh doanh | BRS §2.9 | `D1` | `PDR-007` | `BR-RULE-08` | `UJ-06, 10` | Reporting | `MOD-13` | `MVP Core` | `OWNER-CONFIRMED` | Giờ chốt 06:00 |
| **BR-REP-002** | Báo cáo chốt ca Z-Report & Lệch két | BRS §2.9 | `PDR-007` | `PDR-007` | `BR-RULE-07, 17` | `UJ-05, 06` | Cashier Payment Workflow | `MOD-10, 13` | `MVP Core` | `OWNER-CONFIRMED` | ShiftClosingVarianceLog |
| **BR-REP-003** | Báo cáo phân tích món bán chạy | BRS §2.9 | `PRD §4.8` | `PDR-001` | — | `UJ-06` | Reporting | `MOD-13` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-REP-004** | Nhật ký lưu vết kiểm toán (Audit Trail)| BRS §2.9 | `PRD §4.8` | `PDR-001` | `BR-RULE-02, 17` | `UJ-06, 10` | System Admin | `MOD-02, 13` | `MVP Core` | `OWNER-CONFIRMED` | Bất biến dữ liệu |
| **BR-LOC-001** | Giao diện 100% Tiếng Việt chuẩn mực | BRS §2.10 | `PDR-006` | `PDR-006` | — | Toàn bộ | Tất cả Giao diện | Toàn bộ | `MVP Core` | `OWNER-CONFIRMED` | CultureInfo vi-VN |
| **BR-LOC-002** | Định dạng số VNĐ & Ngày dd/MM/yyyy | BRS §2.10 | `PDR-006` | `PDR-006` | — | Toàn bộ | Tất cả Giao diện | Toàn bộ | `MVP Core` | `OWNER-CONFIRMED` | Không có số lẻ |
| **BR-LOC-003** | In nhiệt ESC/POS tiếng Việt có dấu | BRS §2.10 | `PDR-006` | `PDR-006` | — | `UJ-02, 05` | Kitchen & Cashier Workflows | `MOD-08, 10` | `MVP Core` | `OWNER-CONFIRMED` | CodePage 1258 / UTF-8 |
| **BR-LOC-004** | Lưu siêu dữ liệu HĐĐT Thông tư 78 | BRS §2.10 | `PRD §4.7` | `PDR-006` | — | `UJ-05` | Cashier Payment Workflow | `MOD-10` | `MVP Core` | `OWNER-CONFIRMED` | Tích hợp API sau MVP |
| **BR-SEC-001** | Ma trận 6 vai trò tiêu chuẩn (RBAC) | BRS §1 | `PRD §4.9` | `PDR-001` | — | Toàn bộ | System Admin | `MOD-02` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-SEC-002** | Đăng nhập nhanh bằng PIN 4 số | BRS §1 | `PRD §4.9` | `PDR-001` | — | `UJ-01, 05` | POS & Cashier Workflows | `MOD-02` | `MVP Core` | `OWNER-CONFIRMED` | Không có |
| **BR-SEC-003** | Phê duyệt thao tác nhạy cảm bằng PIN | BRS §1 | `PRD §4.9` | `PDR-001` | `BR-RULE-02, 17` | `UJ-01, 05, 06` | Tất cả Workflows | `MOD-02` | `MVP Core` | `OWNER-CONFIRMED` | ManagerPinAudit |
| **BR-SEC-004** | Phân vùng đa khách thuê OrganizationId | BRS §1 | `CONTR-01` | `PDR-001` | — | Toàn bộ | Data Layer | `MOD-01` | `MVP Core` | `OWNER-CONFIRMED` | Multi-branch ready |
