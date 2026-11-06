# Biên Bản Quyết Định Sản Phẩm: PDR-012 — Mặt Hàng Phục Vụ Trực Tiếp Tại Sảnh

- **Mã định danh quyết định (Decision ID)**: PDR-012
- **Tiêu đề**: Chính sách điều phối đối với các mặt hàng phục vụ trực tiếp tại sảnh bỏ qua màn hình bếp (KDS)
- **Trạng thái**: `CONFIRMED` (Yêu cầu nghiệp vụ đã được phê duyệt; Cơ chế kỹ thuật công cụ điều phối: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Các loại đồ uống đóng chai/lon ướp lạnh (bia lon, nước suối đóng chai, nước ngọt) hoặc các mặt hàng đóng gói sẵn mang đi thường được lưu trữ trực tiếp trong tủ lạnh tại sảnh ăn và được nhân viên phục vụ sảnh (FOH) lấy ra phục vụ khách ngay lập tức. Việc điều phối các mặt hàng đơn giản này xuống màn hình KDS Quầy Bar hay Bếp sẽ làm tắc nghẽn màn hình của nhân viên chế biến bằng các phiếu không cần thiết và làm chậm tốc độ phục vụ.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Cấu Hình Phục Vụ Trực Tiếp (Direct Service Configuration):
- Các món ăn hoặc danh mục món trong thực đơn có thể được cấu hình thuộc tính tuyến phục vụ: `ServiceRouting = DirectFOHPick` (Phục vụ trực tiếp tại sảnh) so với `StationRouting` (Gửi trạm chế biến Bếp/Bar KDS).

### 2.2. Hàng Rào Vận Hành & Bảo Vệ Tài Chính:
- **Không gửi phiếu xuống KDS**: Khi nhân viên báo bếp một đơn gọi món có chứa mặt hàng phục vụ trực tiếp, các mặt hàng này sẽ bỏ qua hoàn toàn hàng đợi chế biến của KDS Quầy Bar.
- **Chuyển đổi trạng thái tức thì**: Các mặt hàng phục vụ trực tiếp lập tức chuyển sang trạng thái `ReadyToServe / Dispensed` ngay khi xác nhận đơn. Nếu có cấu hình máy in tại sảnh, một phiếu lấy hàng nhỏ có thể in tại trạm phục vụ sảnh cục bộ.
- **Bảo đảm hạch toán tài chính & trừ kho nghiêm ngặt**: Việc bỏ qua khâu chế biến trên KDS TUYỆT ĐỐI KHÔNG làm bỏ qua khâu tính tiền hay kiểm soát kho hàng. Các mặt hàng phục vụ trực tiếp vẫn được tính đủ trong hóa đơn khách hàng, báo cáo doanh thu, và tuân thủ chặt chẽ các quy tắc tạm giữ/khấu trừ tồn kho tiêu chuẩn.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Giải phóng màn hình KDS Quầy Bar khỏi sự quá tải; đẩy nhanh tốc độ phục vụ đồ uống cho khách trong giờ cao điểm.
- **Tiêu cực**: Đòi hỏi nhân viên phục vụ phải có ý thức tự giác lấy đồ uống kịp thời cho khách mà không phụ thuộc vào chuông báo hoàn thành của KDS.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Căn cứ theo quyết định của Chủ nhà hàng OD-08 (REV-08), việc phục vụ trực tiếp tại sảnh đối với đồ uống đóng lon/chai bỏ qua KDS Quầy Bar trong khi vẫn bảo đảm đầy đủ tính tiền và trừ kho đã ở trạng thái **CONFIRMED**.
> - **Đề xuất kỹ thuật**: Lược đồ kiểu liệt kê (enum) phân tuyến cụ thể, tích hợp bộ đệm máy in sảnh và giao diện điều phối trạm là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.
