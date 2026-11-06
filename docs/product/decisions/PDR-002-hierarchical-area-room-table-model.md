# Biên Bản Quyết Định Sản Phẩm: PDR-002 — Mô Hình Không Gian Phân Cấp Khu Vực, Phòng Và Bàn Ăn

- **Mã định danh quyết định (Decision ID)**: PDR-002
- **Tiêu đề**: Mô hình không gian phân cấp Khu vực, Phòng ăn và Bàn ăn (Area, Room, Table)
- **Trạng thái**: PROPOSED
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các nhà hàng sở hữu bố cục không gian vật lý rất phong phú và đa dạng. Một số địa điểm sử dụng sảnh ăn mở chung, trong khi những nơi khác lại có các phòng ăn riêng biệt, khu sân vườn ngoài trời và các phòng tiệc VIP sang trọng (VIP 1, VIP 2, VIP 3). Hơn thế nữa, thực khách có thể ngồi tại một bàn đơn, nhiều bàn ghép lại với nhau, bao trọn cả một phòng ăn hoặc đặt riêng một phòng VIP. Chúng ta cần một mô hình miền không gian đủ linh hoạt để đáp ứng trọn vẹn mọi phương thức sắp xếp vận hành thực tế này.

---

## 2. Các Phương Án Được Đánh Giá (Options Considered)

### Phương án A: Danh sách bàn phẳng không phân cấp (Flat Table List)
- Các bàn ăn chỉ đơn thuần được đặt tên "Bàn 1", "Bàn 2", "VIP 1" trong một danh sách phẳng duy nhất mà không có bất kỳ cấu trúc phân cấp không gian nào.
- *Ưu điểm*: Lược đồ cơ sở dữ liệu cực kỳ đơn giản.
- *Nhược điểm*: Không thể nhóm bàn theo tầng hoặc khu vực; không thể dựng sơ đồ mặt bằng trực quan đa khu vực; không hỗ trợ tính năng đặt trước toàn bộ phòng hoặc thực hiện thao tác đổi trạng thái hàng loạt theo khu vực.

### Phương án B: Mô hình phân cấp (Khu vực $\rightarrow$ Phòng $\rightarrow$ Bàn) kèm liên kết phiên đa bàn (Được khuyến nghị)
- Thiết lập cấu trúc phân cấp tường minh:
  - Một Chi nhánh (`Branch`) bao gồm một hoặc nhiều Khu vực ăn uống (`DiningArea`, ví dụ: Tầng 1 Trong nhà, Sân vườn).
  - Một Khu vực ăn uống (`DiningArea`) chứa từ 0 đến nhiều Phòng ăn (`Room`, bao gồm cả các phòng VIP) và từ 0 đến nhiều Bàn độc lập (`Table`).
  - Một Phòng ăn (`Room`) chứa một hoặc nhiều Bàn ăn (`Table`).
  - Một Phiên dùng bữa (`DiningSession`) có thể liên kết với một bàn chính, nhiều bàn ghép lại với nhau (gộp bàn), hoặc liên kết bao trọn toàn bộ phòng ăn.
- *Ưu điểm*: Mô hình hóa hoàn hảo thực tế vận hành; cho phép hiển thị các khung vẽ sơ đồ 2D độc lập theo từng khu vực; hỗ trợ tính năng bao trọn phòng; hỗ trợ phân công nhân viên phục vụ trực quan theo từng khu vực.
- *Nhược điểm*: Các truy vấn quan hệ cơ sở dữ liệu và thanh điều hướng giao diện người dùng phức tạp hơn đôi chút.

---

## 3. Khuyến Nghị & Quyết Định (Recommendation & Decision)
**Lựa chọn Phương án B (Mô hình phân cấp Khu vực $\rightarrow$ Phòng $\rightarrow$ Bàn)**.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng REV-06 (Phụ phí theo Khu vực/Phòng VIP) và REV-09 (Vòng đời bàn độc lập trong phòng ăn dùng chung và khái niệm bao trọn phòng), các yêu cầu nghiệp vụ về cấu trúc phân cấp đa khu vực, gom nhóm phòng và vận hành phòng VIP đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Cấu trúc phân cấp thực thể cụ thể (`Branch` $\rightarrow$ `DiningArea` $\rightarrow$ `Room` $\rightarrow$ `Table`), các khóa quan hệ cơ sở dữ liệu và hệ trục tọa độ khung vẽ trực quan là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Trình thiết kế sơ đồ mặt bằng dễ dàng hiển thị các tab/khu vực riêng biệt; quản lý có thể bao trọn phòng VIP chỉ với một thao tác; nhân viên sảnh định vị bàn ăn trực quan và nhanh chóng.
- **Tiêu cực**: Giao diện POS phục vụ cần có cơ chế chuyển đổi khu vực mượt mà, tránh gây thao tác bấm quá nhiều bước trong giờ cao điểm.

---

## 5. Câu Hỏi & Giả Định (Questions & Assumptions)
- *Giả định*: Mỗi bàn ăn thuộc về duy nhất một Khu vực hoặc một Phòng; một bàn không thể cùng lúc nằm trôi nổi ở nhiều khu vực khác nhau.
