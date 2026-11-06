# Hồ Sơ Bàn Giao Thiết Kế Kỹ Thuật Giai Đoạn 2 (Stage 2 Product Handoff Package)
## DÀNH CHO KIẾN TRÚC SƯ GIẢI PHÁP (SOLUTION ARCHITECT) — DỰ ÁN SMARTRESTAURANT

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Giai Đoạn Bàn Giao**: Từ Giai đoạn 1 (Product Discovery) sang Giai đoạn 2 (Solution Architecture & Technical Design)
- **Mã Tài Liệu**: `STAGE-2-PRODUCT-HANDOFF`
- **Phiên Bản Cơ Sở**: `v1.0.0-FROZEN`
- **Ngày Bàn Giao**: 2026-11-06
- **Trạng Thái Quản Trị**: `PACKAGE PREPARED & READY FOR ARCHITECT REVIEW (PENDING EXPLICIT OWNER AUTHORIZATION)`
- **Phân Định Trạng Thái Quản Trị**:
  1. *Documentation Consolidation*: `COMPLETED`
  2. *Product Specification*: `REVIEWED`
  3. *Stage 1 Baseline*: `OWNER-APPROVED`
  4. *Stage 2 Technical Design*: `PENDING EXPLICIT PROJECT OWNER AUTHORIZATION`
- **Tài Liệu Căn Cứ Hợp Nhất**:
  1. [`PRODUCT-SPECIFICATION-BASELINE.md`](./PRODUCT-SPECIFICATION-BASELINE.md) (Tổng quan cơ sở đặc tả sản phẩm)
  2. [`MVP-SCOPE-BASELINE.md`](./MVP-SCOPE-BASELINE.md) (Phạm vi MVP & Phân kỳ lộ trình)
  3. [`BUSINESS-REQUIREMENTS-BASELINE.md`](./BUSINESS-REQUIREMENTS-BASELINE.md) (Danh mục yêu cầu nghiệp vụ)
  4. [`BUSINESS-RULES-BASELINE.md`](./BUSINESS-RULES-BASELINE.md) (Bộ 18 quy tắc nghiệp vụ bất biến)
  5. [`MODULE-CAPABILITY-BASELINE.md`](./MODULE-CAPABILITY-BASELINE.md) (14 phân hệ & bản đồ năng lực)
  6. [`REQUIREMENT-TRACEABILITY-MATRIX.md`](./REQUIREMENT-TRACEABILITY-MATRIX.md) (Ma trận truy vết hai chiều)
  7. [`OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md`](./OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md) (Sổ quyết định & câu hỏi mở)

---

## 1. Bối Cảnh Nghiệp Vụ (Business Context)

### 1.1. Tầm Nhìn Sản Phẩm
SmartRestaurant là giải pháp phần mềm quản lý vận hành nhà hàng F&B chuyên sâu tại Việt Nam, xây dựng theo mô hình **Modular Monolith** hiện đại. Hệ thống giải quyết bài toán cốt lõi: tối ưu hóa tốc độ ghi nhận món tại bàn, phân tuyến chế biến tức thì xuống các trạm bếp/bar, kiểm soát chặt chẽ thất thoát tồn kho và quỹ két tiền mặt, đồng thời cung cấp trải nghiệm thanh toán tự động qua mã VietQR Napas 24/7.

### 1.2. Đối Tượng Người Dùng Mục Tiêu
- **Nhân viên phục vụ (Waiter)**: Sử dụng tablet POS cảm ứng di động, cần tốc độ thao tác cực nhanh (< 30s/đơn), không gián đoạn khi mạng Wi-Fi chập chờn.
- **Đầu bếp & Pha chế (Chef/Bartender)**: Sử dụng màn hình cảm ứng KDS chống nước/dầu mỡ hoặc máy in nhiệt, cần giao diện thẻ món to rõ, cảnh báo SLA theo màu.
- **Thu ngân (Cashier)**: Sử dụng máy POS cố định tại quầy, két tiền mặt và máy in hóa đơn 80mm, yêu cầu tính toán tiền chính xác từng đồng, kiểm tiền mù chốt ca Z.
- **Quản lý & Chủ nhà hàng (Manager/Owner)**: Sử dụng cổng quản trị Web Razor MVC để thiết lập mặt bằng 2D, giám sát doanh số, duyệt mã PIN và đối soát thất thoát.

### 1.3. Phạm Vi & Ranh Giới Vận Hành MVP (MVP Boundary)
- **Tập trung vào 1 nhà hàng đơn lẻ**: Bản v1.0 MVP chỉ phục vụ 1 nhà hàng độc lập tại 1 địa điểm vật lý (`OD-01`).
- **Không quản lý chuỗi trong MVP**: Không có logic điều chuyển kho liên chi nhánh, không có báo cáo tài chính hợp nhất chuỗi.
- **Sẵn sàng mở rộng trong CSDL**: Khóa ngoại `OrganizationId` và `BranchId` có mặt trên mọi bảng từ ngày đầu (`CONTR-01`).

---

## 2. Bối Cảnh Chức Năng (Functional Context)

### 2.1. Nhóm Năng Lực Nghiệp Vụ Cốt Lõi (14 Capabilities)
Hệ thống bao gồm 14 nhóm năng lực: Cấu hình điểm bán (`CAP-01`), Phân quyền RBAC & PIN (`CAP-02`), Sơ đồ bàn 2D (`CAP-03`), Đặt bàn trước (`CAP-04`), Quản lý thực đơn (`CAP-05`), Bán hàng POS (`CAP-06`), Điều phối bếp (`CAP-07`), Màn hình KDS (`CAP-08`), Thu ngân & Hóa đơn (`CAP-09`), Thanh toán & VietQR (`CAP-10`), Tồn kho món đếm được (`CAP-11`), Quản lý nhân viên & PIN (`CAP-12`), Báo cáo doanh thu (`CAP-13`), Đồng bộ thời gian thực (`CAP-14`).

### 2.2. Các Quy Trình Vận Hành Cốt Lõi (Core Workflows)
1. **Quy trình Sơ đồ mặt bằng & Phiên bàn**: Mở bàn $\rightarrow$ Đổi màu trạng thái $\rightarrow$ Chuyển bàn / Gộp bàn / Hủy gộp $\rightarrow$ Bao trọn phòng VIP $\rightarrow$ Cần dọn dẹp $\rightarrow$ Bàn trống.
2. **Quy trình Gọi món & Điều phối bếp**: Bồi bàn chọn món $\rightarrow$ Thêm đợt món $\rightarrow$ Bắn bếp (tạm giữ kho) $\rightarrow$ Động cơ phân tách trạm $\rightarrow$ Thẻ KDS trạm bếp.
3. **Quy trình Chế biến tại bếp**: Nhận nấu $\rightarrow$ Bếp nấu xong $\rightarrow$ Chuông báo expediter $\rightarrow$ Bưng món ra bàn. Ngoại lệ: Làm lại món lỗi bếp (`IsRemake = true`, giá 0đ, thẻ đỏ ưu tiên, ghi nhận `WasteLog`).
4. **Quy trình Thu ngân & Thanh toán**: In tạm tính (`BillingRequested`) $\rightarrow$ Khách gọi thêm món (nối tiếp vào bill hoặc tạo sub-bill) $\rightarrow$ Tách bill (theo món hoặc chia đều $1/N$) $\rightarrow$ Thanh toán đa phương thức (Tiền mặt / Thẻ / VietQR) $\rightarrow$ Đóng bill (`IsSettled = true`, bất biến CSDL) $\rightarrow$ Giải phóng bàn.

### 2.3. Các Kịch Bản Ngoại Lệ Nghiệp Vụ Trọng Yếu
- **Hủy món sau khi đã bắt đầu nấu**: Bắt buộc nhập mã PIN Quản lý, chọn lý do hủy; tự động ghi nhận tổn thất vào `WasteLog` (`BR-RULE-02, 03`).
- **Khách bùng tiền (Walkout)**: Quản lý nhập mã PIN xác nhận chuyển đơn thành nợ khó đòi; hạch toán tổn thất; giải phóng bàn (`BRS §4`).
- **Chuyển tiền mặt khi QR chờ lâu và webhook đến muộn**: Webhook chuyển vào `SuspenseLedger` kèm cảnh báo âm thanh/hình ảnh; không cộng trùng tiền két (`BR-RULE-10`).
- **Sai lệch tiền két khi thu ngân trả nhầm tiền**: Hóa đơn đã đóng không được sửa; tiền lệch xử lý ngoài quầy bằng biên bản giấy; ghi nhận vào `ShiftClosingVarianceLog` khi kiểm tiền mù chốt ca (`BR-RULE-17`).

---

## 3. Thuộc Tính Chất Lượng & Yêu Cầu Phi Chức Năng (Quality Attributes & NFRs)

Các yêu cầu phi chức năng được trích xuất từ tài liệu NFR hiện hành của Stage 1:

| Thuộc Tính Chất Lượng | Yêu Cầu Nghiệp Vụ Căn Cứ | Chỉ Tiêu Cần Đạt (Trích Xuất Từ NFR Hiện Hành) |
| :--- | :--- | :--- |
| **Hiệu Năng (Performance)** | Phản hồi giao diện POS chạm cảm ứng mượt mà | Thao tác chọn món, đổi danh mục dưới **100ms**; tìm kiếm món dưới **100ms**; tải sơ đồ 50 bàn dưới **1.0s**. |
| **Đồng Bộ Thời Gian Thực** | Bắn phiếu bếp & đổi màu bàn tức thì | Độ trễ thông điệp phát sóng qua mạng nội bộ dưới **500ms** (`NFR-PERF-02`). |
| **Kiểm Soát Đồng Thời** | Tránh ghi đè dữ liệu khi nhiều bồi bàn thao tác | Xử lý race condition cấp dòng món ăn; append-only an toàn; khóa đơn nguyên thanh toán hóa đơn. |
| **Tính Sẵn Sàng (Availability)** | Chịu lỗi khi mạng Wi-Fi bị gián đoạn tạm thời | Đệm dữ liệu thao tác trong `LocalStorage`; tự động bắt tay đồng bộ hóa khi có mạng lại, không sinh trùng thẻ. |
| **An Toàn & Bảo Mật (Security)** | Kiểm soát gian lận tiền mặt & phân quyền | RBAC 6 vai trò; đăng nhập nhanh bằng PIN 4 số; lưu vết kiểm toán bất biến cho mọi thao tác duyệt PIN. |
| **Bản Địa Hóa (Localization)** | Trải nghiệm 100% Tiếng Việt bản địa chuẩn | Giao diện tiếng Việt; format VNĐ `#,,### ₫`; ngày tháng `dd/MM/yyyy`; in nhiệt tiếng Việt ESC/POS CodePage 1258. |

---

## 4. Các Ràng Buộc Nghiệp Vụ Bắt Buộc (Confirmed Business Constraints)

Kiến trúc sư giải pháp trong Stage 2 **bắt buộc phải tuân thủ tuyệt đối các ràng buộc nghiệp vụ sau**:
1. **`D1`**: Giờ chốt ngày mặc định `06:00:00`. `BusinessDate` chụp ảnh lúc tạo đơn hàng. Thay đổi giờ cắt không tính toán lại dữ liệu cũ. Phiên bàn > 24h giữ nguyên ngày kinh doanh ban đầu kèm banner cam (`OQ-07`).
2. **`D2`**: `Tồn khả dụng = Tồn thực tế - Tạm giữ`. Phân định rạch ròi Tạm giữ, Tiêu hao thực tế, Hao hụt và Khấu trừ kho.
3. **`D3`**: Webhook VietQR đến muộn tự động đi vào Sổ Treo `SuspenseLedger`; không ghi nhận trùng; không tự ý hoàn tiền.
4. **`D4`**: Hỗ trợ nhiều hóa đơn con trên một phiên bàn; tách bill không khóa bàn; khách tiếp tục gọi thêm món vào hóa đơn chưa thanh toán.
5. **`D5`**: MVP chỉ quản lý tồn kho món đếm được; định lượng nguyên liệu thô BOM gram/ml hoãn sang v1.1.
6. **`OD-13 Option 2`**: Bàn `BillingRequested` vẫn cho phép chỉnh sửa hoặc gọi thêm món; bàn giải phóng khi 100% hóa đơn con đã thanh toán.
7. **`OD-14 Option A`**: Hóa đơn đã thanh toán (`IsSettled = true`) là bất biến tuyệt đối trong CSDL; sai lệch quỹ két ghi vào `ShiftClosingVarianceLog` tại thời điểm kiểm tiền mù cuối ca; bút toán số hóa `CreditNote` hoãn sang v1.1.
8. **`OD-15 Option A`**: Phân hệ Đặt bàn trước `MOD-06` là `MVP Optional`, thiết kế khớp nối lỏng qua `TableId` có thể null.
9. **`OD-16 Option A`**: 3 Onboarding Presets (Gia đình, Tiêu chuẩn, VIP) là gợi ý cấu hình không ràng buộc.
10. **`OD-17 Option A`**: Chính sách Graceful Drain khi tắt module; 100% dữ liệu lịch sử bảo toàn Read-Only.
11. **`OQ-02 Option A`**: Mặc định ưu tiên KDS điện tử `AutoPrintOnFire: false`; máy in nhiệt ESC/POS làm dự phòng.
12. **`OQ-06 Option A`**: Thuế VAT mặc định tính trên cả phí dịch vụ `IsServiceChargeTaxable: true`.

---

## 5. Danh Mục Chủ Đề Kỹ Thuật Giao Cho Solution Architect Nghiên Cứu Trong Stage 2

> [!IMPORTANT]
> **LƯU Ý QUẢN TRỊ DÀNH CHO SOLUTION ARCHITECT**:  
> Các công nghệ và giải pháp kỹ thuật như `RowVersion`, `Append-Only Delta`, `SignalR`, `Table Partitioning`, `IMemoryCache` liệt kê dưới đây **CHỈ LÀ CÁC CHỦ ĐỀ NGHIÊN CỨU & ĐÁNH GIÁ (INVESTIGATION TOPICS)**. Chúng **KHÔNG PHẢI LÀ QUYẾT ĐỊNH ĐÃ ĐƯỢC PHÊ DUYỆT TRƯỚC**. Solution Architect có trách nhiệm nghiên cứu, so sánh ưu/nhược điểm và đề xuất phương án kỹ thuật tối ưu trong các tài liệu thiết kế của Stage 2 (`TASK-03`).

### 5.1. Ranh Giới Phân Hệ & Thực Thi Phụ Thuộc (Modular Monolith Boundaries & Dependencies)
- **Nội dung điều tra**: Xác định cấu trúc thư viện giải pháp .NET 9 (`SmartRestaurant.Domain`, `Application`, `Infrastructure`, `Presentation`).
- **Câu hỏi kỹ thuật**: Làm thế nào để thực thi sự độc lập giữa 14 phân hệ? Truy vấn chéo (cross-join) giữa các module bị cấm bằng cơ chế nào (Project references hay kiến trúc lỏng)?

### 5.2. Quyền Sở Hữu Dữ Liệu & Ranh Giới Giao Dịch (Data Ownership & Transaction Boundaries)
- **Nội dung điều tra**: Mỗi module sở hữu tập thực thể dữ liệu nào?
- **Câu hỏi kỹ thuật**: Khi đơn hàng phát sinh sự kiện thanh toán, việc cập nhật trạng thái bàn, hóa đơn và tồn kho được thực hiện trong 1 DbContext đơn nguyên hay qua Domain Events nội bộ tiến trình (In-Process MediatR)?

### 5.3. Chiến Lược Kiểm Soát Đồng Thời & Chống Trùng Lặp (Concurrency Strategy & Idempotency)
- **Nội dung điều tra**: Giải quyết race condition khi 2 phục vụ cùng gọi món hoặc thu ngân thanh toán cùng lúc.
- **Phương án cần so sánh**: Khóa lạc quan EF Core `RowVersion` / Concurrency Token vs Mô hình chỉ nối thêm (Append-Only Delta Events) vs Khóa phân tán.
- **Tính lũy biến (Idempotency)**: Cơ chế chống trừ kho 2 lần hoặc sinh 2 thẻ KDS khi mạng nội bộ bị ngắt rồi gửi lại gói tin.

### 5.4. Lưới Giao Tiếp Thời Gian Thực (Real-Time Communication Mesh)
- **Nội dung điều tra**: Thiết kế các Hubs thời gian thực (SignalR Core) kết nối giữa Web POS, KDS và Floor Plan.
- **Phương án cần đánh giá**: Phân tách Hub (`TableHub`, `KitchenHub`, `BillingHub`) vs Sử dụng 1 Hub tổng hợp; chiến lược reconnect và bù đắp dữ liệu khi mất kết nối mạng cục bộ.

### 5.5. Tích Hợp Thanh Toán Bất Đồng Bộ & Sổ Treo Đối Soát (Payment Integration & Suspense Ledger)
- **Nội dung điều tra**: Thiết kế webhook controller nhận thông báo thanh toán VietQR Napas 24/7.
- **Câu hỏi kỹ thuật**: Cơ chế xác thực chữ ký số HMAC/SHA256 của ngân hàng; thuật toán điều chuyển giao dịch quá hạn vào `SuspenseLedger` và cơ chế phát chuông báo thu ngân.

### 5.6. Tính Nhất Quán Tồn Kho & Điểm Kích Hoạt Khấu Trừ (Inventory Consistency & Deduction Trigger)
- **Nội dung điều tra**: Triển khai khóa tồn kho hai pha (`Available = Physical - Reserved`).
- **Câu hỏi kỹ thuật quan trọng**: Điểm kích hoạt (Trigger Point) chính xác để chuyển từ `ReservedStock` sang `ConsumedStock` là gì? (Khi KDS bấm ReadyToServe, hay khi phục vụ bấm Served, hay khi quyết toán hóa đơn? Cần đánh giá ưu/nhược điểm vận hành).

### 5.7. Vòng Đời Module & Cơ Chế Thoát Êm Dịu (Module Lifecycle & Graceful Drain — OD-17)
- **Nội dung điều tra**: Cơ chế kích hoạt/hạ cấp module khi chạy mà không phải khởi động lại máy chủ (Zero downtime).
- **Phương án cần đánh giá**: ASP.NET Core Middleware kiểm tra cờ tính năng trong bộ nhớ đệm `IMemoryCache` vs Custom Action Filter `[RequireModule]` vs Feature Flags framework.

### 5.8. Khả Năng Mở Rộng Đa Chi Nhánh Trong Lược Đồ Dữ Liệu (Multi-Branch Extensibility)
- **Nội dung điều tra**: Thiết kế lược đồ Entity Framework Core hỗ trợ `OrganizationId` và `BranchId`.
- **Phương án cần đánh giá**: Sử dụng EF Core Global Query Filters vs Composite Primary Keys (`BranchId, Id`) để bảo đảm hiệu năng cao nhất trên điểm bán đơn lẻ trong MVP.

### 5.9. Kiến Trúc Bảo Mật & Xác Thực Mã PIN (Security & Authentication Architecture)
- **Nội dung điều tra**: Cơ chế xác thực người dùng kết hợp giữa JWT Token (cho phiên đăng nhập) và xác thực mã PIN 4 số nhanh (cho tablet dùng chung tại bàn).
- **Câu hỏi kỹ thuật**: Cơ chế mã hóa và đối soát mã PIN Quản lý trong bộ nhớ để bảo đảm tốc độ duyệt thao tác dưới 100ms mà không lộ bí mật an toàn.

---

## 6. Chỉ Thị Thực Thi Kỹ Thuật Tiếp Theo (TASK-03 Deliverables)

Solution Architect tiếp nhận gói bàn giao này để nghiên cứu, chuẩn bị kế hoạch kỹ thuật và sẵn sàng thực thi nhiệm vụ **`TASK-03: Stage 1 Sign-Off & Stage 2 Technical Architecture Design`** ngay khi nhận được chỉ đạo / phê duyệt khởi động chính thức từ Chủ Dự Án (Project Owner):
1. `docs/02-architecture/system-architecture.md`: Bản thiết kế kiến trúc Modular Monolith.
2. `docs/02-architecture/data-models-and-erd.md`: Lược đồ CSDL quan hệ Microsoft SQL Server & EF Core ERD.
3. `docs/02-architecture/realtime-signalr-mesh.md`: Hợp đồng kết nối và thông điệp thời gian thực SignalR Hubs.
