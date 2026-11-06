# Biên Bản Quyết Định Sản Phẩm: PDR-006 — Giao Diện Người Dùng Hoàn Toàn Bằng Tiếng Việt (100% Vietnamese Language UI)

- **Mã định danh quyết định (Decision ID)**: PDR-006
- **Tiêu đề**: Tầng trình diễn và toàn bộ giao diện người dùng hiển thị 100% bằng tiếng Việt
- **Trạng thái**: `CONFIRMED` (Được Chủ nhà hàng phê duyệt rõ ràng và bắt buộc thực thi)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Môi trường vận hành mục tiêu của SmartRestaurant chủ yếu là ngành dịch vụ ăn uống (F&B) tại Việt Nam. Nhân viên phục vụ sảnh, phụ bàn, nhân viên pha chế, đầu bếp, thu ngân và quản lý nhà hàng tại Việt Nam đòi hỏi một giao diện thuần Việt tự nhiên nhằm bảo đảm tốc độ thao tác thần tốc, loại bỏ rào cản đào tạo và giao tiếp mạch lạc, không gây hiểu nhầm trong các khung giờ cao điểm căng thẳng.

Chủ nhà hàng đã đưa ra chỉ thị dứt khoát: **"Tôi muốn thực hiện toàn bộ giao diện của dự án này hoàn toàn bằng tiếng Việt"**.

---

## 2. Quyết Định & Ranh Giới Áp Dụng (Decision & Scope of Enforcement)

### 2.1. Phạm vi áp dụng giao diện 100% tiếng Việt:
Toàn bộ các bề mặt tương tác người dùng trên tất cả các ứng dụng máy khách và tầng trình diễn BẮT BUỘC phải hiển thị hoàn toàn bằng tiếng Việt chuẩn mực, tự nhiên và chuyên nghiệp:
1. **Cổng Web Quản Trị & Điều Hành (MVC / Razor)**:
   - Menu điều hướng, cài đặt hệ thống, bảng số liệu tổng quan (dashboard), danh sách nhân sự, báo cáo thống kê, nhật ký kiểm toán.
2. **Trình Thiết Kế Sơ Đồ Mặt Bằng Trực Quan (Konva.js)**:
   - Các công cụ vẽ, tab khu vực, nhãn tên phòng, trạng thái bàn ăn: `Trống` (Available), `Có khách` (Occupied), `Đã đặt` (Reserved), `Đang thanh toán` (BillingRequested), `Cần dọn` (CleaningNeeded), `Ngừng sử dụng` (OutOfService).
3. **Ứng Dụng Tablet POS Của Nhân Viên Phục Vụ**:
   - Danh mục món cảm ứng, tên món, nhóm tùy chọn món kèm theo (ví dụ: "Ít đá", "Không hành", "Cay vừa"), ghi chú món, thao tác giỏ hàng, hộp thoại chuyển bàn / gộp bàn.
4. **Hệ Thống Màn Hình Bếp (KDS) & Quầy Pha Chế**:
   - Tên các bộ phận: "Bếp Nóng", "Bếp Nguội / Salad", "Quầy Pha Chế", "Bếp Bánh / Tráng Miệng", "Bàn Điều Phối Ra Món (Expediter)".
   - Trạng thái dòng món: "Chờ chế biến", "Đang nấu", "Đã xong (Chờ bưng)", "Đã phục vụ", "Đã hủy món".
   - Cảnh báo thời gian: "Quá giờ chế biến (> 20 phút)".
5. **Máy Thu Ngân & Quyết Toán Hóa Đơn**:
   - Xem trước hóa đơn, biểu mẫu chiết khấu/khuyến mãi, màn hình tách hóa đơn ("Tách theo món", "Tách đều"), hộp thoại mở két tiền, quản lý ca làm việc ("Mở ca", "Chốt ca", "Kiểm tiền đầu ca", "Phiếu Z-Report").
6. **Mẫu In Nhiệt Vật Lý**:
   - Phiếu tạm tính ("Phiếu tạm tính"), hóa đơn giá trị gia tăng / phiếu thanh toán ("Hóa đơn thanh toán"), phiếu in báo bếp ("Phiếu báo chế biến").
7. **Phản Hồi Hệ Thống & Cảnh Báo**:
   - Hộp thoại thông báo SweetAlert2, thông điệp lỗi, thông báo xác thực biểu mẫu và chuông báo âm thanh hình ảnh.

### 2.2. Ranh Giới Kỹ Thuật & Mã Nguồn Phần Mềm:
- Các tài liệu kỹ thuật, kiến trúc hệ thống, tên lớp (class) C#, tên cột trong cơ sở dữ liệu và chú thích mã nguồn nội bộ vẫn giữ bằng **tiếng Anh** (theo quy định tại [GEMINI.md](../../../GEMINI.md)) nhằm tuân thủ chuẩn mực kỹ nghệ phần mềm và quy ước mã sạch.
- Toàn bộ chuỗi ký tự hiển thị phía người dùng phải được quản lý tập trung và khoa học trong các tệp tài nguyên bản địa hóa (`.resx`) hoặc từ điển chuỗi giao diện chuyên dụng, nghiêm cấm gán cứng (hardcode) chuỗi giao diện trực tiếp vào logic nghiệp vụ C#.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**:
  - Tối đa hóa tính thân thiện và xóa bỏ hoàn toàn rào cản ngôn ngữ đối với nhân viên phục vụ, đầu bếp và thu ngân tại Việt Nam.
  - Hoàn toàn tương thích với các quy định về chứng từ, hóa đơn tài chính kế toán tại Việt Nam.
  - Chuẩn hóa thuật ngữ ẩm thực và nghiệp vụ nhà hàng bằng tiếng Việt một cách nhất quán.
- **Tiêu cực**:
  - Đòi hỏi sự cẩn trọng cao trong việc quản lý bảng mã ký tự UTF-8 (tiếng Việt có dấu) trên máy in nhiệt, mẫu tin nhắn SMS/thông báo đẩy và hiển thị văn bản trên canvas.

---

## 4. Xác Nhận Phê Duyệt (Confirmation)
- **Trạng thái**: `CONFIRMED` vào ngày 2026-11-02 bởi Chủ nhà hàng (Project Owner).
