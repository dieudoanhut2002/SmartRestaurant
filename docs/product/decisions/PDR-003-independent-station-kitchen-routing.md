# Biên Bản Quyết Định Sản Phẩm: PDR-003 — Điều Phối Bếp Đa Trạm Độc Lập & Đồng Bộ Màn Hình Điều Phối Ra Món

- **Mã định danh quyết định (Decision ID)**: PDR-003
- **Tiêu đề**: Điều phối món theo trạm chế biến bếp động kèm đồng bộ màn hình điều phối ra món tổng thể (Master Expediter)
- **Trạng thái**: PROPOSED
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các nhà hàng vận hành nhiều bộ phận bếp chuyên biệt: Bếp Nóng (nướng, xào, chiên), Bếp Nguội (salad, khai vị sống), Quầy Pha Chế (cocktail, bia, nước ngọt), và Bếp Bánh / Tráng miệng. Một đơn gọi món của khách thường chứa các món ăn được điều phối đến nhiều trạm khác nhau. Chúng ta cần một cơ chế kiến trúc để phân tuyến món ăn chính xác mà không gán cứng (hardcode) tên các trạm bếp vào mã nguồn phần mềm.

---

## 2. Các Phương Án Được Đánh Giá (Options Considered)

### Phương án A: Cố định cứng các trạm bếp trong mã nguồn (Hardcoded Fixed Stations)
- Hệ thống cố định sẵn 4 trạm: "Bếp Nóng", "Quầy Bar", "Bếp Nướng", "Bếp Bánh".
- *Ưu điểm*: Đơn giản khi viết mã ban đầu.
- *Nhược điểm*: Không thể đáp ứng phong cách vận hành đa dạng của các nhà hàng khác nhau (ví dụ: nhà hàng Á có trạm Nướng than hoa + trạm Há cảo/Dimsum + trạm Sơ chế Lẩu so với một nhà hàng Ý có Lò nướng Pizza củi + trạm Mỳ Ý).

### Phương án B: Một màn hình bếp duy nhất hiển thị tất cả (Monolithic Single-Screen Display)
- Mọi bộ phận bếp dùng chung một màn hình KDS khổng lồ hiển thị toàn bộ món của tất cả các đơn hàng.
- *Ưu điểm*: Không cần logic phân tuyến điều phối.
- *Nhược điểm*: Gây hỗn loạn thị giác tột độ trong giờ cao điểm; nhân viên pha chế phải nhìn thấy nồi lẩu; đầu bếp nướng phải nhìn thấy chai rượu; gây quá tải nhận thức không thể chấp nhận được.

### Phương án C: Công cụ điều phối trạm cấu hình linh hoạt kèm màn hình điều phối tổng thể Master Expediter (Được khuyến nghị)
- Quản lý nhà hàng tự định nghĩa các bản ghi Trạm Chế Biến (`KitchenStation`) tùy biến theo từng chi nhánh.
- Mỗi món ăn (hoặc biến thể món) trong thực đơn được gán tới một `KitchenStation` mục tiêu.
- Khi đơn gọi món được báo bếp, công cụ điều phối sẽ tự động phân tách đơn thành các phiếu chế biến riêng biệt gửi đến màn hình KDS của từng bộ phận nấu.
- Một **Màn hình Điều Phối Ra Món Tổng Thể (Master Expediter Display)** đặt tại bàn ra món hiển thị phiếu bàn tổng hợp cùng nhãn trạng thái thời gian thực của từng món con, giúp trưởng bàn ra món điều phối bưng món đồng bộ ra bàn khách.
- *Ưu điểm*: Linh hoạt vận hành tối đa; co giãn tương thích với mọi mô hình ẩm thực hay bố cục bếp; ngăn chặn tình trạng món nguội ngắt chờ đợi tại bàn ra món.
- *Nhược điểm*: Cần quản lý nhiều thiết bị đầu cuối hiển thị và đồng bộ trạng thái thời gian thực giữa các trạm.

---

## 3. Khuyến Nghị & Quyết Định (Recommendation & Decision)
**Lựa chọn Phương án C (Điều phối trạm cấu hình linh hoạt kèm Master Expediter)**.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Các yêu cầu vận hành cụ thể đã được xác nhận theo quyết định của Chủ nhà hàng: REV-08 (Mặt hàng phục vụ trực tiếp tại sảnh bỏ qua KDS Quầy Bar) và REV-11 (Quy trình làm lại món do lỗi bếp và ghi nhận sổ hủy hỏng).
> - **Đề xuất kỹ thuật**: Cả REV-08 lẫn REV-11 đều không đồng nghĩa với việc phê duyệt toàn bộ cơ chế Điều Phối Bếp Động hay Màn hình Master Expediter. Tổng thể công cụ phân tách điều phối trạm, cơ chế hiển thị Master Expediter và lược đồ dữ liệu vẫn thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Linh hoạt hoàn toàn cho mọi mô hình kinh doanh ẩm thực; đầu bếp chỉ thấy những gì họ cần chế biến; nhân viên tiếp thực nhận thông báo rõ ràng khi toàn bộ bàn đã sẵn sàng ra món.
- **Tiêu cực**: Làm tăng thêm các bước cấu hình trong giai đoạn khởi tạo chi nhánh mới.

---

## 5. Câu Hỏi & Giả Định (Questions & Assumptions)
- *Giả định*: Mỗi chi nhánh sẽ có tối thiểu 2 màn hình hiển thị trạm (ví dụ: 1 KDS Bếp Đồ Ăn + 1 KDS Quầy Bar) và 1 màn hình Điều Phối Ra Món Tổng Thể (Expediter).
