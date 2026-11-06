# Hồ Sơ Cơ Sở Quy Tắc Nghiệp Vụ & Kiểm Toán Tính Nhất Quán (Business Rules Baseline)
## DỰ ÁN SMARTRESTAURANT — GIAI ĐOẠN 1 (STAGE 1 BASELINE)

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Mã Tài Liệu**: `BUSINESS-RULES-BASELINE`
- **Phiên Bản Cơ Sở**: `v1.0.0-FROZEN`
- **Trạng Thái Quản Trị**: `OWNER-CONFIRMED & SEALED`
- **Tài Liệu Căn Cứ**: [`BUSINESS-REQUIREMENTS.md`](./BUSINESS-REQUIREMENTS.md), [`OWNER-DECISION-WORKSHOP.md`](./OWNER-DECISION-WORKSHOP.md), [`TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md`](./TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md)

---

## 1. Danh Mục 18 Quy Tắc Nghiệp Vụ Hợp Nhất (Master Business Rules Register)

Bảng hợp nhất 18 Quy tắc Nghiệp vụ Bắt buộc cấu thành rào giậu vận hành của hệ thống:

| Mã Quy Tắc | Miền Nghiệp Vụ | Tên Quy Tắc Nghiệp Vụ | Phát Biểu Quy Tắc Chi Tiết & Ràng Buộc | Cấp Độ Thực Thi | Quyết Định / Bất Biến Căn Cứ |
| :---: | :--- | :--- | :--- | :--- | :---: |
| **BR-RULE-01** | Bàn Ăn | Khóa Bàn Chống Gán Đè | Bàn không thể được gán cho lượt khách mới nếu đang ở trạng thái `Occupied` (Có khách), `BillingRequested` (Đang thanh toán), hoặc `CleaningNeeded` (Cần dọn dẹp). | Chặn Cứng Hệ Thống (Hard System Block) | `PDR-002` |
| **BR-RULE-02** | Điều Chỉnh Món | Ủy Quyền PIN Quản Lý Khi Hủy Món Nấu | Khi món ăn đã chuyển sang trạng thái `Cooking` (Đang nấu) hoặc `ReadyToServe` (Đã xong, chờ bưng), phục vụ KHÔNG THỂ xóa hoặc hủy món nếu thiếu mã PIN Quản lý ủy quyền và lý do hủy bắt buộc. | Phê Duyệt Của Quản Lý (Manager Authorization) | `PDR-015` |
| **BR-RULE-03** | Kế Toán Tổn Thất | Tự Động Quy Trách Nhiệm Hao Phí Hủy Món | Bất kỳ món ăn nào bị hủy sau khi đã vào trạng thái `Cooking` BẮT BUỘC phải được ghi nhận vào Sổ Ghi Nhận Hao Phí Nhà Bếp (`WasteLog`) để đối soát thất thoát. | Tự Động Tuân Thủ (Automatic Compliance) | `PDR-004`, `PDR-015` |
| **BR-RULE-04** | Bàn Ăn | Chặn Chuyển Bàn Vào Bàn Đang Có Khách | Yêu cầu chuyển bàn bị từ chối nếu bàn đích hiện đang có một lượt khách khác đang hoạt động (`Occupied`). Khách chỉ có thể chuyển sang bàn trống (`Available`). | Chặn Cứng Hệ Thống (Hard System Block) | `PDR-002` |
| **BR-RULE-05** | Tính Tiền | Chính Sách Không Chồng Khuyến Mãi | Giảm giá khuyến mãi theo từng món không được cộng dồn với voucher giảm giá theo phần trăm toàn đơn, trừ khi được bật rõ trong cấu hình cài đặt của nhà hàng. | Chính Sách Cấu Hình (Configurable Policy) | `PDR-005` |
| **BR-RULE-06** | Tính Tiền | Bàn Đang Thanh Toán Vẫn Cho Phép Sửa Đổi | Khi bàn ở trạng thái `BillingRequested`, hóa đơn chưa thanh toán vẫn hoàn toàn được phép chỉnh sửa, thêm món mới (nối tiếp vào bill mở hoặc tạo sub-bill mới). Nhiều sub-bill có thể thanh toán độc lập. Bàn CHỈ GIẢI PHÓNG khi 100% hóa đơn đã thanh toán. Hóa đơn đã thanh toán cấm sửa trực tiếp. | Linh Hoạt Vận Hành & Bảo Vệ Kiểm Toán | `OD-13 Option 2` |
| **BR-RULE-07** | Ca Thu Ngân | Chặn Chốt Ca Khi Còn Hóa Đơn Mở | Thu ngân không thể đóng ca làm việc nếu vẫn còn hóa đơn mở chưa thanh toán do thiết bị của thu ngân đó khởi tạo. Thu ngân thực hiện kiểm tiền mù cuối ca. | Chặn Cứng Hệ Thống (Hard System Block) | `PDR-007` |
| **BR-RULE-08** | Kế Toán Ca | Ngày Kinh Doanh Theo Ca Vận Hành | Toàn bộ đơn hàng và hóa đơn tạo trong một ca làm việc đều thuộc về `BusinessDate` của ca đó (mặc định mốc cắt 06:00 sáng). Thay đổi giờ cắt không tính toán hồi tố dữ liệu cũ. | Bất Biến Kế Toán (Accounting Invariant) | `D1 / PDR-007` |
| **BR-RULE-09** | Tồn Kho | Khóa Tồn Kho Hai Pha Cho Món Đếm Được | Bắn đơn xuống bếp sẽ khóa tạm giữ tồn kho hàng đếm được (`ReservedStock`); bếp nấu xong hoặc phục vụ hoàn thành sẽ chuyển sang tiêu hao thực tế (`ConsumedStock`). Công thức: $\text{Khả dụng} = \text{Thực tế} - \text{Tạm giữ}$. | Toàn Vẹn Tồn Kho (Inventory Integrity) | `D2/D5 / PDR-004` |
| **BR-RULE-10** | Thanh Toán | Xử Lý Giao Dịch VietQR Đến Muộn | Khi chuyển phương thức sang Tiền mặt, mã QR bị vô hiệu hóa; webhook ngân hàng đến muộn được điều hướng vào `Sổ Treo Giao Dịch Đối Soát` (`SuspenseLedger`) kèm cảnh báo quản lý, không ghi nhận trùng. | An Toàn Thanh Toán (Payment Safety) | `D3 / PDR-008` |
| **BR-RULE-11** | Tính Tiền | Tách Bill Không Đóng Băng Bàn | Bàn đang thực hiện tách hóa đơn KHÔNG BỊ ĐÓNG BĂNG. Nhân viên phục vụ tiếp tục gọi thêm món vào các hóa đơn con đang mở. Hỗ trợ tách theo món và chia đều $1/N$ trong MVP. | Bất Biến Vòng Đời (Lifecycle Invariant) | `D4 / PDR-005 / OQ-01` |
| **BR-RULE-12** | Bàn Ăn | Hủy Gộp Bàn Phân Bổ Lại Món | Các bàn đã gộp có thể hủy gộp bất kỳ lúc nào; nhân viên phân bổ lại các món ăn tương ứng về bàn tách ra mà không phải bắn lại phiếu chế biến xuống nhà bếp. | Bảo Vệ Vận Hành (Operation Guard) | `PDR-010 / REV-05` |
| **BR-RULE-13** | Tính Tiền | Lưu Vết Phụ Thu Phòng & Khu Vực | Phòng VIP và khu vực hỗ trợ phụ thu theo tỷ lệ % hoặc số tiền cố định được chốt số liệu tại thời điểm tạo hóa đơn; thao tác chuyển bàn không tính toán lại hồi tố các món đã dùng trước đó. | Minh Bạch Hóa Đơn (Billing Transparency) | `PDR-011 / REV-06` |
| **BR-RULE-14** | Điều Phối Bếp | Phân Tuyến Trực Tiếp Tại Quầy FOH | Đồ uống đóng chai sẵn và món lấy ngay được phục vụ trực tiếp tại quầy FOH, bỏ qua hàng đợi pha chế của Bar KDS nhưng vẫn bảo đảm liên kết với kiểm soát hóa đơn và kho. | Tối Ưu Tốc Độ Pha Chế | `PDR-012 / REV-08` |
| **BR-RULE-15** | Nhà Bếp | Làm Lại Món Do Lỗi Bếp (Remake Dish) | Món ăn nấu lại do lỗi nhà bếp không tính thêm tiền cho khách, sinh thẻ KDS làm lại món ưu tiên cao và tự động ghi nhận tổn thất nguyên liệu vào `WasteLog`. | Chất Lượng & Kế Toán | `PDR-015 / REV-11` |
| **BR-RULE-16** | Đơn Hàng | Gọi Món Đồng Thời Không Khóa Bàn | Nhiều nhân viên phục vụ có thể cùng lúc chọn món vào một bàn mà không bị khóa chặn bàn; hệ thống bảo đảm cơ chế chỉ nối thêm (append-only) an toàn và xử lý xung đột cập nhật lạc quan. | Kiểm Soát Đồng Thời (Concurrency Rule) | `PDR-014 / REV-10` |
| **BR-RULE-17** | Tính Tiền | Hóa Đơn Đã Đóng Bất Biến & Lệch Két | Hóa đơn đã quyết toán (`IsSettled = true`) là bất biến tuyệt đối trong CSDL MVP. Mọi sai sót tiền mặt được xử lý hoàn tiền thủ công ngoài quầy kèm biên bản giấy ký tay; ghi nhận lệch két vào `ShiftClosingVarianceLog` khi chốt ca. | Bất Biến Tài Chính & Kiểm Toán Két | `OD-14 Option A` |
| **BR-RULE-18** | Phiên Bàn | Phiên Bàn Kéo Dài > 24 Giờ | Bàn ăn mở kéo dài hơn 24 giờ qua một hoặc nhiều mốc cắt ca vẫn giữ nguyên `BusinessDate` ban đầu khi mở theo nguyên tắc `D1`, đồng thời hiển thị dải băng cảnh báo màu cam cho Quản lý kiểm tra vận hành. | Bảo Toàn Phiên & Cảnh Báo Vận Hành | `OQ-07 Option A` |

---

## 2. Kiểm Toán Tính Nhất Quán Giữa Các Quy Tắc Nghiệp Vụ (Consistency Audit)

Thực hiện kiểm tra chéo toàn diện trên 6 miền nghiệp vụ trọng yếu nhằm bảo đảm không có mâu thuẫn nội tại:

### 2.1. Kiểm Toán Miền Đơn Hàng & Bàn Ăn (Order & Table Consistency)
- **Kiểm tra tương thích giữa `BR-RULE-01`, `04`, `06`, `11`, `16`, `18`**:
  * Khi bàn chuyển sang `BillingRequested` (in phiếu tạm tính), bàn **không bị khóa cứng**. Nhân viên vẫn có thể mở giỏ hàng thêm món theo `BR-RULE-06`.
  * Việc thêm món mới trong trạng thái này tuân thủ cơ chế đồng thời `BR-RULE-16` (append-only an toàn).
  * Phiên bàn có thể có nhiều hóa đơn con (`BR-RULE-11`), việc gọi thêm món sẽ mặc định nối vào hóa đơn chưa thanh toán hoặc tạo sub-bill mới theo lựa chọn của nhân viên.
  * Bàn chỉ chuyển sang `CleaningNeeded` và sau đó `Available` khi 100% hóa đơn con đã thanh toán và hoàn tất phục vụ món ăn.
  * Bàn mở quá 24h (`BR-RULE-18`) không bị hệ thống tự động cưỡng bức đóng bàn, bảo toàn tính liên tục của trải nghiệm khách hàng.
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & KHÔNG MÂU THUẪN**.

### 2.2. Kiểm Toán Miền Nhà Bếp & KDS (Kitchen Consistency)
- **Kiểm tra tương thích giữa `BR-RULE-02`, `03`, `14`, `15`**:
  * Món ăn sau khi bắn bếp được động cơ điều phối phân tách trạm (`MOD-08`). Nếu món có cờ `DirectFOHPick: true`, món lập tức bỏ qua hàng đợi pha chế Bar KDS theo `BR-RULE-14`.
  * Nếu món đã chuyển sang `Cooking` mà khách hủy món, nhân viên phải xin mã PIN Quản lý (`BR-RULE-02`), hệ thống lập tức ghi hao phí vào `WasteLog` (`BR-RULE-03`).
  * Nếu món bị hỏng do lỗi đầu bếp (`BR-RULE-15`), nhân viên kích hoạt quy trình làm lại món: Món mới có giá `0đ`, sinh thẻ KDS ưu tiên cao, và tổn thất đĩa món cũ cũng được tự động ghi nhận vào `WasteLog` theo chuẩn mực kế toán chi phí.
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & KHÔNG MÂU THUẪN**.

### 2.3. Kiểm Toán Miền Thu Ngân & Thanh Toán (Billing & Payment Consistency)
- **Kiểm tra tương thích giữa `BR-RULE-05`, `06`, `07`, `10`, `13`, `17`**:
  * Hóa đơn tính tiền tuân thủ quy tắc không cộng dồn voucher toàn đơn với chiết khấu món (`BR-RULE-05`). Phụ phí phòng VIP được chốt snapshot cố định (`BR-RULE-13`).
  * Khi thanh toán VietQR Napas, nếu chuyển tiền mặt khẩn cấp và webhook đến sau (`BR-RULE-10`), số tiền chuyển vào `SuspenseLedger` để quản lý đối soát riêng, không làm thay đổi số dư két tiền mặt của thu ngân.
  * Hóa đơn sau khi quyết toán thành công (`IsSettled = true`) là bất biến tuyệt đối (`BR-RULE-17`). Khi phát sinh trả tiền thối nhầm hoặc khách trả món, tiền chênh lệch két được ghi nhận trong biên bản giấy ngoài quầy và hạch toán vào `ShiftClosingVarianceLog` tại thời điểm kiểm tiền mù chốt ca (`BR-RULE-07`).
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & KHÔNG MÂU THUẪN**.

### 2.4. Kiểm Toán Miền Quản Lý Tồn Kho (Inventory Consistency — D2/D5)
- **Kiểm tra tương thích giữa `BR-RULE-03`, `09`, `15` và Quyết Định D2/D5**:
  * Định nghĩa chuẩn mực: $\text{Tồn khả dụng (Available Stock)} = \text{Tồn thực tế (Physical Stock)} - \text{Tạm giữ (Reserved Stock)}$.
  * Hệ thống phân biệt rõ ràng 4 khái niệm:
    1. *Tạm giữ (Reserved)*: Khóa số lượng thành phẩm khi đơn món được bắn xuống bếp.
    2. *Tiêu hao thực tế (Physical Consumption)*: Nguyên vật liệu đã được lấy ra nấu hoặc xuất cho khách.
    3. *Hao hụt (Waste)*: Hàng hóa bị hỏng hóc, đổ vỡ, nấu lỗi ghi vào `WasteLog`.
    4. *Khấu trừ kho (Inventory Deduction)*: Bút toán trừ số lượng thực tế trong sổ kho CSDL.
  * **Tuân thủ quy tắc quản trị**: Hệ thống **KHÔNG TỰ Ý ÁP ĐẶT THỜI ĐIỂM KHẤU TRỪ TỒN KHO**. Thời điểm kích hoạt trừ kho (bếp bấm ReadyToServe, phục vụ bấm Served, hay khi quyết toán hóa đơn) là **chủ đề kỹ thuật cần nghiên cứu trong Stage 2** (Solution Architect investigation topic).
  * Trong phạm vi MVP, việc theo dõi tồn kho giới hạn ở món đếm được (lon, chai, suất ăn đóng gói) theo `D5`. Định lượng chi tiết nguyên vật liệu thô (BOM gram/ml) hoãn sang v1.1.
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & TUÂN THỦ NGHIÊM NGẶT RANH GIỚI QUẢN TRỊ**.

### 2.5. Kiểm Toán Miền Quản Trị Module & Thoát Êm Dịu (Module Governance & Graceful Drain)
- **Kiểm tra tương thích giữa 10 Core + 4 Optional Modules và Quyết Định `OD-17`**:
  * 10 Phân hệ Cốt Lõi (`MOD-01..05, 07, 08, 10, 13, 14`) không thể bị tắt, bảo đảm hệ thống luôn duy trì khả năng phục vụ tối thiểu.
  * Khi tắt 1 trong 4 Phân hệ Tùy chọn (`MOD-06, 09, 11, 12`), hệ thống thực hiện Graceful Drain (`OD-17: Phương án A`):
    * Chặn tiếp nhận giao dịch mới ngay lập tức.
    * Cho phép hoàn tất các phiên bàn đang mở, giao dịch webhook đang chờ, phiếu đang nấu dở.
    * Toàn bộ dữ liệu lịch sử được bảo toàn 100% trong CSDL ở trạng thái Read-Only; báo cáo tài chính lịch sử vẫn truy cập và hiển thị đầy đủ.
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & KHÔNG MÂU THUẪN**.

### 2.6. Kiểm Toán Miền Đồng Thời & Kiểm Toán (Concurrency & Audit Integrity)
- **Kiểm tra tương thích giữa `BR-RULE-16`, `BR-SEC-003`, `BR-REP-004`**:
  * Việc bồi bàn gọi món đồng thời trên cùng một bàn được xử lý theo mô hình chỉ nối thêm (append-only), chống ghi đè dữ liệu.
  * Mọi hành động hủy món, chiết khấu hoặc đóng ca đều bắt buộc nhập mã PIN Quản lý và lưu vết vĩnh viễn vào `ManagerPinAudit` và `ConfigurationAuditLog`.
  * **Kết Luận**: **HOÀN TOÀN NHẤT QUÁN & BẢO ĐẢM TOÀN VẸN DỮ LIỆU**.
