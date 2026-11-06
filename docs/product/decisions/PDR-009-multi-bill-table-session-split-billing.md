# Biên Bản Quyết Định Sản Phẩm: PDR-009 — Phiên Bàn Đa Hóa Đơn & Chính Sách Tách Hóa Đơn Linh Hoạt

- **Mã định danh quyết định (Decision ID)**: PDR-009
- **Tiêu đề**: Mô hình đa hóa đơn trên một phiên bàn và chính sách tách hóa đơn linh hoạt (Không khóa bàn ăn)
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Cơ chế kỹ thuật cơ sở dữ liệu & xử lý đồng thời: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Khi các nhóm khách đề nghị chia hóa đơn thanh toán, một số người muốn thanh toán phần của mình để rời đi trước trong khi những người còn lại vẫn ngồi lại và tiếp tục gọi thêm đồ ăn, thức uống. Một đề xuất trước đây từng gợi ý khóa cứng (freeze) toàn bộ bàn ăn trong quá trình tách hóa đơn, nhưng phương án này đã bị bác bỏ hoàn toàn vì nó làm tê liệt quy trình phục vụ khách và làm giảm hiệu suất quay vòng bàn ăn.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Mô hình Phiên Bàn Đa Hóa Đơn (Multi-Bill Table Session):
- Một phiên dùng bữa đang hoạt động (`TableSession`) có thể chứa nhiều hóa đơn con độc lập (`SubBill` / `Invoice`).
- Thao tác tách hóa đơn sẽ phân bổ các dòng món ăn hiện có giữa các hóa đơn con mà không hề khóa bàn ăn.

### 2.2. Gọi Món Không Gián Đoạn, Tính Khả Biên Tập Của `BillingRequested`, và Gắn Món Mặc Định:
- **Không khóa bàn (No Table Freeze)**: Bàn ăn đang trong trạng thái đưa phiếu tạm tính (`BillingRequested`) hoặc đang tách hóa đơn TUYỆT ĐỐI KHÔNG BAO GIỜ bị khóa cứng.
- **Hóa đơn chưa thanh toán được phép chỉnh sửa**: Khi bàn ăn ở trạng thái `BillingRequested`, hóa đơn chưa thanh toán vẫn hoàn toàn có thể chỉnh sửa được. Nhân viên có thể thêm, bớt hoặc điều chỉnh món, tuân thủ theo đúng phân quyền và nhật ký kiểm toán.
- **Gắn món mặc định so với tạo hóa đơn con mới**: Khi khách gọi thêm món sau khi phiếu tạm tính đã được in:
  - **Hành vi mặc định**: Các món gọi thêm mới được tự động gắn vào hóa đơn mở chưa thanh toán hiện có.
  - **Chủ động tạo hóa đơn con**: Nhân viên phục vụ có thể chủ động chọn điều phối các món mới vào một hóa đơn con vừa tạo mới (ví dụ: Hóa đơn con 2) khi khách yêu cầu tính tiền riêng đợt này.
- **Nhiều hóa đơn con đồng thời**: Nhiều hóa đơn con có thể cùng tồn tại song song trong một phiên bàn và được thanh toán độc lập với nhau.

### 2.3. Quyết Toán Độc Lập Cho Từng Hóa Đơn Con:
- Mỗi hóa đơn con có thể được thanh toán độc lập bằng phương thức thanh toán riêng (Tiền mặt, Thẻ, VietQR).
- Khi thanh toán xong Hóa đơn con 1, trạng thái của nó chuyển sang `Paid` và in biên lai riêng. Bàn ăn vật lý vẫn tiếp tục duy trì trạng thái `Occupied` hoặc `BillingRequested` chừng nào vẫn còn hóa đơn con chưa thanh toán.

### 2.4. Hủy Thao Tác Tách Hóa Đơn:
- Thu ngân chỉ được phép hủy thao tác tách hóa đơn và gộp toàn bộ các món trở lại thành một hóa đơn duy nhất **nếu chưa có bất kỳ hóa đơn con nào trong phiên đó phát sinh giao dịch thanh toán**.
- Một khi bất kỳ hóa đơn con nào đã được thanh toán một phần hoặc toàn phần, cấu trúc tách hóa đơn sẽ bị khóa, không được phép hoàn tác (rollback) hàng loạt.

### 2.5. Điều Kiện Giải Phóng Bàn (Không Còn Nghĩa Vụ Tồn Đọng):
Một phiên bàn ăn CHỈ đủ điều kiện để giải phóng và chuyển sang trạng thái `CleaningNeeded` hoặc `Available` khi:
1. Tất cả các hóa đơn con trong phiên bàn đều đã đạt trạng thái `Paid` (hoặc đã `Cancelled` hợp lệ).
2. Tất cả các món ăn đã gọi đều đã đạt trạng thái chế biến và phục vụ cuối cùng (`Served` hoặc đã hủy kèm ghi nhận hao hụt kiểm toán).
3. Không còn bất kỳ nghĩa vụ ẩm thực hay tài chính nào còn mở trên phiên bàn.

### 2.6. Nghiêm Cấm Sửa Trực Tiếp Hóa Đơn Đã Thanh Toán:
- **Tính bất biến của hóa đơn đã thanh toán**: Tuyệt đối trong mọi trường hợp không cho phép trực tiếp chỉnh sửa hoặc thay đổi các dòng món trên những hóa đơn đã đạt trạng thái `Paid`.
- Một khi hóa đơn/hóa đơn con đã được thanh toán, nó trở thành bất biến về mặt tài chính kế toán. Mọi thao tác điều chỉnh hoặc hoàn tiền sau thanh toán phải tuân theo quy trình hủy/hoàn tiền chính thức do cấp quản lý phê duyệt.

### 2.7. Nhật Ký Kiểm Toán (Audit Logging):
- Hệ thống ghi lại toàn bộ lịch sử phân bổ món giữa các hóa đơn con, các chỉnh sửa diễn ra trong trạng thái `BillingRequested`, thao tác điều chuyển của thu ngân, sự kiện thanh toán và bất kỳ hành động hủy tách hóa đơn nào.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Trải nghiệm thực khách mượt mà; khách ngồi lại có thể tiếp tục gọi món mà không bị cản trở; thanh toán chặt chẽ tuyệt đối về mặt toán học.
- **Tiêu cực**: Mô hình dữ liệu bắt buộc phải hỗ trợ quan hệ 1-Nhiều (1-to-N) giữa `TableSession` và `Invoices`.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-13 (D4 / REV-04), phiên bàn đa hóa đơn, không khóa bàn khi xem tạm tính hoặc tách bill, hóa đơn chưa thanh toán vẫn có thể chỉnh sửa trong trạng thái `BillingRequested`, món gọi thêm mặc định gắn vào hóa đơn mở hoặc tạo hóa đơn con mới, tính bất biến của hóa đơn đã thanh toán, và điều kiện giải phóng bàn khi không còn nghĩa vụ tồn đọng đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Cơ chế cô lập giao dịch hóa đơn con, token kiểm soát tranh chấp đồng thời (concurrency tokens) trong cơ sở dữ liệu và màn hình quản lý đa hóa đơn của thu ngân là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.
