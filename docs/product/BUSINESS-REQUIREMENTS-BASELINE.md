# Hồ Sơ Cơ Sở Yêu Cầu Nghiệp Vụ (Business Requirements Baseline)
## DỰ ÁN SMARTRESTAURANT — GIAI ĐOẠN 1 (STAGE 1 BASELINE)

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Mã Tài Liệu**: `BUSINESS-REQUIREMENTS-BASELINE`
- **Phiên Bản Cơ Sở**: `v1.0.0-FROZEN`
- **Trạng Thái Quản Trị**: `OWNER-CONFIRMED & SEALED`
- **Tài Liệu Căn Cứ**: [`BUSINESS-REQUIREMENTS.md`](./BUSINESS-REQUIREMENTS.md), [`PRD.md`](./PRD.md), [`OWNER-DECISION-WORKSHOP.md`](./OWNER-DECISION-WORKSHOP.md), [`TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md`](./TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md)

---

## 1. Nguyên Tắc Quản Trị Yêu Cầu Nghiệp Vụ (Requirements Governance)

Mọi yêu cầu nghiệp vụ trong tài liệu này được phân loại trạng thái phê duyệt nghiêm ngặt:
- **`OWNER-CONFIRMED`**: Yêu cầu đã được Chủ Dự Án phê chuẩn chính thức qua các văn bản phê duyệt (Task 02.6, Task 02.8.5, Task 02.8.6).
- **`PROPOSED`**: Đề xuất kỹ thuật hoặc chi tiết hóa của AI/BA, đang chờ Chủ Dự Án duyệt.
- **`OPEN`**: Hạng mục còn đang trong quá trình trao đổi, chưa có quyết định cuối cùng.
- **`DEFERRED`**: Yêu cầu được Chủ Dự Án chính thức quyết định hoãn sang phiên bản sau.

---

## 2. Danh Mục Yêu Cầu Nghiệp Vụ Chi Tiết Theo Miền Chức Năng (Functional Requirements)

### 2.1. Miền 01: Thiết Lập Nhà Hàng, Khu Vực & Sơ Đồ Bàn (Dining Areas & Floor Plan)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-AREA-001** | Phân Cấp Đa Khu Vực | Hỗ trợ quản lý nhiều khu vực ăn uống riêng biệt trong nhà hàng (Tầng 1, Sân vườn, Ban công, Tầng lửng). | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-002** | Phòng Ăn & Phòng VIP | Hỗ trợ phân cấp các phòng độc lập và phòng VIP riêng biệt nằm trong từng khu vực. | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-003** | Canvas Sơ Đồ Bàn 2D | Công cụ kéo thả đồ họa trực quan bố trí bàn ăn, kích thước bàn, hình dạng bàn (tròn, chữ nhật, vuông) và vật cản kiến trúc (cột, tường, quầy). | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-004** | Đặt Bao Trọn Phòng VIP | Cho phép đặt trước hoặc khóa phục vụ bao trọn một phòng VIP, tự động khóa toàn bộ bàn con bên trong không cho khách lẻ ngồi. | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-005** | Mã Màu Trạng Thái Bàn | Hiển thị trạng thái bàn thời gian thực theo mã màu: `Available` (Xanh lá), `Reserved` (Xanh dương), `Occupied` (Vàng), `BillingRequested` (Tím), `CleaningNeeded` (Cam), `OutOfService` (Xám). | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-006** | Phụ Phí Phòng VIP & Khu Vực | Hỗ trợ cấu hình phụ phí theo tỷ lệ % hoặc số tiền cố định; lưu snapshot tại thời điểm mở bàn (`PDR-011`). | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |
| **BR-AREA-007** | Bàn Độc Lập Trong Phòng Dùng Chung | Hỗ trợ các bàn độc lập trong phòng dùng chung ngồi khách riêng biệt; hoặc khóa bao trọn phòng khi có tiệc lớn (`PDR-013`). | BRS §2.1 | `MOD-04` | `OWNER-CONFIRMED` |

### 2.2. Miền 02: Thực Đơn, Tùy Chọn & Ánh Xạ Trạm Bếp (Menu Catalog & Routing)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-MENU-001** | Cấu Trúc Danh Mục Phân Cấp | Danh mục món ăn phân cấp đa tầng (Danh mục cha $\rightarrow$ Danh mục con $\rightarrow$ Món ăn) kèm thứ tự hiển thị và hình ảnh. | BRS §2.2 | `MOD-05` | `OWNER-CONFIRMED` |
| **BR-MENU-002** | Biến Thể Kích Cỡ & Khẩu Phần | Hỗ trợ nhiều kích cỡ món ăn (S, M, L; 0.5kg, 1.0kg) với giá bán và mã SKU riêng biệt. | BRS §2.2 | `MOD-05` | `OWNER-CONFIRMED` |
| **BR-MENU-003** | Nhóm Tùy Chọn Món Bắt Buộc & Tự Do | Nhóm tùy chọn chọn 1 (độ chín bít tết), chọn nhiều (thêm thạch, trân châu); cấu hình số lượng chọn tối thiểu/tối đa. | BRS §2.2 | `MOD-05` | `OWNER-CONFIRMED` |
| **BR-MENU-004** | Ghi Chú Chế Biến Đặc Biệt | Cho phép phục vụ nhập ghi chú món riêng biệt cho bếp ("Ít cay", "Không hành", "Dị ứng hải sản"). | BRS §2.2 | `MOD-05` | `OWNER-CONFIRMED` |
| **BR-MENU-005** | Ánh Xạ Trạm Bếp Chế Biến | Mỗi món ăn được cấu hình ánh xạ tới trạm chế biến mục tiêu (Bếp Nóng, Bếp Nguội, Quầy Bar, Bếp Nướng). | BRS §2.2 | `MOD-08` | `OWNER-CONFIRMED` |
| **BR-MENU-006** | Phân Tuyến Nhanh Quầy FOH | Đồ uống đóng chai sẵn hoặc món lấy ngay được phục vụ trực tiếp tại sảnh, bỏ qua hàng đợi Bar KDS (`PDR-012`). | BRS §2.2 | `MOD-05, 08` | `OWNER-CONFIRMED` |

### 2.3. Miền 03: POS Bán Hàng & Tiếp Nhận Đơn Gọi Món (POS Order Capture)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :--- |
| **BR-POS-001** | Khởi Tạo Phiên Dùng Bữa Tại Bàn | Phục vụ chạm vào bàn trống để mở phiên dùng bữa, nhập số lượng khách người lớn/trẻ em. | BRS §2.3 | `MOD-07` | `OWNER-CONFIRMED` |
| **BR-POS-002** | Gọi Món Cảm Ứng Nhanh | Giao diện cảm ứng tối ưu hóa, danh mục món trực quan, tìm kiếm nhanh theo mã hoặc tên món dưới 100ms. | BRS §2.3 | `MOD-07` | `OWNER-CONFIRMED` |
| **BR-POS-003** | Quản Lý Các Đợt Gọi Món (Order Rounds) | Mỗi lần bấm "Báo bếp" tạo thành một đợt gọi món riêng biệt, lưu vết thời gian và nhân viên gửi. | BRS §2.3 | `MOD-07` | `OWNER-CONFIRMED` |
| **BR-POS-004** | Gọi Món Đồng Thời Không Khóa Bàn | Nhiều phục vụ có thể đồng thời mở cùng một bàn để thêm món mà không bị chặn, lưu vết an toàn chỉ nối thêm (`PDR-014`). | BRS §2.3 | `MOD-07` | `OWNER-CONFIRMED` |
| **BR-POS-005** | Bàn Tạm Tính Vẫn Cho Phép Sửa Món | Bàn đang ở trạng thái `BillingRequested` vẫn cho phép thêm món hoặc hủy món hợp lệ (`PDR-009 BillingRequested Option 2 / D4`). | BRS §2.3 | `MOD-07` | `OWNER-CONFIRMED` |
| **BR-POS-006** | Chuyển Bàn & Gộp Bàn | Cho phép chuyển toàn bộ phiên sang bàn trống khác, hoặc gộp 2 hay nhiều bàn thành 1 phiên chung. | BRS §2.3 | `MOD-04, 07` | `OWNER-CONFIRMED` |
| **BR-POS-007** | Hủy Gộp Bàn Tái Phân Bổ Món | Cho phép hủy gộp bàn bất kỳ lúc nào, phân bổ lại món ăn về bàn tách ra mà không phải bắn lại phiếu bếp (`PDR-010`). | BRS §2.3 | `MOD-04, 07` | `OWNER-CONFIRMED` |

### 2.4. Miền 04: Quản Lý Đặt Bàn Trước (Table Reservation — MOD-06)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-RES-001** | Tiếp Nhận Đặt Bàn Khách Hẹn | Ghi nhận thông tin người đặt (tên, số điện thoại), thời gian đến, số lượng khách và ghi chú đặc biệt. | BRS §2.4 | `MOD-06` | `OWNER-CONFIRMED (OD-15)` |
| **BR-RES-002** | Khóa Bàn Chống Xếp Khách Trùng | Bàn đã gắn với lịch đặt bàn được hiển thị màu xanh dương và đồng hồ đếm ngược; chặn xếp khách vãng lai khi sắp tới giờ hẹn. | BRS §2.4 | `MOD-06` | `OWNER-CONFIRMED` |
| **BR-RES-003** | Quản Lý Tiền Đặt Cọc | Ghi nhận khoản tiền cọc của khách, hỗ trợ khấu trừ tự động vào hóa đơn cuối cùng khi thanh toán. | BRS §2.4 | `MOD-06` | `OWNER-CONFIRMED` |
| **BR-RES-004** | Bật/Tắt Module Khớp Nối Lỏng | Nhà hàng không nhận đặt bàn có thể tắt module chỉ với 1 thao tác; giao diện tự động ẩn tab Đặt bàn (`OD-15`). | BRS §2.4 | `MOD-06` | `OWNER-CONFIRMED` |

### 2.5. Miền 05: Vận Hành Nhà Bếp & Màn Hình KDS (Kitchen & KDS)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-KIT-001** | Hàng Đợi Thẻ Món Bếp KDS | Hiển thị thẻ món theo thứ tự thời gian gọi, nhóm theo bàn ăn hoặc món ăn tại từng trạm bếp. | BRS §2.5 | `MOD-09` | `OWNER-CONFIRMED` |
| **BR-KIT-002** | Đồng Hồ Đếm Ngược Cảnh Báo SLA | Thẻ món chuyển màu Vàng khi quá ngưỡng cảnh báo (mặc định 10 phút), chuyển màu Đỏ nhấp nháy khi quá hạn (mặc định 20 phút). | BRS §2.5 | `MOD-09` | `OWNER-CONFIRMED` |
| **BR-KIT-003** | Tương Tác Cảm Ứng Bếp | Đầu bếp chạm vào thẻ món để chuyển trạng thái: `Đang nấu` $\rightarrow$ `Đã xong (Chờ bưng)` $\rightarrow$ `Đã phục vụ`. | BRS §2.5 | `MOD-09` | `OWNER-CONFIRMED` |
| **BR-KIT-004** | Quy Trình Làm Lại Món (Remake Dish) | Món lỗi bếp nấu lại sinh thẻ KDS ưu tiên cao màu đỏ, không tính tiền khách, tự động ghi sổ hao phí (`PDR-015`). | BRS §2.5 | `MOD-08, 09` | `OWNER-CONFIRMED` |
| **BR-KIT-005** | Tùy Chọn In Nhiệt Dự Phòng | Mặc định ưu tiên KDS điện tử (`AutoPrintOnFire: false`); máy in nhiệt ESC/POS đóng vai trò in lại khi cần (`OQ-02`). | BRS §2.5 | `MOD-08, 09` | `OWNER-CONFIRMED` |
| **BR-KIT-006** | Thông Báo Tạm Hết Món (86) | Bếp trưởng có thể bấm báo tạm hết món ngay trên KDS, lập tức đồng bộ khóa món trên toàn bộ tablet POS của bồi bàn. | BRS §2.5 | `MOD-09, 14` | `OWNER-CONFIRMED` |

### 2.6. Miền 06: Thu Ngân, Tính Tiền & Hóa Đơn (Cashier & Billing)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-BIL-001** | In Phiếu Tạm Tính (Pre-bill) | In phiếu kiểm tra món trước thanh toán, hiển thị chi tiết món ăn, phụ thu, chiết khấu, VAT và lời chào. | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |
| **BR-BIL-002** | Tách Hóa Đơn Theo Món Chọn Lọc | Cho phép chọn các món ăn cụ thể để tách ra thành hóa đơn con độc lập mà không đóng băng bàn (`PDR-005/009`). | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |
| **BR-BIL-003** | Tách Hóa Đơn Chia Đều ($1/N$) | Cho phép chia đều tổng tiền theo $N$ người; phần tiền lẻ thập phân dồn vào hóa đơn cuối cùng (`PDR-005`). | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |
| **BR-BIL-004** | Thanh Toán Đa Phương Thức | Hỗ trợ kết hợp nhiều hình thức thanh toán trên cùng một hóa đơn (ví dụ: Một phần tiền mặt + Một phần chuyển khoản). | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |
| **BR-BIL-005** | Hóa Đơn Đã Đóng Bất Biến Tuyệt Đối | Hóa đơn đã thanh toán (`IsSettled = true`) không được sửa trực tiếp; sai lệch tiền mặt ghi nhận tại chốt ca (`OD-14`). | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |
| **BR-BIL-006** | Kiểm Tiền Mù Cuối Ca (Blind Shift Close)| Thu ngân khai báo số tiền mặt thực đếm trong két mà không nhìn thấy số hệ thống; ghi chênh lệch vào log (`BR-RULE-07`). | BRS §2.6 | `MOD-10` | `OWNER-CONFIRMED` |

### 2.7. Miền 07: Cổng Thanh Toán Tự Động & Đối Soát VietQR (VietQR Gateway)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-PAY-001** | Sinh Mã VietQR Động Tự Động | Tự động tạo mã QR chuẩn Napas 24/7 chứa chính xác số tiền cần trả và mã tham chiếu hóa đơn duy nhất. | BRS §2.7 | `MOD-11` | `OWNER-CONFIRMED` |
| **BR-PAY-002** | Nhận Webhook Xác Nhận Thanh Toán | Đón bắt webhook bất đồng bộ từ ngân hàng, tự động xác nhận hóa đơn thành công và in bill trong vòng dưới 2 giây. | BRS §2.7 | `MOD-11` | `OWNER-CONFIRMED` |
| **BR-PAY-003** | Dự Phòng Chuyển Tiền Mặt | Khi khách chờ lâu, thu ngân chuyển phương thức sang Tiền mặt; mã QR đang mở lập tức bị vô hiệu hóa. | BRS §2.7 | `MOD-11` | `OWNER-CONFIRMED` |
| **BR-PAY-004** | Sổ Treo Đối Soát Cho Webhook Đến Muộn | Giao dịch webhook đến muộn sau khi đã thu tiền mặt tự động đi vào `SuspenseLedger` kèm cảnh báo quản lý (`PDR-008`). | BRS §2.7 | `MOD-11` | `OWNER-CONFIRMED` |

### 2.8. Miền 08: Quản Lý Tồn Kho Món Đếm Được & Hao Phí (Inventory & Waste)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-INV-001** | Khóa Tồn Kho Hai Pha (Two-Phase Lock)| Bắn lệnh báo bếp tạm giữ kho (`ReservedStock`), nấu xong/phục vụ thành công trừ thực tế (`ConsumedStock`) (`PDR-004`). | BRS §2.8 | `MOD-12` | `OWNER-CONFIRMED` |
| **BR-INV-002** | Giới Hạn Món Đếm Được Trong MVP | MVP chỉ áp dụng quản lý tồn kho cho hàng đóng gói đếm được (lon, chai, suất đóng gói); hoãn BOM gram/ml sang v1.1. | BRS §2.8 | `MOD-12` | `OWNER-CONFIRMED (D5)` |
| **BR-INV-003** | Ghi Sổ Hao Phí Nhà Bếp (WasteLog) | Mọi trường hợp hủy món sau khi đã bắt đầu nấu bắt buộc phải ghi nhận vào `WasteLog` kèm mã lý do tổn thất. | BRS §2.8 | `MOD-12` | `OWNER-CONFIRMED` |
| **BR-INV-004** | Tự Động Khóa Bán Khi Hết Kho | Tự động chuyển trạng thái món sang "Tạm hết món (86)" khi tồn kho khả dụng về bằng 0. | BRS §2.8 | `MOD-12` | `OWNER-CONFIRMED` |

### 2.9. Miền 09: Báo Cáo Doanh Thu & Kiểm Toán (Reporting & Analytics)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-REP-001** | Báo Cáo Doanh Thu Ngày & Giờ Cắt | Tổng hợp doanh thu gộp theo `BusinessDate` (mặc định 06:00 sáng), phân tách doanh thu theo phương thức thanh toán. | BRS §2.9 | `MOD-13` | `OWNER-CONFIRMED` |
| **BR-REP-002** | Báo Cáo Chốt Ca Thu Ngân (Z-Report) | Kết xuất bảng tổng kết ca làm việc của thu ngân, doanh thu thực tế, tiền mặt khai báo và tiền chênh lệch. | BRS §2.9 | `MOD-13` | `OWNER-CONFIRMED` |
| **BR-REP-003** | Phân Tích Mặt Hàng Bán Chạy | Thống kê số lượng bán, doanh thu và tỷ trọng lợi nhuận theo từng nhóm thực đơn và món ăn. | BRS §2.9 | `MOD-13` | `OWNER-CONFIRMED` |
| **BR-REP-004** | Nhật Ký Lưu Vết Kiểm Toán (Audit Trail) | Lưu vết toàn bộ các thao tác nhạy cảm (nhập PIN hủy món, chiết khấu hóa đơn, chỉnh sửa cấu hình). | BRS §2.9 | `MOD-02, 13` | `OWNER-CONFIRMED` |

### 2.10. Miền 10: Tiêu Chuẩn Bản Địa Hóa Tiếng Việt (Vietnamese-First Localization)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-LOC-001** | 100% Giao Diện Tiếng Việt | Toàn bộ giao diện người dùng hiển thị tiếng Việt có dấu chuẩn xác theo văn phong F&B Việt Nam (`PDR-006`). | BRS §2.10 | Toàn bộ | `OWNER-CONFIRMED` |
| **BR-LOC-002** | Định Dạng Địa Phương Chuẩn | Tiền tệ VNĐ (`#,,### ₫`), ngày tháng `dd/MM/yyyy`, giờ 24h `HH:mm:ss`, múi giờ `Asia/Ho_Chi_Minh` (UTC+7). | BRS §2.10 | Toàn bộ | `OWNER-CONFIRMED` |
| **BR-LOC-003** | In Nhiệt Tiếng Việt Có Dấu ESC/POS | Hỗ trợ in tiếng Việt có dấu qua UTF-8/CodePage 1258, kèm dự phòng tự động chuyển thành không dấu khi cần. | BRS §2.10 | `MOD-08, 10` | `OWNER-CONFIRMED` |
| **BR-LOC-004** | Sẵn Sàng Hóa Đơn Điện Tử (TT78/NĐ123)| Lưu trữ thông tin thuế doanh nghiệp của khách hàng sẵn sàng cho kết nối API sau MVP. | BRS §2.10 | `MOD-10` | `OWNER-CONFIRMED` |

### 2.11. Miền 11: Định Danh, Phân Quyền & Bảo Mật (Identity, RBAC & Security)

| Mã Yêu Cầu | Tên Yêu Cầu Nghiệp Vụ | Mô Tả Nghiệp Vụ Chi Tiết | Nguồn Tài Liệu | Module ID | Trạng Thái Phê Duyệt |
| :---: | :--- | :--- | :---: | :---: | :---: |
| **BR-SEC-001** | Phân Quyền Vai Trò Nghiệp Vụ (RBAC) | Hỗ trợ 6 vai trò tiêu chuẩn: Admin, Quản lý, Thu ngân, Bồi bàn, Đầu bếp, Thủ kho với ma trận quyền hạn rõ ràng. | BRS §1 | `MOD-02` | `OWNER-CONFIRMED` |
| **BR-SEC-002** | Đăng Nhập Nhanh Bằng PIN 4 Số | Cho phép bồi bàn và thu ngân chuyển đổi tài khoản tức thì trên màn hình cảm ứng bằng mã PIN cá nhân bảo mật. | BRS §1 | `MOD-02` | `OWNER-CONFIRMED` |
| **BR-SEC-003** | Phê Duyệt Thao Tác Nhạy Cảm Bằng PIN | Hủy món đã nấu, áp dụng chiết khấu lớn, hoặc hủy hóa đơn bắt buộc phải nhập mã PIN của Quản lý nhà hàng. | BRS §1 | `MOD-02` | `OWNER-CONFIRMED` |
| **BR-SEC-004** | Phân Vùng Dữ Liệu Đa Chi Nhánh | Cấu trúc dữ liệu sẵn sàng phân vùng đa chi nhánh từ ngày đầu; không bắt buộc mọi entity phải chứa cả hai trường `OrganizationId` và `BranchId`; việc phân vùng dữ liệu và ownership theo entity là nội dung đánh giá ở Stage 2 (`OD-01`, `TECH-06`). | BRS §1 | `MOD-01` | `OWNER-CONFIRMED` |
