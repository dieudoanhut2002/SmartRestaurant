# Báo Cáo Đánh Giá Sản Phẩm & Thẩm Định Nghiệp Vụ Chuyên Sâu (Product Review Report) — SmartRestaurant

- **Mã tài liệu**: `docs/product/PRODUCT-REVIEW-REPORT.md`
- **Phiên bản**: 1.0.0
- **Trạng thái**: `RESOLVED & CONFIRMED` (Toàn bộ phát hiện đã được Chủ dự án thẩm định và phê duyệt trong Task 02.6)
- **Vai trò thực hiện**: Senior Product Architect & Chuyên gia Nghiệp vụ Nhà hàng (Độc lập)
- **Ngày hoàn thành**: 02/11/2026

---

## 1. Đánh Giá Tổng Quan (Overall Assessment)

Bộ tài liệu đặc tả Giai đoạn 1 (Stage 1) của **SmartRestaurant** hiện bao gồm 10 tài liệu nền tảng, 4 tài liệu quy trình chuyên sâu và 6 bản ghi quyết định kiến trúc/nghiệp vụ. Nhìn chung, hệ thống tài liệu đã thể hiện được tầm nhìn tốt về mô hình **Modular Monolith**, bám sát định hướng **Vietnamese-First**, và bao quát được các nghiệp vụ cốt lõi từ Sơ đồ bàn, POS gọi món, Điều phối bếp KDS, đến Quầy thu ngân.

Tuy nhiên, dưới góc nhìn phản biện độc lập của một Chuyên gia Vận hành Nhà hàng và Kiến trúc sư Sản phẩm Cao cấp, **bộ tài liệu hiện tại vẫn còn tồn tại những lỗ hổng nghiệp vụ vận hành thực tế quan trọng, một số giả định mang tính lý thuyết, và một số mâu thuẫn/thiếu sót cần được khắc phục triệt để trước khi bước sang Giai đoạn 2 (Thiết kế Kiến trúc Kỹ thuật)**.

### Thống Kê Các Phát Hiện (Findings Summary):
- **CRITICAL (Nghiêm trọng - Bắt buộc xử lý trước khi thiết kế kiến trúc)**: 4 phát hiện
- **HIGH (Cao - Cần thống nhất trước khi chốt sơ đồ CSDL ERD)**: 6 phát hiện
- **MEDIUM (Trung bình - Có thể tinh chỉnh trong đặc tả module chi tiết)**: 5 phát hiện
- **LOW (Thấp - Có thể xem xét ở các bản phát hành tiếp theo)**: 3 phát hiện

---

## 2. Các Vấn Đề Nghiêm Trọng (Critical Issues — CRITICAL)

### [REV-01] [CRITICAL] Thiếu Khái Niệm "Ngày Kinh Doanh" (Business Date) vs "Ngày Lịch" (Calendar Date) Cho Ca Làm Việc Xuyên Đêm
- **Tài liệu liên quan**: [BUSINESS-REQUIREMENTS.md](./BUSINESS-REQUIREMENTS.md), [CASHIER-PAYMENT-WORKFLOW.md](./workflows/CASHIER-PAYMENT-WORKFLOW.md)
- **Vấn đề**:
  - Hầu hết các nhà hàng, quán nhậu, quán bar, lẩu/nướng tại Việt Nam đều hoạt động qua nửa đêm (ví dụ: mở cửa từ 17:00 hôm nay đến 02:00 hoặc 03:00 sáng hôm sau).
  - Tài liệu hiện tại gắn chặt toàn bộ doanh thu, lượt khách, ca làm việc và đơn hàng với ngày giờ hệ thống (`DateTime.UtcNow` chuyển đổi sang ngày lịch `dd/MM/yyyy`).
- **Tác động nghiệp vụ**:
  - Bàn khách vào lúc 23:30 ngày 02/11 và thanh toán lúc 01:15 ngày 03/11 sẽ bị hạch toán doanh thu sang ngày 03/11.
  - Báo cáo doanh thu ca tối bị xé làm đôi giữa 2 ngày lịch. Báo cáo chốt ca (Z-Report) không thể đối soát được với tiền kiểm kê thực tế của ca tối hôm đó.
  - Sai lệch toàn bộ báo cáo doanh thu theo ngày kinh doanh, sai lệch hạch toán kế toán và đối soát thuế.
- **Khuyến nghị giải quyết**:
  - Bổ sung trường bắt buộc `BusinessDate` (Ngày kinh doanh) vào `ShiftSession`, `DiningSession`, `Order` và `Invoice`.
  - Quy tắc: Mọi đơn hàng mở trong một ca làm việc (dù kéo dài qua 00:00 đêm) đều thuộc về `BusinessDate` của ca đó. Báo cáo doanh thu ngày sẽ tổng hợp theo `BusinessDate`, không gom theo ngày lịch tự nhiên.
- **Cần Chủ dự án phê duyệt**: **CÓ** $\rightarrow$ **ĐÃ PHÊ DUYỆT (CONFIRMED qua PDR-007)**: Áp dụng Ngày Kinh Doanh (Business Date) gắn với Ca làm việc. Toàn bộ đơn hàng mở trong ca đêm đều hạch toán vào Ngày kinh doanh của ca đó.

---

### [REV-02] [CRITICAL] Lỗ Hổng Cơ Chế Giữ Chỗ Tồn Kho Trong PDR-004 (BOM Inventory Deduction Timing)
- **Tài liệu liên quan**: [docs/product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md](./decisions/PDR-004-inventory-bom-deduction-trigger-policy.md), [KITCHEN-WORKFLOW.md](./workflows/KITCHEN-WORKFLOW.md)
- **Vấn đề**:
  - PDR-004 quy định: *Chỉ trừ tồn kho nguyên liệu khi món ăn chuyển sang trạng thái `Đã xong - Chờ bưng` (ReadyToServe)*.
  - Giả sử nhà hàng chỉ còn nguyên liệu cho đúng 2 suất "Cá hồi nướng" (thời gian nướng 20 phút).
  - Bàn 01 gọi 2 suất lúc 19:00. Bếp bấm `Đang nấu` (Cooking). Vì chưa `ReadyToServe`, tồn kho trên hệ thống vẫn báo còn 2 suất.
  - Lúc 19:05, Bàn 02 gọi tiếp 2 suất Cá hồi nướng. Hệ thống kiểm tra thấy tồn kho vẫn còn 2 suất nên cho phép Waiter gửi đơn thành công. Bếp nhận đơn thứ 2 mới tá hỏa vì trong tủ lạnh không còn miếng cá hồi nào!
- **Tác động nghiệp vụ**:
  - Gây hiện tượng "bán khống" món ăn (overselling) trong giờ cao điểm đối với các món nấu lâu và các nguyên liệu đắt tiền có số lượng hạn chế trong ngày.
  - Phục vụ phải chạy ra xin lỗi khách tại Bàn 02, làm giảm nghiêm trọng uy tín nhà hàng.
- **Khuyến nghị giải quyết**:
  - Sửa đổi quy tắc trừ kho thành **Cơ chế 2 bước (Two-Phase Inventory Lock)**:
    1. Khi đơn được **Báo bếp (Fire)** $\rightarrow$ Hệ thống chuyển số lượng nguyên liệu tương ứng sang trạng thái **Tạm giữ (Reserved / Allocated)**. Món ăn tự động chuyển sang `Tạm hết món (86)` nếu tổng tồn kho khả dụng trừ đi lượng tạm giữ $\le 0$.
    2. Khi bếp bấm **Đã xong (ReadyToServe)** $\rightarrow$ Chuyển từ **Tạm giữ** thành **Xuất kho tiêu hao thực tế (Deducted)**.
    3. Nếu món bị **Hủy sau khi báo bếp** $\rightarrow$ Giải phóng lượng tạm giữ hoàn lại tồn kho khả dụng (nếu chưa nấu), hoặc ghi vào Sổ hủy hỏng (nếu đã chế biến).
- **Cần Chủ dự án phê duyệt**: **CÓ** $\rightarrow$ **ĐÃ PHÊ DUYỆT (CONFIRMED qua PDR-004 v2)**: Áp dụng cơ chế Khóa tồn kho 2 bước (Tạm giữ khi Báo bếp $\rightarrow$ Trừ thực tế khi Nấu xong). Đồng thời dời trừ kho BOM gram/ml xuống v1.1; MVP v1.0 quản lý trực tiếp theo suất/đĩa/chai/lon.

---

### [REV-03] [CRITICAL] Thiếu Quy Trình Xử Lý Xung Đột Thanh Toán Chậm Của Cổng VietQR (Bank Webhook Race Condition)
- **Tài liệu liên quan**: [CASHIER-PAYMENT-WORKFLOW.md](./workflows/CASHIER-PAYMENT-WORKFLOW.md) §4, [OPEN-QUESTIONS.md](./OPEN-QUESTIONS.md)
- **Vấn đề**:
  - Khách quét mã VietQR để trả 600.000 ₫. Do hệ thống ngân hàng bị nghẽn mạng Napas hoặc App ngân hàng của khách bị treo, màn hình thu ngân không nhận được tín hiệu xác nhận thanh toán sau 2 phút chờ đợi.
  - Khách vội về nên chuyển sang trả **Tiền mặt 600.000 ₫**. Thu ngân bấm xác nhận thu tiền mặt và hoàn tất hóa đơn, giải phóng bàn sang `Cần dọn dẹp`.
  - 3 phút sau, giao dịch chuyển khoản ngân hàng Napas thông mạng, webhook ngân hàng gửi tín hiệu báo: *Đã nhận thành công 600.000 ₫ từ khách hàng cho đơn hàng này!*
  - Khách hàng đã bị trừ tiền trong tài khoản ngân hàng VÀ đã trả tiền mặt tại quầy!
- **Tác động nghiệp vụ**:
  - Thu ngân bị thừa tiền, khách hàng phát hiện bị trừ 2 lần sẽ quay lại khiếu nại, kế toán không có quy trình đối soát tự động khoản tiền "mồ côi" này.
- **Khuyến nghị giải quyết**:
  - Bổ sung trạng thái giao dịch ngân hàng: `Đang chờ webhook` (Pending Webhook).
  - Khi thu ngân chuyển phương thức thanh toán sang Tiền mặt cho hóa đơn đang chờ QR, hệ thống phải tự động **Vô hiệu hóa mã QR tham chiếu (Invalidate QR Ref)**.
  - Nếu Webhook ngân hàng bắn về cho một mã đã bị vô hiệu hóa hoặc hóa đơn đã đóng bằng tiền mặt: Hệ thống tự động ghi nhận vào tài khoản **Tiền chuyển khoản chờ hoàn trả (Unallocated Deposit)**, đồng thời hiển thị cảnh báo đỏ trên màn hình Thu ngân và Quản lý: *"Phát hiện khách hàng Bàn 04 vừa chuyển khoản trùng 600.000 ₫ sau khi đã thanh toán tiền mặt. Vui lòng bấm Hoàn tiền cho khách."*
- **Cần Chủ dự án phê duyệt**: **CÓ** $\rightarrow$ **ĐÃ PHÊ DUYỆT (CONFIRMED qua PDR-008)**: Vô hiệu hóa QR khi thu tiền mặt; webhook về muộn tự động ghi vào Quỹ tiền chờ hoàn trả và bật cảnh báo thu ngân.

---

### [REV-04] [CRITICAL] Xung Đột Trạng Thái Khi Đang Tách Hóa Đơn (Split Bill) Mà Có Món Bị Hủy Hoặc Gọi Thêm
- **Tài liệu liên quan**: [CASHIER-PAYMENT-WORKFLOW.md](./workflows/CASHIER-PAYMENT-WORKFLOW.md) §2, [ORDER-WORKFLOW.md](./workflows/ORDER-WORKFLOW.md)
- **Vấn đề**:
  - Tài liệu cho phép chia hóa đơn thành Hóa đơn A (đã thanh toán) và Hóa đơn B (chưa thanh toán).
  - Trong lúc Hóa đơn B đang mở, nếu khách tại bàn yêu cầu gọi thêm 2 lon bia, hoặc yêu cầu hủy 1 món trên Hóa đơn B do bếp làm sai, đơn hàng gốc sẽ bị phân mảnh trạng thái như thế nào?
  - Liệu Bàn có được giải phóng khi Hóa đơn B thanh toán xong nếu có món gọi thêm phát sinh trong lúc đó?
- **Tác động nghiệp vụ**:
  - Dễ dẫn đến tình trạng treo bàn (Deadlock table state) hoặc bỏ sót món gọi thêm không được tính vào hóa đơn nào, gây thất thoát tiền cho nhà hàng.
- **Khuyến nghị giải quyết**:
  - Áp dụng mô hình **Phiên bàn đa hóa đơn (Multi-Bill per Table Session)**: Tuyệt đối KHÔNG khóa bàn. Cho phép bồi bàn tiếp tục gọi thêm món vào các hóa đơn con chưa thanh toán với cơ chế chỉ định đích tường minh.
  - Cho phép hủy thao tác tách khi chưa thanh toán; chỉ giải phóng bàn khi toàn bộ hóa đơn con đã thanh toán và các món ăn đã được phục vụ xong.
- **Cần Chủ dự án phê duyệt**: **CÓ** $\rightarrow$ **ĐÃ PHÊ DUYỆT (CONFIRMED qua PDR-009)**: Phê duyệt mô hình Phiên bàn đa hóa đơn (Multi-Bill per Table Session), không khóa bàn khi tách bill, lưu đầy đủ audit log.

---

## 3. Các Yêu Cầu Nghiệp Vụ Còn Thiếu (Missing Business Requirements — HIGH & MEDIUM)

### [REV-05] [HIGH] Thiếu Nghiệp Vụ "Tách Bàn Đã Gộp" (Unmerge Tables)
- **Tài liệu liên quan**: [FLOOR-PLAN-WORKFLOW.md](./workflows/FLOOR-PLAN-WORKFLOW.md) §3.2
- **Vấn đề**:
  - Tài liệu đã đặc tả chi tiết nghiệp vụ **Gộp bàn (Merge Tables)** khi ghép Bàn 04 và Bàn 05 thành bàn lớn.
  - Tuy nhiên, hoàn toàn thiếu nghiệp vụ ngược lại: **Hủy gộp bàn (Unmerge Tables)**.
  - Trong thực tế: Một đoàn khách 10 người ngồi Bàn 04 + Bàn 05. Sau 1 tiếng, nhóm 4 người ở Bàn 05 có việc bận xin phép về trước và muốn tách riêng phần của họ ra để tính tiền, trong khi nhóm Bàn 04 vẫn tiếp tục ngồi ăn uống.
- **Khuyến nghị**: Bổ sung quy trình `Hủy gộp bàn`: Chuyển các món do Bàn 05 gọi sang một phiên bàn độc lập mới tại Bàn 05, giữ lại các món của Bàn 04, và mở khóa Bàn 05 để thanh toán độc lập.
- **Cần Chủ dự án phê duyệt**: **CÓ**.

---

### [REV-06] [HIGH] Thiếu Nghiệp Vụ Điều Chỉnh Bảng Giá Theo Khu Vực & Phụ Thu Phòng VIP
- **Tài liệu liên quan**: [BUSINESS-REQUIREMENTS.md](./BUSINESS-REQUIREMENTS.md) §2.1, [PDR-002](./decisions/PDR-002-hierarchical-area-room-table-model.md)
- **Vấn đề**:
  - Tài liệu phân chia Khu vực gồm: Sân vườn, Tầng 1, Tầng 2, Phòng VIP.
  - Trong thực tế nhà hàng Việt Nam:
    - Phòng VIP thường áp dụng mức chiết khấu khác hoặc có **Phí dịch vụ phòng VIP** (ví dụ: phụ thu 10% hoặc phụ thu theo giờ).
    - Một số khu vực như Rooftop / Sân vườn có thể áp dụng biểu giá đồ uống cao hơn khu vực trong nhà.
    - Chuyển bàn từ Tầng 1 lên Phòng VIP: Tiền món cũ có bị áp dụng mức phụ thu mới không?
- **Khuyến nghị**: Bổ sung cơ chế cấu hình **Phụ thu theo Khu vực / Phòng VIP** (theo % hoặc theo số tiền cố định). Khi chuyển bàn từ khu vực thường sang khu vực có phụ thu, hệ thống phải hiển thị popup hỏi thu ngân: *"Áp dụng phụ thu phòng VIP cho toàn bộ đơn hay chỉ cho các món gọi từ thời điểm này?"*.
- **Cần Chủ dự án phê duyệt**: **CÓ**.

---

### [REV-07] [HIGH] Thiếu Quy Trình Quản Lý Khách Hàng Nợ (Công Nợ / Ghi Sổ / Khách VIP Doanh Nghiệp)
- **Tài liệu liên quan**: [CASHIER-PAYMENT-WORKFLOW.md](./workflows/CASHIER-PAYMENT-WORKFLOW.md)
- **Vấn đề**:
  - Tài liệu chỉ liệt kê 3 phương thức: Tiền mặt, Thẻ ngân hàng, VietQR.
  - Rất nhiều nhà hàng tại Việt Nam phục vụ khách quen, sếp doanh nghiệp tiếp khách ký hóa đơn nợ để công ty thanh toán vào cuối tháng (Thanh toán công nợ / Ký sổ).
- **Khuyến nghị**: Bổ sung phương thức thanh toán **Ký nợ / Công nợ (Charge to Account)**. Yêu cầu nhập Tên khách hàng/Doanh nghiệp, Mã số thuế, Hạn mức nợ và Mã PIN Quản lý phê duyệt. Hóa đơn được đóng nhưng chuyển trạng thái thành `Chờ thanh toán công nợ` (Accounts Receivable).
- **Cần Chủ dự án phê duyệt**: **CÓ**.

---

### [REV-08] [MEDIUM] Thiếu Nghiệp Vụ "Món Ăn Kèm Theo Bếp" vs "Đồ Uống Phục Vụ Ngay"
- **Tài liệu liên quan**: [ORDER-WORKFLOW.md](./workflows/ORDER-WORKFLOW.md), [KITCHEN-WORKFLOW.md](./workflows/KITCHEN-WORKFLOW.md)
- **Vấn đề**:
  - Khi khách gọi bia ướp lạnh, nhân viên phục vụ thường lấy trực tiếp từ tủ lạnh đồ uống tại trạm phục vụ của tầng thay vì gửi xuống bếp/quầy bar pha chế xa xôi.
  - Hiện tại hệ thống quy định mọi đồ uống đều phải gửi xuống `Quầy Bar KDS`. Đối với các quán nhỏ chỉ có 1 tủ bia, việc bắt đầu bếp bar bấm KDS cho 1 chai nước ngọt là dư thừa và cản trở tốc độ.
- **Khuyến nghị**: Bổ sung cờ cấu hình trên từng món đồ uống: `Phục vụ trực tiếp tại bàn (FOH Direct Pick)` vs `Chế biến tại Quầy Bar (Bar KDS Routing)`. Nếu là món phục vụ trực tiếp, khi Báo bếp, món tự động đánh dấu `Đã xong (ReadyToServe)` và in phiếu kiểm đồ cho nhân viên phục vụ lấy ngay tại tủ.
- **Cần Chủ dự án phê duyệt**: Không (Quyết định nội bộ module).

---

### [REV-09] [MEDIUM] Thiếu Quản Lý Bàn Trong Cùng Một Phòng (Multiple Tables in Shared Room)
- **Tài liệu liên quan**: [FLOOR-PLAN-WORKFLOW.md](./workflows/FLOOR-PLAN-WORKFLOW.md) §3.3
- **Vấn đề**:
  - Tài liệu hiện tại mới chỉ giải quyết trường hợp "Bao trọn phòng" (Book Entire Room).
  - Chưa làm rõ: Nếu Phòng tiệc có 4 bàn (Bàn 1, 2, 3, 4), hai đoàn khách lạ cùng ngồi chung phòng (Bàn 1 và Bàn 3) thì giao diện hiển thị thế nào, âm thanh và sự riêng tư được kiểm soát ra sao.
- **Khuyến nghị**: Cho phép một Phòng ăn chứa nhiều Bàn hoạt động hoàn toàn độc lập như ở Khu vực ngoài trời, trừ khi Quản lý chủ động bấm nút "Bao trọn phòng".
- **Cần Chủ dự án phê duyệt**: Không.

---

## 4. Bất Cập Trong Các Quy Trình Nghiệp Vụ (Workflow Problems)

### [REV-10] [HIGH] Bất Cập Quy Trình Đồng Thời (Concurrency) Khi Hai Waiter Cùng Chạm Vào Một Bàn
- **Tài liệu liên quan**: [ORDER-WORKFLOW.md](./workflows/ORDER-WORKFLOW.md) §3
- **Vấn đề**:
  - Tài liệu ghi: *"Pessimistic Cart Lock hoặc Optimistic Session Concurrency"*. Đây là lựa chọn kỹ thuật chưa dứt khoát, chưa đưa ra hành vi nghiệp vụ rõ ràng cho người dùng.
  - Nếu Waiter A đang mở Bàn 04 để chọn món, Waiter B cũng cầm máy tính bảng đến Bàn 04 và chọn món cho khách.
  - Nếu áp dụng khóa cứng (Pessimistic Lock): Waiter B bị văng ra không làm được gì.
  - Nếu áp dụng khóa lạc quan (Optimistic Concurrency): Ai bấm gửi trước thì được, người gửi sau bị báo lỗi mất sạch giỏ hàng vừa chọn!
- **Khuyến nghị**: Quy định rõ trải nghiệm nghiệp vụ:
  - Áp dụng cơ chế **Gộp đơn tự động không xung đột (Append-Only Delta Firing)**.
  - Bàn 04 không bị khóa màn hình. Waiter A chọn món 1 & 2 $\rightarrow$ Bấm Báo bếp $\rightarrow$ Bổ sung vào đơn.
  - Waiter B chọn món 3 & 4 $\rightarrow$ Bấm Báo bếp $\rightarrow$ Hệ thống tự động kiểm tra phiên bàn và nối tiếp món 3 & 4 vào danh sách dưới dạng một Đợt gọi món mới mà không ghi đè món của Waiter A.
- **Cần Chủ dự án phê duyệt**: **CÓ**.

---

### [REV-11] [MEDIUM] Thiếu Quy Trình "Đổi Trả Món Lỗi / Đổi Món Tại Bàn"
- **Tài liệu liên quan**: [ORDER-WORKFLOW.md](./workflows/ORDER-WORKFLOW.md), [KITCHEN-WORKFLOW.md](./workflows/KITCHEN-WORKFLOW.md)
- **Vấn đề**:
  - Khách gọi "Bò né", bếp làm bị cháy hoặc mặn, khách yêu cầu đổi đĩa khác.
  - Hiện tại quy trình chỉ có: Hủy món hoặc Gọi món mới.
  - Nếu hủy món rồi gọi lại: Khách bị tính tiền 2 lần trên hóa đơn tạm tính nếu nhân viên quên xóa, hoặc số liệu báo cáo bếp bị tính là 2 suất bán thay vì 1 suất hỏng làm lại.
- **Khuyến nghị**: Bổ sung thao tác **Làm lại món (Remake Dish)** trên POS/KDS: Giữ nguyên giá tiền 1 món trên hóa đơn khách hàng, nhưng phát sinh một phiếu bếp ưu tiên màu tím: *"Làm lại do lỗi bếp"* và tự động trừ nguyên liệu suất hỏng vào Sổ Hủy Hỏng (Waste Log).
- **Cần Chủ dự án phê duyệt**: Không.

---

## 5. Đánh Giá Khả Thi Của Phạm Vi MVP (MVP Scope Feasibility)

Bảng phân loại tính năng MVP hiện tại trong [MVP-SCOPE.md](./MVP-SCOPE.md) đã khá chặt chẽ, tuy nhiên cần điều chỉnh một số tính năng để giảm rủi ro trễ tiến độ:

| Tính Năng Hiện Tại Trong MVP | Đánh Giá Độ Phức Tạp | Khuyến Nghị Điều Chỉnh | Lý Do |
| :--- | :---: | :---: | :--- |
| **Thiết Kế Sơ Đồ Bàn 2D Kéo Thả (Konva.js)** | Rất Cao | **Thu gọn phạm vi MVP**: Hỗ trợ lưới tọa độ đơn giản (Grid snap) và các hình cơ bản (tròn, vuông, chữ nhật). Bỏ tính năng xoay góc tự do và vẽ tường cong trong MVP. | Giảm 50% thời gian code Canvas 2D mà vẫn đáp ứng 95% sơ đồ nhà hàng thực tế. |
| **Tách Hóa Đơn (Split Bill) Bằng Chia Đều 1/N** | Trung Bình | **Giữ lại trong MVP**. | Cực kỳ phổ biến khi giới trẻ/văn phòng đi ăn trưa. |
| **Tách Hóa Đơn Bằng Cách Chọn Từng Món** | Cao | **Giữ lại trong MVP** nhưng giới hạn tối đa 3 hóa đơn con cho một lần tách. | Tránh giao diện đa cột quá phức tạp trên màn hình cảm ứng nhỏ. |
| **Định Lượng Nguyên Liệu BOM Tự Động Trừ Kho** | Rất Cao | **Khuyến nghị dời xuống v1.1**: MVP chỉ nên quản lý số lượng tồn kho của **Món bán trực tiếp** (ví dụ: Tồn 50 chai bia, tồn 20 suất lẩu). Chưa trừ sâu xuống cấp gram thịt bò, ml nước mắm. | Cấu hình BOM cực kỳ tốn thời gian của chủ nhà hàng. 90% nhà hàng mở mới chưa kịp chuẩn hóa định lượng gram/ml trong tháng đầu. |

> [!WARNING]
> **Cảnh báo rủi ro tiến độ**: Nếu bắt buộc triển khai toàn bộ hệ thống định lượng BOM sâu (gram/ml) ngay trong bản MVP v1.0, dự án có nguy cơ kéo dài thời gian kiểm thử và làm chậm ngày ra mắt sản phẩm ít nhất 4–6 tuần.

---

## 6. Ranh Giới Module & Chồng Chéo Trách Nhiệm (Module Boundary Concerns)

Kiểm tra 14 module trong [MODULE-OVERVIEW.md](./MODULE-OVERVIEW.md):

1. **Chồng chéo giữa Module 10 (Cashier & Billing) và Module 11 (Payment Management)**:
   - *Vấn đề*: `Invoice` (Hóa đơn) và `Payment` (Khoản thanh toán) bị tách thành 2 module riêng biệt. Trong mô hình Modular Monolith giai đoạn đầu, việc tách riêng Payment tạo ra sự phân mảnh không cần thiết giữa tính toán hóa đơn và ghi nhận giao dịch thanh toán.
   - *Khuyến nghị*: Gom **Module 10 và Module 11** thành một module thống nhất: **Module Thanh Toán & Thu Ngân (Billing & Cashier Management)** để đảm bảo tính toàn vẹn giao dịch (Atomic Transaction) khi thanh toán nhiều phương thức.
2. **Khái niệm đặt nhầm chỗ**:
   - `ManagerPinOverride` hiện được đặt trong `Module 02: User & Access Management`. Tuy nhiên, mã PIN Quản lý là một cơ chế ủy quyền tại chỗ (Contextual In-Place Authorization), nó gắn liền với các hành động cụ thể tại POS (`Module 07`) và Thu ngân (`Module 10`). Cần định nghĩa rõ interface ủy quyền chung thay vì phụ thuộc cứng vào Module 02.

---

## 7. Rà Soát Tính Nhất Quán Bản Địa Hóa Tiếng Việt (Vietnamese Localization)

Kiểm tra toàn bộ văn bản trong [VIETNAMESE-LOCALIZATION-STANDARD.md](./VIETNAMESE-LOCALIZATION-STANDARD.md) và các tài liệu khác:

1. **Điểm tốt**:
   - Bộ thuật ngữ tiếng Việt đã được chuẩn hóa rất xuất sắc với cấu trúc 4 cột, loại bỏ các từ dịch thô kệch (như "Chiếm đóng", "Khả dụng", "Tổng phụ").
   - Quy chuẩn tiền tệ VNĐ `#,,### ₫` và ngày giờ `dd/MM/yyyy` hoàn toàn chính xác theo văn hóa Việt Nam.
2. **Điểm cần làm rõ thêm**:
   - Từ ngữ trên KDS: Thẻ món ăn dùng từ *"Món ưu tiên"* hay *"Làm gấp"*. Trong thực tế bếp Việt Nam, các đầu bếp thích từ ngắn gọn 2 chữ: **"LÀM GẤP"** (hoặc **"ƯU TIÊN"**), không nên dùng câu dài *"Món ưu tiên (Gấp)"* trên thẻ KDS vì tốn diện tích hiển thị.
   - Phiếu in tạm tính: Cần ghi rõ dòng chữ: *"Phiếu này chỉ dùng để đối chiếu, không phải hóa đơn thanh toán"* để tránh nhân viên gian lận đưa phiếu tạm tính cho khách rồi bỏ túi tiền mặt không bấm xuất hóa đơn.

---

## 8. Lỗ Hổng Yêu Cầu Phi Chức Năng (Non-Functional Requirements Gaps)

1. **Yêu cầu NFR-PERF-05 chưa có cơ sở**:
   - Trong `NON-FUNCTIONAL-REQUIREMENTS.md`, mục `NFR-PERF-05` đặt chỉ tiêu: *"Sustain up to 50 concurrent orders and 200 active tables per branch"*. Đây là một chỉ tiêu ước tính chưa qua đo kiểm (Benchmark). Cần duy trì cờ `OPEN QUESTION` cho chỉ tiêu này cho đến khi có kết quả tải thực tế.
2. **Khả năng hoạt động của Máy in Nhiệt qua mạng LAN (Network Printing Reliability)**:
   - Nhà hàng thường dùng máy in nhiệt cắm dây mạng LAN hoặc qua Wi-Fi. Khi máy in hết giấy hoặc kẹt giấy, driver in mạng có thể bị treo luồng (thread lock).
   - NFR cần bổ sung: **Timeout in ấn tối đa $\le 3\text{ giây}$**, nếu máy in không phản hồi phải chuyển sang hàng đợi in nền (Background Print Spooler) và thông báo trên POS: *"Máy in bếp đang kẹt giấy hoặc mất kết nối"*, không được làm đơ màn hình cảm ứng của phục vụ.

---

## 9. Mâu Thuẫn Giữa Các Tài Liệu (Document Contradictions)

| Mã mâu thuẫn | Tài liệu A | Tài liệu B | Bản chất mâu thuẫn | Hướng xử lý |
| :--- | :--- | :--- | :--- | :--- |
| **CONTR-01** | `PRD.md` §4.2 ghi: *"Central multi-branch warehouse logistics excluded from MVP"* | `MODULE-OVERVIEW.md` §2 Module 01 ghi: *"Multi-branch organization hierarchy in MVP"* | Gây hiểu nhầm giữa nghiệp vụ chuỗi chi nhánh và mô hình MVP. | Đồng bộ theo Quyết định Chủ dự án OD-01: MVP phục vụ duy nhất 01 nhà hàng độc lập tại 01 địa điểm vật lý; bảo lưu cấu trúc kỹ thuật đa chi nhánh sẵn có để tránh lãng phí công sức, không đưa nghiệp vụ quản lý chuỗi hay điều chuyển kho liên chi nhánh vào phạm vi MVP; giao Solution Architect rà soát chi tiết tại Stage 2. |
| **CONTR-02** | `PDR-004` ghi: *"Trừ kho nguyên liệu khi món chuyển sang ReadyToServe"* | `BUSINESS-REQUIREMENTS.md` §2.2 `BR-MENU-004` ghi: *"Trừ kho tự động theo công thức khi chế biến"* | Khái niệm "khi chế biến" mâu thuẫn với "khi nấu xong (ReadyToServe)". | Đồng bộ theo giải pháp Khóa 2 bước (Two-Phase Lock) tại `REV-02`. |

---

## 10. Danh Sách Các Quyết Định Nghiệp Vụ Đã Được Chủ Dự Án Phê Duyệt (Task 02.6)

Toàn bộ các phát hiện từ cuộc Đánh giá Sản phẩm đã được Chủ dự án phân tích, chỉ đạo và chính thức phê duyệt phương án giải quyết trong **Task 02.6**:

| Mã phát hiện / Quyết định | Quyết định & Tài liệu PDR | Trạng thái | Nội dung đã được duyệt & Ràng buộc áp dụng |
| :--- | :--- | :---: | :--- |
| **D1 / REV-01** | [PDR-007](./decisions/PDR-007-business-date-shift-management.md) | `CONFIRMED` | Cấu hình `BusinessDayCutoffTime` theo chi nhánh (mặc định 06:00). Đơn tạo trước cutoff thuộc ngày kinh doanh đó. Tách biệt `BusinessDate` với timestamp vật lý. Không đổi hồi tố ngày kinh doanh của đơn cũ. |
| **D2 / REV-02** | [PDR-004](./decisions/PDR-004-inventory-bom-deduction-trigger-policy.md) | `CONFIRMED` | Công thức `Tồn khả dụng = Tồn thực tế - Tồn tạm giữ`. Tạm giữ khi xác nhận gửi bếp, tự động cảnh báo & khóa món khi hết hàng. Giải phóng khi hủy hợp lệ. Ghi Waste Log khi hủy sau chế biến. Kiểm soát đồng thời chống âm kho và chống trừ 2 lần khi retry. |
| **D3 / REV-03** | [PDR-008](./decisions/PDR-008-vietqr-delayed-webhook-handling.md) | `CONFIRMED` | Cho phép thu tiền mặt khi QR timeout. QR về muộn đưa vào "Giao dịch treo đối soát", thông báo tức thì cho thu ngân & quản lý. Đối soát mã GD ngân hàng & mã tham chiếu để chống trùng. Không tự động cộng tiền lần 2, không tự động hoàn tiền khi chưa xác minh. Xử lý thủ công có phê duyệt. |
| **D4 / REV-04** | [PDR-009](./decisions/PDR-009-multi-bill-table-session-split-billing.md) | `CONFIRMED` | Mô hình Phiên bàn đa hóa đơn (`TableSession` chứa nhiều `SubBill`). **KHÔNG khóa bàn khi tách bill**. Bồi bàn tiếp tục gọi món vào các bill chưa thanh toán (chỉ định đích rõ ràng). Cho phép hủy tách khi chưa thanh toán. Chỉ giải phóng bàn khi 100% nghĩa vụ hoàn tất. |
| **D5 (MVP BOM Scope)** | [PDR-004](./decisions/PDR-004-inventory-bom-deduction-trigger-policy.md) | `CONFIRMED` | MVP quản lý hàng hóa/thành phẩm đếm được (lon, chai, phần). Dời quản lý BOM nguyên liệu thô theo gram/ml sang v1.1. Kiến trúc sẵn sàng mở rộng. |
| **REV-05 (Unmerge Tables)** | [PDR-010](./decisions/PDR-010-unmerge-tables.md) | `CONFIRMED` | Cho phép hủy gộp bàn và chọn món phân bổ về bàn tách. Giữ nguyên lịch sử gọi món, ticket bếp, trạng thái chế biến và thanh toán. Không tạo ticket trùng. Hỗ trợ bàn đã có món trước gộp. |
| **REV-06 (VIP/Area Surcharge)** | [PDR-011](./decisions/PDR-011-area-and-vip-room-surcharges.md) | `CONFIRMED` | MVP hỗ trợ phụ thu theo % và phí cố định cho phòng VIP/Khu vực. Chưa làm phí theo giờ. Lưu snapshot phụ thu. Không tự động tính lại món cũ khi chuyển bàn. Thể hiện rõ trên hóa đơn. |
| **REV-07 (Khách Ký Nợ)** | N/A | `DEFERRED` | **Không đưa vào MVP**. Tạm hoãn sang phiên bản sau dưới dạng Module Quản lý công nợ B2B đầy đủ (hạn mức, phê duyệt, đối soát, thu nợ). |
| **REV-08 (Direct Pick Drink)** | [PDR-012](./decisions/PDR-012-direct-service-items-foh.md) | `CONFIRMED` | Cấu hình món/nhóm món phục vụ trực tiếp tại sảnh, bỏ qua KDS Bar. Vẫn ghi nhận đơn hàng, doanh thu và kiểm soát tồn kho. |
| **REV-09 (Bàn trong phòng)** | [PDR-013](./decisions/PDR-013-shared-room-and-entire-room-booking.md) | `CONFIRMED` | Bàn trong phòng mặc định hoạt động độc lập. "Bao trọn phòng" chiếm giữ toàn bộ bàn trong phòng, ngăn khách khác. Khi kết thúc phiên bao trọn phòng, giải phóng bàn theo đúng quy trình chuẩn. |
| **REV-10 (Concurrent Waiters)** | [PDR-014](./decisions/PDR-014-concurrent-order-append-business-rules.md) | `CONFIRMED` | Nghiệp vụ: Nhiều nhân viên thao tác trên cùng bàn không khóa bàn; không ghi đè món; phát hiện xung đột khi sửa/hủy món; chống tạo đơn trùng. Kỹ thuật: Append-Only Delta Firing là đề xuất (`PROPOSED`), giao Solution Architect quyết định cơ chế ở Giai đoạn 2. |
| **REV-11 (Remake Dish)** | [PDR-015](./decisions/PDR-015-remake-dish-workflow.md) | `CONFIRMED` | Làm lại do lỗi bếp không tính thêm tiền cho khách. Có ticket/nhận diện riêng trên KDS. Ghi nhận hao hụt vào Waste Log. Phân biệt với món gọi bổ sung. |
| **CONTR-01 (Đa chi nhánh)** | [PRD.md](./PRD.md) | `CONFIRMED` | Nghiệp vụ MVP phục vụ 01 nhà hàng độc lập tại 01 địa điểm vật lý (theo OD-01). Nền tảng kỹ thuật mở rộng đa chi nhánh được bảo lưu ở mức kiến trúc; không làm tính năng nghiệp vụ chuỗi hay điều chuyển kho trong MVP; giao Solution Architect rà soát tại Stage 2. |
| **CONTR-02 (Thời điểm trừ kho)** | [PDR-004](./decisions/PDR-004-inventory-bom-deduction-trigger-policy.md) | `CONFIRMED` | Chuẩn hóa vòng đời tồn kho 2 bước: Tạm giữ khi xác nhận đơn $\rightarrow$ Tiêu thụ khi chế biến/phục vụ $\rightarrow$ Giải phóng khi hủy hợp lệ $\rightarrow$ Hao hụt khi hủy/hỏng sau chế biến. Chống trừ 2 lần khi retry. MVP quản lý hàng đếm được; BOM sang v1.1. |
