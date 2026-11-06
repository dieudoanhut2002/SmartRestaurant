# Biên Bản Quyết Định Sản Phẩm: PDR-015 — Quy Trình Làm Lại Món Do Lỗi Bếp & Chế Biến

- **Mã định danh quyết định (Decision ID)**: PDR-015
- **Tiêu đề**: Quy trình làm lại món (Remake Dish) và chính sách ghi nhận sổ hủy hỏng do lỗi chế biến và phục vụ
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Trình bày giao diện KDS: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Khi một món ăn đã nấu xong bị từ chối tại bàn ăn do lỗi của nhà bếp (ví dụ: bít tết cháy khét, canh quá mặn, sơ suất để dính dị ứng của khách, hoặc làm nhầm món ăn kèm), các quy trình POS thông thường ép nhân viên phải hủy món cũ rồi tạo đơn gọi món mới sẽ gây ra sai sót hóa đơn (khách bị tính tiền 2 lần) hoặc làm sai lệch báo cáo kiểm kê kho bếp.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Bảo Vệ Tuyệt Đối Hóa Đơn Khách Hàng:
- Thao tác làm lại món do lỗi của bếp TUYỆT ĐỐI KHÔNG BAO GIỜ tính thêm tiền vào hóa đơn của thực khách. Số lượng dòng món và tổng tiền phải trả trên hóa đơn của khách được giữ nguyên không đổi.

### 2.2. Nhận Diện Thẻ Làm Lại Chuyên Biệt Trên Màn Hình KDS:
- Lệnh làm lại món sẽ kích hoạt một phiếu chế biến ưu tiên khẩn cấp trên màn hình KDS của trạm chế biến tương ứng kèm nhận diện trực quan nổi bật:
  - Nhãn hiển thị: **"LÀM LẠI DO LỖI BẾP"** (Remake - Kitchen Error).
  - Hiển thị rõ lý do cụ thể (ví dụ: Quá mặn, Khét, Dị vật, Nhầm món).

### 2.3. Ghi Nhận Sổ Hủy Hỏng & Tiêu Hao Kho Thực Tế:
- Hệ thống tự động ghi nhận phần món ăn bị loại bỏ vào **Sổ Hủy Hỏng (Waste Log / ScrapLoss)** để theo dõi tiêu hao nguyên vật liệu và nâng cao trách nhiệm của bộ phận bếp.
- Việc tạm giữ/tiêu hao kho được ghi nhận cho phần ăn làm lại thay thế nhằm đảm bảo số liệu tồn kho thực tế luôn chuẩn xác.

### 2.4. Vết Kiểm Toán & Phân Biệt Tuyệt Đối Với Gọi Món Mới:
- Hệ thống ghi lại đầy đủ: Người yêu cầu, Quản lý phê duyệt (mã PIN), Mốc thời gian, Phân loại lỗi và Bộ phận bếp chịu trách nhiệm.
- Thao tác làm lại món được phân biệt rạch ròi với hành vi khách tự gọi thêm món mới hoặc gọi suất bổ sung.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Vận hành bếp minh bạch; bảo vệ tính chính xác của hóa đơn khách hàng; cung cấp số liệu thực tế để kiểm soát chất lượng ẩm thực của nhà bếp.
- **Tiêu cực**: Giao diện POS phục vụ phải trang bị thao tác nhập lệnh làm lại có kiểm soát, bắt buộc phải chọn lý do và cần quản lý phê duyệt.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-12 (REV-11), không tính thêm tiền cho khách, điều phối phiếu làm lại ưu tiên cao, ghi nhận hao hụt nội bộ (`ScrapLoss`) với sự phê duyệt của quản lý, và phân biệt vết kiểm toán với đơn gọi thêm đã ở trạng thái **CONFIRMED**.
> - **Đặc tả giao diện đề xuất**: Kiểu dáng hiển thị trực quan cụ thể (ví dụ: phong cách thẻ màu tím, viền nhấp nháy, tần số âm thanh chuông báo) là **đề xuất giao diện của AI (AI UI proposal)**, chờ hoàn thiện trong giai đoạn thiết kế UI/UX ở Stage 2.
