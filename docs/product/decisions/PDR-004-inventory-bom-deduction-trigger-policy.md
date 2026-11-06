# Biên Bản Quyết Định Sản Phẩm: PDR-004 — Khóa Tồn Kho 2 Bước & Chính Sách Khấu Trừ BOM Phân Kỳ

- **Mã định danh quyết định (Decision ID)**: PDR-004
- **Tiêu đề**: Khóa tồn kho 2 bước (Two-Phase Inventory Lock) và lộ trình triển khai định lượng nguyên vật liệu (BOM) theo phân kỳ
- **Trạng thái**: `CONFIRMED` (Quy tắc nghiệp vụ & Phạm vi đã được Chủ nhà hàng phê duyệt theo D2/D5; Cơ chế kỹ thuật triển khai: `PROPOSED`)
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Trong kinh doanh dịch vụ ăn uống, luôn tồn tại một khoảng trễ thời gian giữa lúc nhận đơn gọi món, nhà bếp chế biến, bưng phục vụ tại bàn và thời điểm thanh toán hóa đơn. Nếu chỉ trừ tồn kho vào lúc thanh toán sẽ dẫn đến việc bán vượt quá số lượng tồn (overselling) trong giờ cao điểm đối với các món ăn có số lượng giới hạn (ví dụ: vịt quay nguyên con, bít tết bò Wagyu, bia nhập khẩu). Ngược lại, nếu trừ thẳng tồn kho thực tế ngay khi nhận món mà không qua cơ chế tạm giữ sẽ làm lệch dữ liệu kho khi bếp không thể phục vụ hoặc khách hủy món hợp lệ.

---

## 2. Quyết Định & Quy Tắc Vận Hành (Được Chủ Nhà Hàng Phê Duyệt)

### 2.1. Phân định Ranh giới Phạm vi theo Phân kỳ (Phased Scope Boundary):
1. **Phiên bản v1.0 (Phạm vi MVP)**:
   - Quản lý theo dõi **Hàng hóa bán trực tiếp / Hàng đếm được (Direct Saleable / Countable Items)**: Đồ uống đóng gói (bia lon/chai, nước ngọt, rượu vang), tráng miệng đóng hộp sẵn và các món ăn chế biến theo khẩu phần nguyên chiếc/đĩa đếm được.
   - Trì hoãn việc bóc tách định lượng nguyên liệu chi tiết dạng gram/ml (BOM: gram thịt bò, ml nước mắm) sang **Phiên bản v1.1**.
   - Kiến trúc module bắt buộc phải công bố các giao diện nghiệp vụ tường minh (`IInventoryDeductionEngine`) để sẵn sàng kích hoạt tính năng định lượng BOM chuyên sâu trong v1.1 theo cơ chế cắm-chạy (plug-and-play) mà không gây xáo trộn kiến trúc.

### 2.2. Chuyển Đổi Trạng Thái Tồn Kho Chuẩn & Công Thức Toán Học:
$$\text{AvailableStock (Tồn khả dụng)} = \text{PhysicalStock (Tồn thực tế)} - \text{ReservedStock (Tồn tạm giữ)}$$

1. **Bước 1 — Tạm Giữ (Reserve khi Xác Nhận Đơn Hàng / Báo Bếp)**:
   - Khi một dòng món được xác nhận hoặc nhấn báo bếp xuống quầy bếp/bar, số lượng yêu cầu lập tức được phân bổ vào `ReservedStock`.
   - Khi `AvailableStock <= 0`, hệ thống kích hoạt ngay cảnh báo hết hàng và khóa món ăn không cho tiếp tục gọi thêm trên toàn bộ máy POS và máy tính bảng của nhân viên phục vụ.
2. **Bước 2 — Tiêu Thụ Thực Tế (Consume khi Chế Biến / Phục Vụ)**:
   - Khi món ăn được đánh dấu là đã chế biến hoặc đã phục vụ (theo chính sách từng trạm), số lượng tạm giữ được chuyển đổi vĩnh viễn thành tiêu thụ thực tế `ConsumedStock`:
     $$\text{PhysicalStock} \leftarrow \text{PhysicalStock} - \text{Quantity}$$
     $$\text{ReservedStock} \leftarrow \text{ReservedStock} - \text{Quantity}$$
3. **Bước 3 — Hủy Món Hợp Lệ (Legitimate Cancellation)**:
   - Khi món bị hủy trước khi bắt đầu chế biến: Hệ thống giải phóng `ReservedStock` hoàn trả lại vào `AvailableStock`. Không ghi nhận hao hụt/hủy hỏng.
4. **Bước 4 — Hao Hụt / Hủy Bỏ (Waste & Spoilage)**:
   - Khi món bị hủy sau khi bếp đã bắt đầu chế biến (cháy khét, rơi đổ, khách bỏ về): `ReservedStock` chuyển đổi thành `WasteStock` và ghi nhận vào bảng theo dõi hủy hỏng `WasteLog` kèm mã lý do bắt buộc và sự phê duyệt của Quản lý bằng mã PIN.

> [!NOTE] Phân Định Ranh Giới Về Thời Điểm Kích Hoạt Khấu Trừ Kho Thực Tế (F-04 Resolution)
> - **Quy tắc bất biến đã duyệt**: Căn cứ quyết định của Chủ Dự Án `D2` và `D5`, công thức $\text{Tồn khả dụng} = \text{Tồn thực tế} - \text{Tạm giữ}$ và phạm vi quản lý món đếm được trong MVP là bất biến đã được xác nhận.
> - **Chủ đề kỹ thuật mở**: Thời điểm kích hoạt chuyển từ `Reserved` sang `Consumed` và chính sách trừ tồn kho thực tế (khi KDS báo hoàn thành, khi phục vụ bưng món, hay khi thanh toán) tiếp tục được duy trì ở trạng thái **OPEN / TECHNICAL INVESTIGATION** cho Giai đoạn 2. Không tự ý ấn định sự kiện trigger nào là mặc định.
> - **Món DirectFOHPick**: Được ghi nhận đầy đủ trong luồng đơn hàng, doanh thu, tồn kho và thanh toán, nhưng không dùng quy định này để suy diễn trigger trừ tồn kho.

### 2.3. Hàng Rào Bảo Vệ Bất Đồng Bộ & Tính Bất Khả Biến (Concurrency & Idempotency):
- **Ngăn chặn tồn kho âm (Preventing Negative Stock)**: Kiểm soát tranh chấp (concurrency control) phải đảm bảo các đơn đặt món song song cho những suất cuối cùng không thể làm cho tồn khả dụng bị âm.
- **Thử lại an toàn (Idempotent Retry)**: Việc mạng bị thử lại hoặc bấm gửi đơn gọi món nhiều lần tuyệt đối không được gây ra việc tạm giữ tồn kho trùng lặp hay trừ kho hai lần.

---

## 3. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Xóa bỏ hoàn toàn tình trạng bán vượt số lượng tồn; vết kiểm toán mạch lạc giữa tồn thực tế, tồn tạm giữ, tiêu thụ thực và hao hụt; giữ cho tiến độ triển khai MVP nhanh chóng và khả thi.
- **Tiêu cực**: Báo cáo giá vốn hàng bán (COGS) bóc tách đến từng gram nguyên liệu thô được dời sang v1.1.
