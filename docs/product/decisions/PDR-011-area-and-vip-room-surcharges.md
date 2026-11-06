# Biên Bản Quyết Định Sản Phẩm: PDR-011 — Chính Sách Phụ Phí Theo Khu Vực & Phòng VIP

- **Mã định danh quyết định (Decision ID)**: PDR-011
- **Tiêu đề**: Cấu hình và quy tắc áp dụng phụ phí theo Khu vực ăn uống và Phòng VIP
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Kiến trúc công cụ tính toán: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các phòng ăn VIP riêng tư, sảnh tầng thượng (rooftop) hoặc khu vực sân vườn cảnh quan tại các nhà hàng Việt Nam thường áp dụng chính sách phụ thu dịch vụ đặc biệt hoặc phí thuê phòng riêng. Hệ thống cần có các quy tắc minh bạch về cách thức cấu hình, tính toán và hiển thị các khoản phụ phí này trên hóa đơn của thực khách.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Các Loại Phụ Phí Được Hỗ Trợ Trong Phạm Vi MVP:
- **Phụ phí theo tỷ lệ phần trăm (Percentage Surcharge)**: Một tỷ lệ phần trăm có thể cấu hình (ví dụ: +10% phí phục vụ phòng VIP) tính trên tổng tiền món ăn trước thuế/phí.
- **Phụ phí số tiền cố định (Fixed Fee Surcharge)**: Một số tiền cụ thể cố định (ví dụ: 200.000 ₫ phí sử dụng phòng riêng) cộng trực tiếp vào hóa đơn.
- **Loại trừ phạm vi (Scope Exclusion)**: Tính phí thuê phòng theo giờ (ví dụ: 100.000 ₫/giờ) được loại trừ rõ ràng sang các phiên bản sau và KHÔNG bắt buộc trong MVP.

### 2.2. Phân Cấp Cấu Hình:
- Phụ phí có thể được cấu hình ở cấp độ **Khu vực (Area level)** (ví dụ: Sảnh Sky Lounge) hoặc ở cấp độ **Phòng ăn (Room level)** (ví dụ: Phòng VIP 01, Phòng VIP 02).

### 2.3. Chụp Ảnh Giá Trị Phụ Phí (Snapshot) & Quy Tắc Khi Chuyển Bàn:
- **Ảnh chụp giá trị bất biến (Immutable Snapshot)**: Tỷ lệ hoặc mức phụ phí áp dụng được chụp ảnh cố định (snapshot) vào phiên bàn và đơn hàng ngay khi kích hoạt.
- **Chính sách khi chuyển bàn (Table Transfer Policy)**: Khi khách chuyển từ bàn tiêu chuẩn thông thường vào phòng VIP, hệ thống TUYỆT ĐỐI KHÔNG tự động tính hồi tố hoặc đội phụ phí lên các món ăn đã được bưng phục vụ trước thời điểm chuyển bàn. Phụ phí chỉ áp dụng theo sự lựa chọn và xác nhận rõ ràng của cấp quản lý tại thời điểm chuyển vị trí.
- **Minh bạch trên hóa đơn**: Phụ phí bắt buộc phải được hiển thị thành các dòng riêng biệt, rõ ràng và minh bạch trên cả phiếu tạm tính lẫn hóa đơn giá trị gia tăng / hóa đơn bán lẻ (nghiêm cấm việc ngấm ngầm cộng dồn phụ phí vào giá của từng món ăn).

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Phù hợp hoàn hảo với mô hình kinh doanh ẩm thực tại Việt Nam; tính minh bạch tuyệt đối giúp loại trừ hoàn toàn các tranh chấp thanh toán với khách hàng.
- **Tiêu cực**: Công cụ tính toán hóa đơn của thu ngân phải kết hợp các quy tắc phụ thu khu vực/phòng ăn cùng với thuế VAT và phí dịch vụ thông thường.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-07 (REV-06), phụ phí phần trăm và phụ phí cố định cho phòng VIP/khu vực, cơ chế snapshot phụ phí, và chính sách chuyển bàn không tính hồi tố đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Quy trình đường ống tính toán (calculation pipeline) của công cụ hóa đơn, lược đồ dữ liệu snapshot phụ phí thực thể và cờ cấu hình tính thuế trên phụ phí là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.
