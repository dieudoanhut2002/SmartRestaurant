# Sổ Đăng Ký Câu Hỏi Mở & Yêu Cầu Hoãn (Open Questions & Deferred Register)
## DỰ ÁN SMARTRESTAURANT — GIAI ĐOẠN 1 (STAGE 1 BASELINE)

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Mã Tài Liệu**: `OPEN-QUESTIONS-AND-DEFERRED-REGISTER`
- **Phiên Bản Cơ Sở**: `v1.0.0-FROZEN`
- **Trạng Thái Quản Trị**: `OWNER-CONFIRMED & SEALED`
- **Tài Liệu Căn Cứ**: [`OPEN-QUESTIONS.md`](./OPEN-QUESTIONS.md), [`OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md), [`OWNER-DECISION-WORKSHOP.md`](./OWNER-DECISION-WORKSHOP.md), [`TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md`](./TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md), [`TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md`](./TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md)

---

## 1. Nguyên Tắc Quản Trị Vòng Đời Quyết Định (Decision Lifecycle Principles)

1. **Thẩm Quyền Tối Cao**: Chỉ có Project Owner mới có quyền chuyển đổi trạng thái từ `OPEN` sang `CONFIRMED` hoặc `DEFERRED`.
2. **Bảo Tồn Lịch Sử & Minh Chứng (Tamper-Evident Auditability)**: Mọi quyết định đều phải gắn liền với mã bằng chứng từ Hội Thảo Quyết Định Task 02.8.5 và Báo Cáo Triển Khai Task 02.8.6.
3. **Phân Định Rạch Ròi Nghiệp Vụ vs Kỹ Thuật**:
   - *Quyết Định Nghiệp Vụ*: Thuộc thẩm quyền của Chủ Dự Án (Project Owner).
   - *Chủ Đề Nghiên Cứu Kỹ Thuật*: Giao cho Kiến trúc sư giải pháp (Solution Architect) điều tra trong Giai đoạn 2 (Stage 2).

---

## 2. Sổ Đăng Ký Quyết Định Của Chủ Dự Án & Câu Hỏi Đã Giải Quyết (Owner Confirmed Register)

Toàn bộ 8 câu hỏi chính sách mở và khung phân hệ đã được Chủ Dự Án chính thức biểu quyết phê duyệt trong Task 02.8.5:

| ID | Nội Dung Chính Sách / Câu Hỏi | Trạng Thái Hiện Tại | Người Có Thẩm Quyền | Yêu Cầu Bị Ảnh Hưởng | Module Bị Ảnh Hưởng | Tác Động Lên MVP | Giai Đoạn Giải Quyết | Bằng Chứng / Nguồn Phê Duyệt |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **`OD-14`** | Xử lý khiếu nại, hủy & hoàn tiền hóa đơn đã thanh toán | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-BIL-005`, `BR-RULE-17` | `MOD-10`, `MOD-13` | CSDL bất biến; hoàn tiền mặt ngoài két kèm ký giấy; log `ShiftClosingVarianceLog`. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OD-14; Task 02.8.5 §2.1 |
| **`OD-15`** | Phân vị Phân hệ Đặt bàn trước (`MOD-06`) trong MVP | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-RES-001..004` | `MOD-06`, `MOD-04` | Đưa vào MVP diện `MVP Optional`; khớp nối lỏng; 1 chạm bật/tắt. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OD-15; Task 02.8.5 §2.1 |
| **`OD-16`** | Áp dụng 3 gói cấu hình mẫu khởi tạo (Presets) | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-SEC-001` | Toàn bộ | Gợi ý cấu hình không ràng buộc trong Wizard: Gia đình, Tiêu chuẩn, VIP. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OD-16; Task 02.8.5 §2.1 |
| **`OD-17`** | Quy tắc thoát nghiệp vụ khi tắt module dở dang | `OWNER-CONFIRMED (Phương án A)` | Project Owner | Toàn bộ | `MOD-06, 09, 11, 12` | Thoát êm dịu (Graceful Drain); bảo tồn 100% dữ liệu lịch sử Read-Only. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OD-17; Task 02.8.5 §2.1 |
| **`OQ-01`** | Tách hóa đơn theo % tùy biến lẻ (60/40) | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-BIL-002, 003` | `MOD-10` | Hoãn % lẻ sang v1.1; MVP hỗ trợ Tách theo món và Chia đều $1/N$. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OQ-01; Task 02.8.5 §2.2 |
| **`OQ-02`** | Tự động in nhiệt bếp song song khi báo bếp | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-KIT-005` | `MOD-08`, `MOD-09` | Mặc định `AutoPrintOnFire: false`, ưu tiên KDS; máy in nhiệt làm dự phòng. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OQ-02; Task 02.8.5 §2.2 |
| **`OQ-06`** | Quy tắc tính thuế VAT trên phí dịch vụ | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-BIL-001` | `MOD-03`, `MOD-10` | Mặc định `IsServiceChargeTaxable: true`, VAT tính trên cả phí dịch vụ. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OQ-06; Task 02.8.5 §2.2 |
| **`OQ-07`** | Bàn ăn kéo dài > 24 giờ qua mốc cắt ca | `OWNER-CONFIRMED (Phương án A)` | Project Owner | `BR-POS-001`, `BR-RULE-18` | `MOD-04`, `MOD-07` | Giữ `BusinessDate` gốc theo D1; hiển thị banner màu cam; không đóng cưỡng bức. | Stage 1 (Task 02.8.5/6) | Workshop Ballot OQ-07; Task 02.8.5 §2.2 |
| **`Khung Phân Hệ`** | Cơ cấu 14 Phân hệ nghiệp vụ | `OWNER-CONFIRMED` | Project Owner | Toàn bộ | Toàn bộ 14 Phân hệ | Phê chuẩn chính thức 10 Core Mandatory + 4 MVP Optional. | Stage 1 (Task 02.8.5/6) | Workshop Ballot Framework; Task 02.8.5 §2.2 |

---

## 3. Sổ Đăng Ký Các Yêu Cầu Hoãn Chính Thức (Confirmed Deferred Requirements)

Các hạng mục đã được Chủ Dự Án xác nhận hoãn sang các phiên bản sau MVP để bảo vệ tiến độ và giảm độ phức tạp:

| ID Yêu Cầu Hoãn | Nội Dung Năng Lực Nghiệp Vụ Hoãn | Phiên Bản / Giai Đoạn Mục Tiêu | Căn Cứ Hoãn Của Chủ Dự Án | Tác Động Phạm Vi MVP | Phân Hệ Liên Quan | Trạng Thái Quản Trị |
| :---: | :--- | :---: | :---: | :--- | :---: | :---: |
| **`DEF-BOM`** | Định lượng nguyên liệu thô theo công thức (BOM gram/ml) | **Phiên bản v1.1** | Quyết định `D5 / PDR-004` | MVP chỉ quản lý hàng đếm được (chai/lon/khẩu phần); giảm áp lực kiểm kê đầu ca. | `MOD-12` | `CONFIRMED DEFERRED (v1.1)` |
| **`DEF-EXPED`** | Màn hình điều phối bàn ra món Master Expediter tại pass | **Phiên bản v1.1** | `PDR-003` | MVP sử dụng KDS trạm bếp và chuông báo đơn giản; đầu bếp tự điều phối món. | `MOD-08, 09` | `CONFIRMED DEFERRED (v1.1)` |
| **`DEF-SPLIT-PCT`**| Tách hóa đơn theo % tùy biến lẻ (60/40, 70/30) | **Phiên bản v1.1** | Quyết định `OQ-01 (Phương án A)` | MVP hỗ trợ Tách theo món và Chia đều $1/N$; tránh sai lệch tiền lẻ VNĐ phức tạp. | `MOD-10` | `CONFIRMED DEFERRED (v1.1)` |
| **`DEF-CREDITNOTE`**| Sinh bút toán điều chỉnh hóa đơn đã đóng số hóa (`CreditNote`) | **Phiên bản v1.1** | Quyết định `OD-14 (Phương án A)` | MVP xử lý sai lệch tiền két bằng biên bản giấy và log chốt ca `ShiftClosingVarianceLog`. | `MOD-10, 13` | `CONFIRMED DEFERRED (v1.1)` |
| **`REV-07`** | Tài khoản công nợ khách hàng doanh nghiệp / Ký nợ (B2B AR)| **Phiên bản v1.2+** | Quyết định `REV-07` | MVP chỉ phục vụ thanh toán dứt điểm trong ca; tách B2B AR thành module sau. | `MOD-10` | `CONFIRMED DEFERRED (v1.2+)` |
| **`DEF-ROTATE`** | Xoay tự do sơ đồ bàn ngoài 4 góc vuông (0°, 90°, 180°, 270°)| **Phiên bản v1.2** | `PDR-002` | Giảm độ phức tạp tương tác canvas Konva.js trong MVP. | `MOD-04` | `CONFIRMED DEFERRED (v1.2)` |
| **`DEF-QR-ORDER`** | Thực khách tự quét QR tại bàn đặt món & thanh toán trước | **Phiên bản v2.0** | `PRD §4.2` | Tách rời nghiệp vụ tự phục vụ; tập trung hoàn thiện POS phục vụ tại bàn cho nhân viên. | `MOD-07, 10` | `CONFIRMED DEFERRED (v2.0)` |
| **`DEF-CHAIN`** | Quản lý chuỗi đa chi nhánh, danh mục tập trung & kho tổng | **Phiên bản v2.0** | `OD-01 / CONTR-01` | Bảo toàn trọng tâm vận hành nhà hàng độc lập đơn điểm trong MVP v1.0. | `MOD-01, 12` | `CONFIRMED DEFERRED (v2.0)` |

---

## 4. Danh Mục Chủ Đề Kỹ Thuật Cần Nghiên Cứu Trong Giai Đoạn 2 (Technical Topics for Stage 2)

Các nội dung dưới đây là **chủ đề kỹ thuật giao cho Solution Architect điều tra trong Stage 2**, KHÔNG PHẢI là quyết định sản phẩm đã chốt:

| Mã Chủ Đề | Tên Chủ Đề Kỹ Thuật Cần Điều Tra | Mục Tiêu Kỹ Thuật Trong Giai Đoạn 2 | Các Phương Án Kỹ Thuật Cần Đánh Giá | Trạng Thái Quản Trị |
| :---: | :--- | :--- | :--- | :---: |
| **`TECH-01`** | Chiến lược kiểm soát đồng thời (Concurrency Strategy) | Xử lý race condition khi nhiều phục vụ gọi món hoặc thu ngân thanh toán cùng lúc. | Optimistic Concurrency `RowVersion` vs Append-Only Delta Events. | `INVESTIGATION REQUIRED — STAGE 2` |
| **`TECH-02`** | Thời điểm kích hoạt khấu trừ tồn kho (Deduction Trigger) | Xác định chính xác sự kiện kích hoạt trừ kho thực tế (`ConsumedStock`) theo D2. | Trigger tại KDS `ReadyToServe` vs Thu ngân `InvoiceSettled` vs Phục vụ `Served`. | `INVESTIGATION REQUIRED — STAGE 2` |
| **`TECH-03`** | Cơ chế đồng bộ thời gian thực (Real-time Mesh) | Đảm bảo độ trễ truyền tin dưới 500ms và khả năng chịu lỗi khi mất kết nối mạng. | SignalR Core Hubs vs Polling vs Local Storage reconciliation. | `INVESTIGATION REQUIRED — STAGE 2` |
| **`TECH-04`** | Ranh giới giao dịch & dữ liệu phân hệ (Transaction Boundaries)| Đảm bảo tính độc lập giữa các module trong Modular Monolith, chống cross-join. | In-Process MediatR Domain Events vs Direct Service Injection. | `INVESTIGATION REQUIRED — STAGE 2` |
| **`TECH-05`** | Cơ chế thực thi Graceful Drain khi chạy (Runtime Drain) | Chặn giao dịch mới và ngắt worker ngầm khi tắt module tùy chọn theo `OD-17`. | ASP.NET Core Middleware vs Action Filter `[RequireModule]` vs IMemoryCache. | `INVESTIGATION REQUIRED — STAGE 2` |
| **`TECH-06`** | Phân vùng đa chi nhánh trong lược đồ CSDL (Partitioning) | Đảm bảo sẵn sàng mở rộng chuỗi mà không ảnh hưởng hiệu năng điểm bán đơn. | Composite Primary Keys (`OrganizationId`, `BranchId`, `EntityId`) vs Global Query Filters. | `INVESTIGATION REQUIRED — STAGE 2` |
