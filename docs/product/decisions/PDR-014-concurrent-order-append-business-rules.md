# Biên Bản Quyết Định Sản Phẩm: PDR-014 — Quy Tắc Nghiệp Vụ Gọi Món Đồng Thời Của Nhân Viên Phục Vụ

- **Mã định danh quyết định (Decision ID)**: PDR-014
- **Tiêu đề**: Quy tắc nghiệp vụ cho các thao tác gọi món đồng thời của nhiều nhân viên phục vụ trên cùng một phiên bàn
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Cơ chế kỹ thuật triển khai: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Trong giờ cao điểm, nhiều nhân viên phục vụ thường cùng lúc chăm sóc các vị khách khác nhau tại một bàn tiệc lớn đông người. Việc khóa cứng bàn ăn độc quyền (Pessimistic Table Lock) sẽ làm tê liệt vận hành, trong khi cơ chế kiểm soát tranh chấp lạc quan (optimistic concurrency) đơn giản lại tiềm ẩn nguy cơ ghi đè thay đổi giỏ hàng hoặc làm mất các món ăn vừa được nhập khi nhiều nhân viên cùng gửi đơn gần như đồng thời.

---

## 2. Quy Tắc Nghiệp Vụ Đã Phê Duyệt (Chủ Nhà Hàng Xác Nhận)

### 2.1. Truy Cập Bàn Không Khóa Chặn (Non-Blocking Shared Table Access):
- Hệ thống cho phép nhiều nhân viên phục vụ cùng lúc mở và tạo đơn gọi món nháp cho cùng một bàn ăn vật lý mà không kích hoạt cơ chế khóa chặn giao diện (UI lockouts).

### 2.2. Chống Thất Thoát Dữ Liệu Tuyệt Đối (Data Loss Prevention):
- Thao tác gửi đơn gọi món của một nhân viên phục vụ TUYỆT ĐỐI KHÔNG BAO GIỜ được ghi đè, xóa bỏ hoặc làm mất các dòng món ăn được gửi đồng thời bởi một nhân viên phục vụ khác.

### 2.3. Phát Hiện Xung Đột Khi Sửa / Hủy Món (Conflict Detection):
- Nếu Nhân viên A thao tác hủy hoặc chỉnh sửa một dòng món ăn cụ thể trong khi Nhân viên B cũng đang đồng thời chỉnh sửa chính dòng món đó, hệ thống phải phát hiện xung đột, ngăn chặn việc sai lệch dữ liệu cục bộ và hiển thị thông báo rõ ràng phản ánh đúng trạng thái thực tế mới nhất.

### 2.4. Tính Bất Khả Biến & Chống Trùng Lặp (Idempotency & Duplicate Prevention):
- Hệ thống phải trang bị các chốt bảo vệ an toàn thử lại bất khả biến ở cả phía máy khách và máy chủ (ví dụ: khóa tính bất khả biến idempotency key khi gửi đơn) để ngăn chặn việc tạo phiếu bếp trùng lặp hoặc nhân đôi dòng món do nhân viên bấm liên tiếp nhiều lần (double clicking) hoặc do mạng thử lại tự động.

---

## 3. Trạng Thái Triển Khai Kỹ Thuật (Phạm Vi Kiến Trúc Giải Pháp Stage 2)
- **Trạng thái**: `PROPOSED`
- **Chỉ thị kiến trúc**: Mô hình "Gửi Delta Chỉ Ghi Thêm" (Append-Only Delta Firing) từng được gợi ý trước đây là một đề xuất về mặt kiến trúc, không phải là một giải pháp kỹ thuật bị áp đặt bắt buộc. **Kiến trúc sư giải pháp (Solution Architect)** trong Giai đoạn 2 (Stage 2) có trách nhiệm phân tích và lựa chọn cơ chế kỹ thuật tối ưu nhất (ví dụ: event sourcing delta append, hàng đợi lệnh mức dòng nguyên tử, hoặc khóa lạc quan phân tán) để hiện thực hóa trọn vẹn các quy tắc nghiệp vụ nêu trên.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Năng suất phục vụ cao và trải nghiệm người dùng mượt mà cho nhân viên trong giờ cao điểm; loại bỏ hoàn toàn tình trạng mất đơn món.
- **Tiêu cực**: Kiến trúc kỹ thuật phải thiết kế cơ chế xử lý xung đột đồng thời và tính bất khả biến thật sự vững chắc.
