# Biên Bản Quyết Định Sản Phẩm: PDR-007 — Quản Lý Ngày Kinh Doanh Cho Ca Làm Việc Qua Đêm

- **Mã định danh quyết định (Decision ID)**: PDR-007
- **Tiêu đề**: Ứng dụng Ngày Kinh Doanh (Business Date) có thể cấu hình cho các ca làm việc qua đêm và hạch toán tài chính
- **Trạng thái**: `CONFIRMED` (Quy tắc nghiệp vụ đã được Chủ nhà hàng phê duyệt theo D1; Cơ chế kỹ thuật triển khai: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các nhà hàng, quán bar, câu lạc bộ bia và tụ điểm ẩm thực đêm tại Việt Nam thường xuyên hoạt động kéo dài qua nửa đêm (ví dụ: từ 17:00 chiều đến 02:00 hoặc 04:00 sáng hôm sau). Nếu gắn các bản ghi vận hành trực tiếp vào ngày dương lịch tự nhiên (`DateTime.UtcNow` chuyển đổi sang `dd/MM/yyyy`), ca làm việc qua đêm sẽ bị xẻ đôi tại mốc 00:00 nửa đêm, làm phân mảnh doanh thu, doanh số theo nhóm món, việc kiểm kê tiền mù chốt ca của nhân viên và tiêu hao kho thành 2 ngày lịch rời rạc.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Giờ Chốt Ngày Kinh Doanh Cấu Hình Theo Chi Nhánh:
- Mỗi chi nhánh có một thuộc tính giờ chốt ngày có thể cấu hình độc lập: `BusinessDayCutoffTime` (mặc định: `06:00:00` sáng).
- Một chu kỳ vận hành của Ngày Kinh Doanh $T$ kéo dài từ `CutoffTime` của ngày $T$ đến `CutoffTime - 1 giây` của ngày $T+1$ (ví dụ: từ `06:00:00` ngày 10/10/2026 đến `05:59:59` ngày 11/10/2026).

### 2.2. Gắn Cố Định Ngày Kinh Doanh Khi Tạo Đơn Hàng:
- Khi một phiên dùng bữa hoặc một đơn gọi món được khởi tạo, hệ thống tính toán Ngày Kinh Doanh (`BusinessDate`) hiện hành của chi nhánh tại đúng thời điểm đó và gắn cố định bất biến vào bản ghi `Order`.
- **Đơn hàng kéo dài qua mốc chốt ca**: Đơn hàng mở trước giờ chốt ngày (ví dụ: 23:30 đêm) và thanh toán sau giờ chốt ngày (ví dụ: 06:45 sáng) vẫn được ghi nhận chặt chẽ vào đúng `BusinessDate` đã được gán tại thời điểm tạo đơn ban đầu.
- **Phân định ranh giới dấu vết kiểm toán**: Hệ thống phân định rạch ròi giữa ngày phân vùng tài chính (`BusinessDate: DateOnly`) và mốc thời gian UTC vật lý (`CreatedAt: DateTimeOffset`, `PaidAt: DateTimeOffset`).
- **Tính bất biến khi thay đổi cấu hình**: Nếu người quản lý điều chỉnh `BusinessDayCutoffTime`, các đơn hàng đã tạo, hóa đơn đã quyết toán và ca làm việc đã đóng TUYỆT ĐỐI KHÔNG BAO GIỜ bị thay đổi hồi tố `BusinessDate`.

### 2.3. Khớp Nối Ca Làm Việc & Báo Cáo Tài Chính:
- Ca làm việc của nhân viên (`ShiftSession`) kiểm kê tiền két mù dựa trên toàn bộ các giao dịch nằm trong ranh giới vận hành của ca đó.
- Báo cáo tài chính hàng ngày và báo cáo chốt ca Z-Report được tổng hợp thống nhất tuyệt đối theo `BusinessDate`.

---

## 3. Các Tình Huống Vận Hành & Ghi Chú Kiến Trúc
- **Phiên bàn kéo dài nhiều ngày liên tục (> 24 giờ)**: Trong trường hợp hy hữu một phiên bàn vẫn mở qua nhiều lần chốt ngày, bàn ăn đó vẫn giữ nguyên `BusinessDate` ban đầu khi tạo, nhưng hệ thống sẽ phát cảnh báo vận hành để quản lý can thiệp xử lý, tránh để tồn đọng phiên bàn ảo.
- **Tác động kiến trúc**: Các chỉ mục cơ sở dữ liệu trên các bảng `Orders`, `Payments`, và `ShiftSessions` bắt buộc phải bao gồm cặp khóa `(BranchId, BusinessDate)` để tối ưu hóa hiệu năng các truy vấn vận hành hàng ngày.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Báo cáo tài chính chính xác 100% cho các mô hình hoạt động về đêm; chốt két và cân bằng tiền ca làm việc hoàn hảo mà không bị chia cắt bởi ngày dương lịch.
- **Tiêu cực**: Đòi hỏi các câu truy vấn báo cáo phải nhóm dữ liệu theo `BusinessDate` thay vì dùng hàm SQL `CAST(CreatedAt AS DATE)`.
