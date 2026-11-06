# Biên Bản Quyết Định Sản Phẩm: PDR-001 — Mô Hình Triển Khai Kiến Trúc Modular Monolith

- **Mã định danh quyết định (Decision ID)**: PDR-001
- **Tiêu đề**: Mô hình triển khai Modular Monolith với khả năng mở rộng đa chi nhánh
- **Trạng thái**: `CONFIRMED` (Đã được phê duyệt rõ ràng trong các chỉ thị nền tảng của dự án)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ (Senior Product Architect & BA)

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
SmartRestaurant được định hướng phục vụ đa dạng các quy mô kinh doanh ẩm thực—từ các quán cà phê, nhà hàng bistro độc lập đơn lẻ cho đến các chuỗi nhà hàng ẩm thực lớn nhiều địa điểm chi nhánh. Chúng ta cần một mô hình kiến trúc và triển khai vừa mang lại sự tinh gọn, đơn giản trong vận hành cục bộ cho các nhà hàng độc lập hiện nay, vừa mở ra con đường nâng cấp liền mạch, không gián đoạn để tiến tới vận hành quản lý tập trung đa chi nhánh trong tương lai.

---

## 2. Các Phương Án Được Đánh Giá (Options Considered)

### Phương án A: Kiến trúc Microservices phân tán (Distributed Microservices Architecture)
- Tách rời thành các dịch vụ vi mô độc lập: Thực đơn (Menu), Quản lý bàn (Tables), Gọi món (Orders), Màn hình bếp (KDS), Hóa đơn (Billing), và Quản lý kho (Inventory) với hệ thống hàng đợi thông điệp bất đồng bộ (RabbitMQ/Kafka) cùng các cơ sở dữ liệu phân tán.
- *Ưu điểm*: Khả năng mở rộng độc lập cho các dịch vụ có lưu lượng truy cập cao (ví dụ: máy POS bán hàng so với hệ thống báo cáo).
- *Nhược điểm*: Chi phí hạ tầng khổng lồ, quản trị các giao dịch phân tán vô cùng phức tạp (Saga choreography), độ trễ mạng phát sinh giữa khu vực sảnh (FOH) và khu vực bếp (BOH), chi phí vận hành quá tốn kém đối với nhà hàng quy mô vừa và nhỏ, tiềm ẩn rủi ro lỗi ghi kép (dual-write).

### Phương án B: Ứng dụng Monolith nguyên khối ghép nối chặt (Tightly-Coupled Monolith)
- Một mã nguồn duy nhất nơi mà các bảng dữ liệu bàn ăn, đơn món và hóa đơn thanh toán thoải mái truy vấn chéo (cross-join) lẫn nhau mà không có bất kỳ ranh giới đóng gói bảo vệ nào.
- *Ưu điểm*: Tốc độ phát triển ban đầu rất nhanh.
- *Nhược điểm*: Nhanh chóng biến thành nợ kỹ thuật (technical debt) không thể bảo trì chỉ sau vài tháng; không thể cô lập dữ liệu đa chi nhánh; rủi ro lỗi lan truyền dây chuyền cực cao mỗi khi tái cấu trúc mã nguồn.

### Phương án C: Kiến trúc Modular Monolith phân vùng Chi nhánh/Tenant từ đầu (Được khuyến nghị)
- Một ứng dụng triển khai duy nhất nhưng được cấu trúc chặt chẽ thành các module nghiệp vụ gắn kết cao với ranh giới nghiêm ngặt, sử dụng sự kiện miền nội bộ tiến trình (in-process domain events) và các hợp đồng giao tiếp dịch vụ tường minh.
- Mô hình dữ liệu bắt buộc cài đặt định danh `OrganizationId` và `BranchId` trên tất cả các thực thể giao dịch ngay từ ngày đầu tiên.
- *Ưu điểm*: Triệt tiêu độ trễ mạng trong các thao tác vận hành nội bộ FOH/BOH, triển khai cài đặt cực kỳ thuận tiện, độ tin cậy tuyệt đối nhờ giao dịch cơ sở dữ liệu đơn nhất (ACID), module hóa ranh giới nghiệp vụ mạch lạc, dễ dàng tách thành các microservices độc lập trong tương lai nếu quy mô đòi hỏi.
- *Nhược điểm*: Đòi hỏi kỷ luật kỹ thuật khắt khe nhằm ngăn chặn lập trình viên đi đường tắt vượt qua giao diện công khai (public interface) của module.

---

## 3. Khuyến Nghị & Quyết Định (Recommendation & Decision)
**Lựa chọn Phương án C (Modular Monolith phân vùng Chi nhánh/Tenant từ đầu)**.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Dung lượng hạ tầng triển khai gọn nhẹ cho các khách hàng đơn điểm; không vướng phải cạm bẫy của giao dịch phân tán; tốc độ phản hồi tính bằng mili-giây trên toàn bộ mạng cục bộ nhà hàng.
- **Tiêu cực**: Giao diện module phải được giám sát nghiêm ngặt để chống phụ thuộc vòng (circular dependencies) và ngăn chặn truy cập trái phép vào kho dữ liệu (repository) giữa các module.

---

## 5. Câu Hỏi & Giả Định (Questions & Assumptions)
- *Giả định*: Đa số các đợt triển khai ban đầu sẽ dành cho các cơ sở nhà hàng độc lập đơn điểm trước khi bước sang giai đoạn cấp phép chuỗi nhượng quyền đa chi nhánh.
