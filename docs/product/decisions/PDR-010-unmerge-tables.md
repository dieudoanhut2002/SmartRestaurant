# Biên Bản Quyết Định Sản Phẩm: PDR-010 — Chính Sách Hủy Gộp Bàn & Tái Phân Bổ Món Ăn

- **Mã định danh quyết định (Decision ID)**: PDR-010
- **Tiêu đề**: Chính sách hủy gộp bàn đối với các bàn đã ghép trước đó kèm tái phân bổ món ăn
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Cơ chế kỹ thuật triển khai: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Khi hai hay nhiều bàn ăn đã được gộp lại thành một phiên dùng bữa chung duy nhất (ví dụ: Bàn 04 và Bàn 05), các tình huống vận hành thực tế thường xuyên phát sinh nhu cầu hủy gộp chúng (ví dụ: nhân viên gộp nhầm bàn do sơ suất, hoặc một nhóm khách ngồi tại Bàn 05 quyết định về sớm hơn và muốn tách ra thanh toán riêng).

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Khả Năng Hủy Gộp Bàn (Unmerge Capability):
- Hệ thống hỗ trợ tính năng hủy gộp các bàn đã ghép trở lại thành các phiên bàn vật lý độc lập ở bất kỳ giai đoạn nào của ca phục vụ.

### 2.2. Phân Bổ Món & Tính Toàn Vẹn Của Phiếu Bếp:
- **Bảo toàn lịch sử trước đó**: Khi thực hiện hủy gộp, thu ngân hoặc quản lý có thể tích chọn những món ăn nào thuộc về bàn được tách ra (ví dụ: Bàn 05).
- **Không sinh phiếu bếp trùng lặp**: Hành động hủy gộp bàn TUYỆT ĐỐI KHÔNG BAO GIỜ sinh lại phiếu bếp hoặc gửi lệnh chế biến trùng lặp xuống màn hình KDS đối với các món đã được báo bếp từ trước.
- **Bảo toàn trạng thái**: Toàn bộ trạng thái chế biến hiện có của món ăn (`Cooking`, `ReadyToServe`, `Served`), trạm bếp chịu trách nhiệm và lịch sử mốc thời gian đều được bảo toàn nguyên vẹn.
- **Xử lý các món đã có từ trước**: Nếu Bàn 05 đã có các món ăn đang hoạt động từ trước khi gộp vào Bàn 04, các món gốc và phiếu bếp tương ứng phải được phân bổ chính xác trở lại phiên bàn được khôi phục của Bàn 05.

### 2.3. Vòng Đời Bàn Ăn Độc Lập:
- Sau khi hủy gộp, Bàn 05 trở thành một phiên bàn (`TableSession`) độc lập hoàn toàn, có thể gọi thêm món mới, in phiếu tạm tính, thanh toán hóa đơn và chuyển sang trạng thái `CleaningNeeded` mà không làm ảnh hưởng đến Bàn 04.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Độ linh hoạt vận hành rất cao cho các bữa tiệc và đoàn khách đông; loại bỏ hoàn toàn các thao tác xử lý thủ công vòng vèo làm sai lệch số liệu doanh thu.
- **Tiêu cực**: Đòi hỏi giao diện thu ngân phải hỗ trợ thao tác kéo thả hoặc danh sách chọn trực quan để tái phân bổ các dòng món giữa các bàn được hủy gộp.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-06 (REV-05), việc hủy gộp bàn ở bất kỳ giai đoạn phục vụ nào, bảo toàn trạng thái chế biến món ăn, không sinh phiếu bếp trùng lặp, và khôi phục phiên bàn độc lập đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Lược đồ thực thể tái phân bổ cụ thể và cơ chế giao diện kéo thả là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.
