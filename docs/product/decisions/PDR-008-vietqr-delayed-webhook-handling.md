# Biên Bản Quyết Định Sản Phẩm: PDR-008 — Xử Lý Webhook VietQR Đến Muộn & Sổ Treo Đối Soát

- **Mã định danh quyết định (Decision ID)**: PDR-008
- **Tiêu đề**: Quy trình xử lý webhook VietQR đến muộn sau khi hóa đơn đã được quyết toán bằng Tiền mặt
- **Trạng thái**: `CONFIRMED` (Quy tắc nghiệp vụ đã được Chủ nhà hàng phê duyệt theo D3; Cơ chế kỹ thuật triển khai: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Trong các khung giờ cao điểm thanh toán, độ trễ từ cổng chuyển mạch ngân hàng (Napas 24/7) hoặc mạng viễn thông của khách hàng có thể làm chậm trễ các webhook xác nhận thanh toán VietQR. Nếu khách hàng không muốn chờ đợi và chuyển sang trả tiền mặt rồi rời đi, việc webhook của ngân hàng gửi thông báo thành công sau đó vài phút sẽ tạo ra xung đột doanh thu đa phương thức rất phức tạp.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Dự Phòng Bằng Tiền Mặt Khi Quá Hạn Chờ VietQR (Cash Fallback):
- Nếu yêu cầu thanh toán VietQR chưa nhận được xác nhận sau khoảng thời gian hết hạn (timeout threshold) đã cấu hình, thu ngân được phép thu tiền mặt và đóng hóa đơn.
- Yêu cầu thanh toán QR đang chờ sẽ được chuyển trạng thái thành `CancelledByCashier / CashFallback`.

### 2.2. Điều Chuyển Vào Sổ Treo Đối Soát Khi Webhook Đến Muộn:
- Khi webhook ngân hàng gửi xác nhận đã nhận được tiền khớp với mã thanh toán QR đã bị hủy hoặc hóa đơn đã quyết toán tiền mặt, giao dịch này lập tức được **chuyển vào "Giao dịch treo đối soát" (Suspense Reconciliation Queue)**.
- **Cảnh báo âm thanh & hình ảnh tức thì**: Hệ thống lập tức phát cảnh báo mức độ ưu tiên cao trên màn hình của Thu ngân và Quản lý.

### 2.3. Kiểm Soát Chống Trùng Lặp & Xác Thực Chặt Chẽ:
- **Khớp mã giao dịch ngân hàng & tham chiếu**: Mọi webhook gửi đến phải được kiểm tra tính hợp lệ bằng mã giao dịch duy nhất của ngân hàng (`BankTransactionId`) và mã tham chiếu nội bộ (`PaymentRef`) để chống ghi nhận trùng.
- **Nghiêm cấm ghi có hai lần tự động**: Hệ thống TUYỆT ĐỐI KHÔNG tự động cộng thêm tiền vào hóa đơn đã được đóng bằng tiền mặt.
- **Nghiêm cấm tự động hoàn tiền khi chưa xác minh**: Hệ thống TUYỆT ĐỐI KHÔNG tự động kích hoạt lệnh hoàn tiền ngân hàng chỉ dựa trên dữ liệu webhook mà chưa qua kiểm tra thực tế.

### 2.4. Quy Trình Xử Lý Thủ Công Kèm Nhật Ký Kiểm Toán Toàn Vẹn:
- Quản lý có thẩm quyền rà soát các giao dịch treo trong sổ đối soát với 3 hướng xử lý có kiểm soát:
  1. **Khớp thủ công (Manual Match / Gán vào hóa đơn)**: Gán số tiền vào một hóa đơn chưa thanh toán hoặc thanh toán thiếu nếu khách đã chuyển nhầm.
  2. **Ghi nhận tiền chưa phân bổ (Unallocated Revenue)**: Lưu giữ trong sổ quỹ treo chờ khách hàng liên hệ khiếu nại.
  3. **Hoàn tiền được phê duyệt (Approved Cash/Bank Refund)**: Thực hiện hoàn trả tiền mặt hoặc chuyển khoản theo quy trình có chữ ký duyệt và giải trình nguyên nhân bắt buộc của Quản lý.
- Mọi gói tin webhook, các lần thử khớp và thao tác đối soát đều phải được ghi lại vĩnh viễn trong bảng nhật ký kiểm toán bất biến.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Xóa bỏ hoàn toàn sai lệch tiền trong két; ngăn chặn thất thoát tài chính và nộp thuế trùng lặp; bảo đảm tính minh bạch trong kiểm toán tài chính.
- **Tiêu cực**: Đòi hỏi nhân viên thu ngân và quản lý phải được đào tạo bài bản về quy trình đối soát sổ quỹ treo.
