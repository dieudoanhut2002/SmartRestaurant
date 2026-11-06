# Biên Bản Quyết Định Sản Phẩm: PDR-013 — Quản Lý Phòng Ăn Dùng Chung Đa Bàn & Đặt Bao Trọn Phòng

- **Mã định danh quyết định (Decision ID)**: PDR-013
- **Tiêu đề**: Quản lý vòng đời phòng ăn đa bàn dùng chung và quy trình đặt bao trọn toàn bộ phòng
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Kiến trúc vùng chứa phòng ăn: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các phòng tiệc lớn hoặc sảnh ăn khép kín thường chứa nhiều bàn ăn độc lập. Hệ thống bắt buộc phải hỗ trợ trọn vẹn cả hai kịch bản vận hành thực tế: cho phép các đoàn khách không quen biết ngồi ăn tại các bàn riêng biệt trong cùng một phòng, đồng thời hỗ trợ đặt trước hoặc xếp khách bao trọn toàn bộ phòng cho một bữa tiệc riêng tư duy nhất.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Vận Hành Bàn Độc Lập Mặc Định:
- Các bàn ăn nằm trong cùng một không gian phòng ăn theo mặc định vận hành hoàn toàn độc lập với nhau. Mỗi bàn tự duy trì phiên dùng bữa riêng, danh sách món gọi, các hóa đơn con và các trạng thái vòng đời bàn ăn (`Available`, `Occupied`, `Reserved`).

### 2.2. Đặt Bao Trọn Toàn Bộ Phòng (Bao Trọn Phòng):
- Quản lý nhà hàng hoặc nhân viên đón tiếp có thể thực hiện thao tác **"Bao trọn phòng" (Book Entire Room)** để phục vụ một bữa tiệc hoặc đoàn khách lớn riêng tư.
- **Khóa phòng độc quyền (Exclusive Room Lock)**: Khi kích hoạt, hệ thống lập tức khóa chặn không cho xếp thêm khách vãng lai và chuyển đổi toàn bộ các bàn ăn trong phòng đó sang trạng thái đã đặt/có khách gắn kết với bản ghi đặt phòng tổng thể duy nhất. Không bất kỳ khách vãng lai hay đoàn khách nào khác có thể bị xếp vào bất kỳ bàn nào trong phòng này.

### 2.3. Giải Phóng Phiên Phòng Theo Quy Chuẩn:
- Khi phiên ăn bao trọn phòng kết thúc, từng bàn ăn riêng lẻ bên trong phòng phải được kiểm tra đối chiếu nghĩa vụ và giải phóng nghiêm ngặt theo đúng vòng đời bàn ăn tiêu chuẩn (xác nhận 100% hóa đơn đã quyết toán và toàn bộ món đã được phục vụ) trước khi chuyển trạng thái sang `CleaningNeeded` hoặc `Available`.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Phản ánh chuẩn xác nghiệp vụ quản lý phòng tiệc nhà hàng; tối đa hóa doanh thu nhờ hỗ trợ linh hoạt cả việc ngồi ghép phòng lẫn bao trọn phòng riêng tư.
- **Tiêu cực**: Trình thiết kế sơ đồ mặt bằng và ứng dụng POS phải hiển thị mạch lạc cả ranh giới không gian phòng lẫn trạng thái của từng bàn ăn riêng lẻ bên trong.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-09 (REV-09), việc vận hành bàn độc lập trong phòng dùng chung, khóa bao trọn phòng độc quyền, và xác thực giải phóng bàn theo tiêu chuẩn đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Cấu trúc dữ liệu gom nhóm ranh giới phòng, cơ chế truyền sự kiện trên canvas và máy trạng thái đặt phòng là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.
