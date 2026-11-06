# Nhật Ký Các Câu Hỏi Nghiệp Vụ Chờ Quyết Định — SmartRestaurant
## (Open Business Questions & Decision Log)

- **Phiên Bản Tài Liệu**: 1.0.0
- **Trạng Thái Tài Liệu**: `OPEN` (Đang mở chờ xem xét)
- **Chủ Quản Tài Liệu**: Trưởng Ban Kiến Trúc Sản Phẩm & Chuyên Viên Phân Tích Nghiệp Vụ (Senior Product Architect & Business Analyst)

---

## 1. Tóm Tắt Tổng Quan & Phân Loại Mức Độ Ảnh Hưởng (Executive Summary & Impact Classification)

Để duy trì tốc độ phát triển cao đồng thời bảo vệ tính toàn vẹn kiến trúc, các câu hỏi nghiệp vụ còn mở được phân loại thành 2 nhóm rõ ràng:
- **NHÓM 1 — NGHẼN TIẾN ĐỘ THIẾT KẾ MVP (TIER 1 — MVP DESIGN BLOCKERS)**: Các quyết định logic nghiệp vụ tác động trực tiếp đến cấu trúc thực thể cơ sở dữ liệu cốt lõi, thuật toán tính tiền hoặc máy trạng thái. Bắt buộc phải thống nhất trước khi chốt mô hình dữ liệu ở Stage 3.
- **NHÓM 2 — CÂU HỎI KHÔNG LÀM NGHẼN TIẾN ĐỘ / HẬU MVP (TIER 2 — NON-BLOCKING / POST-MVP INQUIRIES)**: Các quyết định ảnh hưởng đến việc mở rộng trong tương lai, xử lý tình huống biên nâng cao hoặc các năng lực thuộc phiên bản v1.1+, không cản trở việc bàn giao MVP ban đầu.

---

## 2. Nhóm 1: Các Vấn Đề Cần Quyết Định Cho MVP (Tier 1: MVP Design Blockers)

### OQ-01: Tách Hóa Đơn Theo Tỷ Lệ Phần Trăm Lẻ (Uneven Percentage-Based Bill Splitting)
- **Bối cảnh**: Khách đi ăn theo nhóm thường có nhu cầu chia tiền hóa đơn. SmartRestaurant MVP hiện hỗ trợ:
  1. *Tách theo món được chọn* (Khách A trả cho món 1 & 2; Khách B trả cho món 3).
  2. *Chia đều hóa đơn* (Lấy tổng số tiền chia đều cho $N$ hóa đơn con bằng nhau, ví dụ: 50/50, 25/25/25/25).
- **Câu hỏi**: SmartRestaurant có cần hỗ trợ tách hóa đơn theo tỷ lệ phần trăm tùy biến bất kỳ (ví dụ: Khách A trả 60%, Khách B trả 40%) hoặc chia theo số tiền ấn định tùy ý (ví dụ: Khách A trả 500.000 ₫, Khách B trả số tiền 732.000 ₫ còn lại) hay không?
- **Tại sao quan trọng**: Việc chia theo số tiền lẻ tùy ý đòi hỏi quy tắc làm tròn số lẻ phức tạp và thuật toán tính lại thuế GTGT trên từng phần giá trị món ăn.
- **Khuyến nghị mặc định**: **Hoãn việc chia theo phần trăm tùy ý sang v1.1**. Trong MVP chỉ hỗ trợ *Tách theo món* và *Chia đều ($1/N$)*. Nếu khách muốn trả các khoản tiền khác nhau, thu ngân có thể dùng tính năng thanh toán kết hợp nhiều hình thức (Multi-Tender) trên một hóa đơn (ví dụ: thu trước 500.000 ₫ Tiền mặt, sau đó thu 732.000 ₫ Chuyển khoản VietQR trên cùng hóa đơn).
- **Trạng thái**: `OPEN QUESTION` (Chờ Chủ Dự Án xác nhận)

---

### OQ-02: Tự Động In Nhiệt Song Song Khi Bắn Đơn Xuống KDS (Simultaneous Physical Printing)
- **Bối cảnh**: Khi nhân viên phục vụ bấm "Báo bếp" từ máy tính bảng, các món ăn được phát sóng qua SignalR tới các màn hình KDS nhà bếp.
- **Câu hỏi**: Hệ thống có nên tự động kích hoạt máy in nhiệt ESC/POS in phiếu giấy trong bếp song song với mỗi lần thẻ KDS xuất hiện, hay việc in giấy chỉ là tùy chọn in lại thủ công dự phòng?
- **Tại sao quan trọng**: Việc in kép gây lãng phí giấy in nhiệt và có thể gây bối rối cho đầu bếp (phải nhìn cả giấy lẫn màn hình). Tuy nhiên, tại một số trạm bếp nhiều dầu mỡ hoặc nhiệt độ cao (ví dụ: trạm nướng lẩu), một số đầu bếp truyền thống vẫn muốn có phiếu giấy ghim trên thanh ray.
- **Khuyến nghị mặc định**: **Thiết lập việc in phiếu bếp thành một tham số cấu hình theo từng trạm** (`AutoPrintOnFire: true/false`). Mặc định là `false` (ưu tiên màn hình KDS số) kèm nút bấm 1 chạm "In lại phiếu bếp" trên màn hình KDS làm dự phòng phần cứng khẩn cấp.
- **Trạng thái**: `OPEN QUESTION` (Chờ Chủ Dự Án xác nhận)

---

### OQ-03: Quy Trách Nhiệm Hao Phí Khi Hủy Món Sau Nấu & Làm Lại Món (Waste Attribution)
- **Bối cảnh**: Khi khách hủy món sau khi món đã chuyển sang trạng thái `Cooking` (Đang nấu) hoặc món ăn phải nấu lại do lỗi nhà bếp.
- **Nghị quyết xử lý**: **ĐÃ ĐƯỢC GIẢI QUYẾT trong Task 02.6 (PDR-004 & PDR-015)**. Món hủy sau nấu và món làm lại được chuyển từ số lượng tạm giữ sang `WasteStock` trong bảng `WasteLog` kèm mã lý do bắt buộc và sự phê duyệt của quản lý. Hóa đơn của khách được bảo vệ an toàn.
- **Trạng thái**: `CONFIRMED`

---

### OQ-07: Xử Lý Phiên Bàn Kéo Dài Bất Thường (> 24 Giờ Vượt Mốc Cắt Ca)
- **Bối cảnh**: Trong các tình huống hiếm gặp (ví dụ: chi nhánh sân bay mở 24/7, mô hình nhà hàng kết hợp karaoke xuyên đêm, hoặc nhân viên quên đóng một bàn khách đã về), một phiên bàn có thể mở kéo dài hơn 24 giờ, vượt qua 2 hoặc nhiều mốc chuyển ngày kinh doanh (`BusinessDate` Cutoff).
- **Câu hỏi**: Khi một đơn hàng mở xuyên qua từ 2 mốc cắt ca trở lên, hệ thống có nên tự động cảnh báo quản lý để cưỡng bức đóng bàn/tách bill, hay âm thầm giữ nguyên `BusinessDate` ban đầu cho đến khi thu ngân chốt bill thủ công?
- **Khuyến nghị mặc định**: Hệ thống giữ nguyên `BusinessDate` gốc để đảm bảo tính nhất quán hạch toán, đồng thời gắn một dải băng cảnh báo màu vàng nổi bật "Phiên bàn kéo dài > 24h" trên bảng điều khiển của Quản lý để nhắc nhở kiểm tra vận hành.
- **Trạng thái**: `OPEN QUESTION` (Đang đánh giá vận hành)

---

### OQ-08: Lựa Chọn Giải Pháp Kiểm Soát Đồng Thời Khi Nhiều Phục Vụ Cùng Gọi Món (Concurrency Strategy)
- **Bối cảnh**: Nhiều nhân viên phục vụ có thể cùng lúc chọn món và bắn đơn vào một bàn đang có khách (PDR-014).
- **Câu hỏi**: Kiến trúc sư giải pháp nên lựa chọn cơ chế kỹ thuật cụ thể nào trong Stage 2 để bảo đảm việc nối thêm đơn không bị khóa chặn, bảo đảm tính bất biến khi thử lại (idempotency) và phát hiện xung đột?
  - Phương án A: Kiến trúc hướng sự kiện (Event Sourcing) / Lệnh chỉ nối thêm (Delta Command Append).
  - Phương án B: Stored Procedure nguyên tử trong SQL / Giao dịch delta phiên bàn kết hợp kiểm tra đồng thời bằng `RowVersion`.
  - Phương án C: Hàng đợi lệnh trong bộ nhớ phân tán (Hàng đợi Redis / Channel Background Worker).
- **Trạng thái**: `OPEN QUESTION` (Ủy quyền cho Kiến trúc sư Giải pháp đánh giá trong Stage 2)

---

### OQ-09: Chính Sách Xử Lý Khiếu Nại, Hủy & Hoàn Tiền Hóa Đơn Đã Thanh Toán (Paid Bill Void/Refund Policy)
- **Bối cảnh**: Căn cứ Quyết định của Chủ Dự án OD-13 và PDR-009, hóa đơn đã thanh toán (`PaidInvoices`) là bất biến và nhân viên phục vụ hoặc thu ngân không thể tự ý chỉnh sửa trực tiếp.
- **Câu hỏi**: Khi phát sinh sai sót vận hành sau khi đã thu tiền (ví dụ: phát hiện tính nhầm tiền, khách thanh toán trùng 2 lần, khách khiếu nại sau khi thanh toán, hoặc trả lại món sau thanh toán), quy trình hủy, điều chỉnh hoặc hoàn tiền được quy định như thế nào?
  - Phương án A: Giữ tính bất biến cứng không sửa đổi trong CSDL (Khuyến nghị cho MVP). Hóa đơn đã quyết toán được khóa vĩnh viễn trên POS. Mọi khoản hoàn tiền hoặc điều chỉnh được xử lý thủ tục hành chính giấy tờ ngoài phần mềm và đối soát như một khoản chênh lệch tiền mặt khi chốt ca kiểm tiền mù.
  - Phương án B: Xây dựng quy trình Hủy Hóa Đơn & Bút Toán Âm (Credit Note) trong ứng dụng. Yêu cầu mã PIN Quản lý để hủy hóa đơn, sinh bút toán âm triệt tiêu doanh thu và khôi phục/phân bổ lại các món trong phiên bàn.
- **Khuyến nghị mặc định**: **Phương án A cho MVP**. Duy trì tính bất biến nghiêm ngặt của CSDL cho hóa đơn đã thanh toán; hoãn máy trạng thái hủy/hoàn tiền in-app phức tạp cho đến khi có quyết định chính thức của Chủ Dự Án.
- **Trạng thái**: `OPEN QUESTION` (OD-14 — Đang chờ Chủ Dự Án quyết định)

---

## 3. Nhóm 2: Các Câu Hỏi Không Làm Nghẽn Tiến Độ / Hậu MVP (Tier 2: Non-Blocking / Post-MVP)

### OQ-04: Quản Lý Chuỗi Cung Ứng Kho Tổng Tập Trung Đa Chi Nhánh (Centralized Warehouse Supply Chain)
- **Bối cảnh**: Trong các chuỗi nhà hàng đa chi nhánh, nguyên vật liệu thường được mua số lượng lớn về kho tổng hoặc bếp trung tâm, sau đó điều chuyển thành các lô nhỏ về từng chi nhánh.
- **Câu hỏi**: Khi mở rộng chuỗi đa chi nhánh trong phiên bản v2.0, hệ thống có hỗ trợ quy trình điều chuyển kho liên chi nhánh hay mỗi chi nhánh tự quản lý đơn đặt hàng mua sắm độc lập?
- **Tại sao quan trọng**: Điều chuyển liên chi nhánh đòi hỏi phiếu xuất kho vận chuyển, trạng thái hàng đang đi đường và hạch toán giá vốn nội bộ.
- **Khuyến nghị mặc định**: **Hoãn sang v2.0**. Trong phiên bản v1.0 MVP, quản lý danh mục và số dư tồn kho độc lập theo từng `BranchId`.
- **Trạng thái**: `OPEN QUESTION` (Không làm nghẽn tiến độ MVP)

---

### OQ-05: Chính Sách Bắt Buộc Thanh Toán Trước Khi Khách Tự Quét QR Gọi Món (Phase 2 Prepayment Policy)
- **Bối cảnh**: Trong Giai đoạn 2, khách quét mã QR tại bàn để xem thực đơn và gọi món trực tiếp từ điện thoại thông minh.
- **Câu hỏi**: Đơn gọi món qua QR của khách có bắt buộc phải thanh toán trực tuyến ngay (qua VietQR hoặc MoMo) trước khi gửi xuống bếp hay khách có thể gọi món tự do và trả tiền sau tại quầy thu ngân khi ăn xong?
- **Tại sao quan trọng**: Bắt buộc trả tiền trước giúp chống tình trạng "gọi món đùa cợt" hoặc bấm nhầm, nhưng gây bất tiện nếu khách muốn gọi thêm nhiều đợt đồ uống trong suốt bữa ăn.
- **Khuyến nghị mặc định**: **Cấu hình linh hoạt theo mô hình nhà hàng**:
  - Mô hình đồ ăn nhanh / Phục vụ tại quầy: Bắt buộc thanh toán ngay khi gọi món.
  - Mô hình nhà hàng truyền thống / Phục vụ tại bàn: Thanh toán khi ăn xong (Phục vụ xác nhận đơn hoặc mở khóa phiên bàn).
- **Trạng thái**: `OPEN QUESTION` (Xem xét trong Giai đoạn 2)

---

### OQ-06: Quy Tắc Tính Thuế GTGT Trên Phí Dịch Vụ (Service Charge Taxability)
- **Bối cảnh**: Một số nhà hàng áp dụng phí dịch vụ 5%. Theo quy định thuế tại Việt Nam, tùy theo mô hình đăng ký kinh doanh, thuế GTGT có thể áp dụng trên $(\text{Tiền món sau giảm} + \text{Phí dịch vụ})$ hoặc chỉ tính trên $\text{Tiền món sau giảm}$.
- **Câu hỏi**: Thuế GTGT được tính trên tổng số tiền bao gồm cả phí dịch vụ, hay thuế GTGT và phí dịch vụ được tính toán độc lập song song trên số tiền món sau giảm giá?
- **Tại sao quan trọng**: Tác động trực tiếp đến thuật toán làm tròn số và việc tuân thủ pháp lý của engine tính tiền.
- **Khuyến nghị mặc định**: **Cung cấp công tắc cấu hình cấp chi nhánh**:
  - `IsServiceChargeTaxable: true` $\rightarrow \text{Thuế GTGT} = (\text{Tiền món sau giảm} + \text{Phí dịch vụ}) \times \text{Thuế suất GTGT}$.
  - `IsServiceChargeTaxable: false` $\rightarrow \text{Thuế GTGT} = \text{Tiền món sau giảm} \times \text{Thuế suất GTGT}$.  
  Mặc định là `true` (thông lệ phổ biến của ngành ẩm thực Việt Nam).
- **Trạng thái**: `OPEN QUESTION` (Chờ Chủ Dự Án xác nhận)

---

### OQ-10: Ranh Giới Sở Hữu Thực Thể & Phân Vùng Dữ Liệu Đa Chi Nhánh (Data Partitioning Boundaries)
- **Bối cảnh**: Căn cứ Quyết định của Chủ Dự Án OD-01, phạm vi nghiệp vụ MVP giới hạn nghiêm ngặt cho một nhà hàng độc lập tại một địa điểm kinh doanh vật lý duy nhất. Nền tảng kỹ thuật hiện có (như các khóa `OrganizationId` và `BranchId`) được giữ lại ở những nơi hợp lý để tránh phải đập đi xây lại trong tương lai.
- **Câu hỏi**: Kiến trúc sư giải pháp nên phân vùng quyền sở hữu thực thể trong lược đồ cơ sở dữ liệu như thế nào trong Stage 2?
  - Các thực thể danh mục dùng chung (`Category`, `MenuItem`, `RecipeBOM`) so với các thực thể vận hành giao dịch (`DiningArea`, `RestaurantTable`, `TableSession`, `Order`, `Invoice`, `ShiftSession`).
  - Thực thể nào bắt buộc phải có cả hai khóa `OrganizationId` và `BranchId`, và thực thể nào chỉ cần `BranchId`?
- **Khuyến nghị mặc định**: Ủy quyền cho Kiến trúc sư Giải pháp đánh giá chi tiết theo từng thực thể trong Stage 2. Bảo đảm không áp đặt cấu trúc khóa kép dư thừa lên các thực thể chỉ cần một ranh giới chi nhánh duy nhất, đồng thời bảo vệ tuyệt đối tính phân lập dữ liệu.
- **Trạng thái**: `OPEN QUESTION` (Ủy quyền cho Kiến trúc sư Giải pháp đánh giá trong Stage 2)
