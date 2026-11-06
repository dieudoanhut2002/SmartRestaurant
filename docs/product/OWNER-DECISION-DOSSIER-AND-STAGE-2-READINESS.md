# Bộ Hồ Sơ Quyết Định Của Chủ Dự Án & Báo Cáo Đánh Giá Sẵn Sàng Sang Giai Đoạn 2
## (Owner Decision Register & Stage 2 Solution Architecture Readiness Assessment)

- **Mã Nhiệm Vụ**: TASK 02.8.2
- **Dự Án**: SmartRestaurant — Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Giai Đoạn**: Stage 1 — Khám Phá Sản Phẩm & Đặc Tả Miền Nghiệp Vụ (Product Discovery & Business Domain Specifications)
- **Vai Trò Đảm Nhiệm**: Giám Đốc Kiểm Toán Quản Trị Sản Phẩm & Trưởng Ban Kiến Trúc Hệ Thống (Senior Product Governance Auditor & Lead Solution Architect)
- **Trạng Thái Kiểm Toán**: `READY FOR PROJECT OWNER DECISION`
- **Ngày Hoàn Tất**: 2026-11-05
- **Tài Liệu Căn Cứ & Đối Soát**:
  - Quyết định Gốc của Chủ Dự Án: `D1–D5`, `REV-05` đến `REV-11`, `BillingRequested Option 2`.
  - Quyết định Sản phẩm & Kiến trúc: `OD-01` đến `OD-13`, `PDR-001` đến `PDR-015`.
  - Báo cáo Kiểm toán Quản trị Task 02.7.5: [`FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md`](./FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md).
  - Đặc tả Quản lý Phân hệ Task 02.8: [`MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md`](./MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md).
  - Báo cáo Tái Đối soát Phân hệ Task 02.8.1: [`MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md`](./MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md).
  - Quy chuẩn Quản trị Dự án: [`GEMINI.md`](../../GEMINI.md) & [`AGENTS.md`](../../AGENTS.md).

---

## MỤC LỤC TỔNG QUAN

1. **Deliverable 1: Thẩm Định Tính Toàn Vẹn Quy Tắc Kho D2/D5 (D2/D5 Inventory Integrity Verification)**
2. **Deliverable 2: Sổ Đăng Ký Quyết Định Của Chủ Dự Án (Owner Decision Register)**
3. **Deliverable 3: Bảng So Sánh Chi Tiết Phương Án OD-14 (OD-14 Paid Bill Correction Comparison)**
4. **Deliverable 4: Tóm Tắt Quyết Định OD-15, OD-16, OD-17 (OD-15/16/17 Decision Briefs)**
5. **Deliverable 5: Ma Trận Truy Vết & Đánh Giá Tắc Nghẽn Câu Hỏi Mở (Open Question Traceability Matrix)**
6. **Deliverable 6: Đánh Giá Mức Độ Sẵn Sàng Bước Vào Giai Đoạn 2 (Stage 2 Readiness Assessment)**
7. **Deliverable 7: Báo Cáo Rủi Ro Còn Lại & Khuyến Nghị Chuyên Môn (Remaining Risks & Recommendations)**
8. **Deliverable 8: Báo Cáo Kiểm Tra Cây Làm Việc & Thay Đổi Git (Git Change & Provenance Report)**
9. **Trạng Thái Cổng Kiểm Soát Cuối Cùng (Final Status Declaration)**

---

## 1. Deliverable 1: Thẩm Định Tính Toàn Vẹn Quy Tắc Kho D2/D5 (D2/D5 Inventory Integrity Verification)

### 1.1. Xác nhận Tuân thủ Tuyệt đối các Quyết định Đã Chốt của Chủ Dự án (D2/D5)
Hệ thống tài liệu Giai đoạn 1 bảo toàn 100% các nguyên tắc kho bất biến đã được phê duyệt:
1. **Công thức Tồn kho Khả dụng**:
   $$\text{Tồn kho Khả dụng (Available Stock)} = \text{Tồn kho Vật lý (Physical Stock)} - \text{Tồn kho Đã Giữ trước (Reserved Stock)}$$
2. **Thời điểm Kích hoạt Giữ kho (Reservation Point)**:
   - Việc giữ kho xảy ra chính xác tại thời điểm **Xác nhận đơn hàng / Bắn đơn xuống bếp (`OrderConfirmed` / `FiredToKitchen`)**.
   - Tuyệt đối không giữ kho khi nhân viên mới chỉ thêm món vào giỏ hàng nháp (`DraftOrder`) để tránh khóa ảo tài nguyên.
3. **Giải phóng Giữ kho khi Hủy Đơn Hợp lệ**:
   - Khi món ăn bị hủy trước khi bếp tiếp nhận chế biến (`Status = Confirmed/Queued`), lượng hàng đã giữ trước được hoàn trả ngay lập tức: `ReservedStock` giảm, `AvailableStock` tăng tương ứng.
4. **Phân định Rõ ràng 3 Khái niệm Tồn kho**:
   - `ReservedStock` (Đã giữ trước), `ConsumedStock` (Đã tiêu hao), và `WasteStock` (Hao phí/Hủy món) là 3 khái niệm kế toán độc lập, không bị gộp chung.
5. **Phạm vi Kho trong MVP**:
   - MVP chỉ quản lý tồn kho cho **Hàng hóa Thành phẩm Đếm được (Countable Finished Goods)** như lon bia, chai nước ngọt, bánh tráng miệng nguyên chiếc.
   - Định mức BOM nguyên vật liệu chi tiết (gram thịt, ml dầu ăn, gia vị) tiếp tục **hoãn sang Phiên bản v1.1**.

### 1.2. Bóc tách Tường minh 5 Khái niệm Nghiệp vụ Vận hành Kho & Bếp
Để loại bỏ hoàn toàn việc suy diễn sai lệch điểm kích hoạt trừ kho trong tương lai, bảng sau đây phân định rạch ròi 5 khái niệm:

| Khái Niệm | Bản Chất Nghiệp Vụ | Thực Thể Dữ Liệu Tác Động | Thời Điểm Phát Sinh | Tình Trạng Quản Trị |
| :--- | :--- | :--- | :--- | :---: |
| **1. Reservation (Giữ trước)** | Khóa logic số lượng khả dụng để ngăn chặn bán vượt số lượng tồn trên các POS khác. | `InventoryItem.ReservedQuantity += Qty`<br>`Available = Physical - Reserved` | Bắn đơn xuống bếp (`OrderFired`) | `OWNER-CONFIRMED (D2/D5)` |
| **2. Physical Consumption (Tiêu hao thực tế)** | Việc sử dụng thực tế nguyên liệu/hàng hóa trong quầy pha chế hoặc bếp. | Hàng hóa bị bóc mở, chế biến tại trạm. | Khi đầu bếp lấy đồ ra chế biến | Khái niệm vận hành thực tế |
| **3. Waste Recording (Ghi nhận Hao phí)** | Ghi sổ tổn thất khi món đã nấu nhưng bị đổ, cháy, hoặc khách hủy sau khi bếp đã nấu. | Ghi sổ hao hụt vào `WasteLog` khi làm lại món do lỗi bếp theo REV-11 / PDR-015 | Khi hủy/làm lại món sau chế biến | `OWNER-CONFIRMED (PDR-004/015)` |
| **4. Inventory Deduction (Khấu trừ sổ kho)** | Bút toán kế toán chuyển đổi từ `ReservedStock` sang `ConsumedStock` và giảm `PhysicalStock`. | `PhysicalQuantity -= Qty`<br>`ReservedQuantity -= Qty` | Chưa định nghĩa trong Stage 1; Solution Architect sẽ đánh giá và thiết kế trong Stage 2 | `AI/BA PROPOSAL / TECHNICAL DESIGN REQUIRED — STAGE 2` |
| **5. KDS Status Transitions (Chuyển trạng thái KDS)** | Tiến trình hiển thị thị giác trên màn hình bếp (`Chờ nấu` $\rightarrow$ `Đang nấu` $\rightarrow$ `Chờ cung ứng` $\rightarrow$ `Đã phục vụ`). | `OrderItem.KitchenStatus`<br>(Chỉ phục vụ điều phối nhịp độ bếp) | Đầu bếp chạm vào màn hình KDS | `AI/BA PROPOSAL (Vận hành thị giác)` |

> [!IMPORTANT]
> **Xác nhận Ranh giới Quản trị về Khấu trừ Kho (COR-02)**:  
> Quyết định D2/D5 của Chủ Dự án chỉ xác nhận công thức Tồn kho Khả dụng, thời điểm tạm giữ (Reservation) khi gửi bếp, và giải phóng khi hủy đơn hợp lệ. Chủ Dự án **KHÔNG** ấn định thời điểm chuyển đổi từ Reserved sang Consumed, không mặc định KDS `ReadyToServe` là sự kiện trừ kho, và không tự quyết định việc Bar trừ kho khi in phiếu hay Bếp Nóng trừ kho khi hoàn thành KDS. Toàn bộ thời điểm và cơ chế kích hoạt trừ kho thực tế được phân loại chính xác là `AI/BA PROPOSAL / TECHNICAL DESIGN REQUIRED — STAGE 2` và thuộc thẩm quyền đánh giá của Solution Architect trong Stage 2.

### 1.3. Cơ chế "Báo Hết Món (86 / Out-of-Stock)" Thủ công khi Phân hệ Kho (`MOD-12`) bị Tắt
- **Hành vi Vận hành**: Khi phân hệ `MOD-12` (Quản lý Tồn kho Tự động) bị tắt, hệ thống không chạy thuật toán tính `Available = Physical - Reserved`. Để ngăn chặn khách gọi món đã hết nguyên liệu, Quản lý hoặc Thu ngân sử dụng tính năng **Báo Hết Món Thủ Công (Manual 86-ing)**:
  - Quản lý/Thu ngân gạt công tắc `IsAvailable = false` ("Tạm ngưng phục vụ / Hết món") trên danh mục món ăn (`MOD-02`).
  - Giao diện POS phục vụ ngay lập tức làm mờ món ăn và vô hiệu hóa nút thêm vào giỏ hàng.
- **Phân loại Quản trị**:
  - Tính năng thuộc tính `MenuItem.IsAvailable` trong Danh mục Thực đơn (`MOD-02`): **`OWNER-CONFIRMED`**.
  - Luồng đồng bộ thời gian thực báo hết món thủ công qua SignalR khi phân hệ `MOD-12` bị tắt: Phân loại chính xác là **`AI/BA PROPOSAL`** (Đang chờ Chủ dự án phê duyệt cùng Khung phân hệ).

---

## 2. Deliverable 2: Sổ Đăng Ký Quyết Định Của Chủ Dự Án (Owner Decision Register)

Bảng phân loại độc lập tách biệt rõ ràng giữa các Quyết định Đã Chốt và các Đề xuất Mở đang chờ phê duyệt:

| Mã Quyết Định | Tên Vấn Đề Nghiệp Vụ | Căn Cứ Thẩm Quyền | Phân Loại Quản Trị | Tóm Tắt Tùy Chọn & Khuyến Nghị Chuyên Môn |
| :---: | :--- | :---: | :---: | :--- |
| **`D1`** | **Ranh giới Ngày Kinh Doanh & Giờ Cắt Ca** | Owner Sign-off | `OWNER-CONFIRMED` | Mặc định Cutoff 06:00 sáng. Phiên bàn mở giữ nguyên Ngày Kinh Doanh (`BusinessDate`) tại thời điểm mở xuyên suốt ca đêm. (Lưu ý: Việc xử lý phiên bàn kéo dài > 24h thuộc OQ-07 đã được Chủ Dự Án phê duyệt theo Phương án A: Giữ `BusinessDate` gốc + Banner cảnh báo Quản lý). |
| **`D2/D5`** | **Khóa Kho Hai Pha & Hàng Đếm Được** | Owner Sign-off | `OWNER-CONFIRMED` | Khóa tại thời điểm gửi bếp; hủy món hợp lệ mở khóa; BOM chi tiết gram/ml hoãn sang v1.1. |
| **`D3`** | **Xử Lý VietQR Đến Chậm & Hàng Đợi Tiền Treo (Suspense Ledger)** | Owner Sign-off (PDR-008) | `OWNER-CONFIRMED` | Cho phép thu tiền mặt khi QR timeout. Giao dịch QR về muộn đưa vào Hàng đợi Tiền treo; thông báo tức thì cho thu ngân; đối soát thủ công có phê duyệt. |
| **`D4`** | **Phiên Bàn Đa Hóa Đơn (Multi-Bill) & Tách Bill Không Khóa Bàn** | Owner Sign-off (PDR-009) | `OWNER-CONFIRMED` | Bàn không bị khóa khi tách bill. Phục vụ tiếp tục gọi món vào bill chưa thanh toán. Hỗ trợ tách theo món & tách đều $1/N$; thanh toán Multi-Tender. |
| **`OD-01`** | **Ranh giới Cửa hàng Đơn lập cho MVP** | Owner Sign-off | `OWNER-CONFIRMED` | 01 nhà hàng độc lập tại 01 địa điểm duy nhất. Không có logistics điều chuyển chuỗi trong MVP. |
| **`OD-13`** | **Tính Bất Biến của Hóa Đơn Đã Thanh Toán** | Owner Sign-off | `OWNER-CONFIRMED` | Hóa đơn đã thanh toán (`IsSettled = true`) không thể bị sửa đổi trực tiếp bởi Thu ngân/Phục vụ. |
| **`OD-14`** | **Chính sách Xử lý Khiếu nại & Hủy Hóa Đơn Đã Thanh Toán** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Giữ DB bất biến (`IsSettled = true`), xử lý chênh lệch thực tế qua biên bản giấy khi chốt két ca & ghi log đối soát (`ShiftClosingVarianceLog`). Hoãn chức năng CreditNote trong app sang v1.1. |
| **`OD-15`** | **Phân vị Phân hệ Đặt Bàn Trước (`MOD-06`) trong MVP** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Phân hệ Tùy chọn MVP (`MVP Optional`) — có thể bật/tắt linh hoạt 1 chạm qua cờ tính năng; loose coupling với thực thể `Reservation`. |
| **`OD-16`** | **Áp dụng Bộ Gói Cấu Hình Khởi Tạo (Onboarding Presets)** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Áp dụng 3 Gói gợi ý mẫu Wizard (Gia đình, Tiêu chuẩn, VIP) không ràng buộc trong Wizard Onboarding. |
| **`OD-17`** | **Quy Tắc Thoát Nghiệp Vụ Phân Tầng (Graceful Drain Policy)** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Thoát phân tầng theo loại giao dịch dở dang (bàn ăn, đặt cọc, VietQR); dữ liệu lịch sử bảo lưu Read-Only. |
| **`OQ-01`** | **Hoãn Tách Hóa Đơn theo Tỷ Lệ % Tùy Biến** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Hoãn chia % lẻ sang v1.1; MVP hỗ trợ Tách theo món chọn lọc & Tách đều $1/N$. |
| **`OQ-02`** | **Cấu Hình Tự Động In Nhiệt Bếp khi Bắn Đơn** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Cấu hình theo trạm, mặc định `AutoPrintOnFire: false` (Ưu tiên KDS màn hình; in nhiệt làm fallback). |
| **`OQ-06`** | **Quy Tắc Tính Thuế VAT trên Phí Dịch Vụ** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Cấu hình chi nhánh, mặc định `IsServiceChargeTaxable: true` (VAT tính trên Tiền món + Phí dịch vụ). |
| **`OQ-07`** | **Kiểm Soát Phiên Bàn Kéo Dài > 24 Giờ Vượt Mốc Cắt Ca** | Task 02.8.5 Workshop | `OWNER-CONFIRMED (Phương án A)` | **Phương án A**: Giữ Ngày kinh doanh gốc khi mở bàn + Hiển thị dải băng cảnh báo màu cam cho Quản lý. |
| **Khung Phân Hệ** | **Cơ Cấu 10 Core - 4 Optional Modules** | Task 02.8.5 Workshop | `OWNER-CONFIRMED` | Phê chuẩn chính thức: 10 Phân hệ Cốt lõi Bắt buộc; 4 Phân hệ Tùy chọn (MOD-06, MOD-09, MOD-11, MOD-12). |

---

## 3. Deliverable 3: Bảng So Sánh Chi Tiết Phương Án OD-14 (OD-14 Paid Bill Correction Comparison)

> [!CAUTION]
> **Ranh giới Bất biến Được Bảo toàn**: Cả hai phương án đều tôn trọng nguyên tắc: Thu ngân không thể bấm sửa trực tiếp trên hóa đơn đã thanh toán.  
> **Tuyên bố Quản trị**: Báo cáo này **KHÔNG** tự ý phê duyệt bất kỳ phương án nào thay cho Chủ Dự án.

| Tiêu Chí Đánh Giá | Phương Án A: Xử Lý Ngoại Suy Bằng Thủ Tục Giấy Cho MVP (Manual Off-System Handling) | Phương Án B: Quy Trình Hủy Hóa Đơn & Bút Toán Âm Trong App (In-App Void & Credit Note Workflow) |
| :--- | :--- | :--- |
| **1. Tác Động Nghiệp Vụ (Business Impact)** | - Thu ngân báo Quản lý xử lý ngay tại chỗ.<br>- Quản lý xuất tiền mặt hoàn cho khách (nếu có) và lập biên bản giấy 2 liên có chữ ký khách.<br>- Khách hàng rời đi nhanh chóng, không bị chậm trễ do lỗi hệ thống mạng. | - Quản lý đăng nhập vào POS bằng mã PIN quản lý, chọn chức năng "Hủy/Điều chỉnh Thanh toán".<br>- Hệ thống in ra "Phiếu Hủy Hóa Đơn" điện tử và phiếu hoàn tiền.<br>- Trải nghiệm số hóa chuyên nghiệp, hiện đại đối với khách hàng. |
| **2. Hàm Ý Kiểm Toán Tài Chính (Financial Audit Implications)** | - Hóa đơn trong CSDL giữ nguyên 100% doanh thu ban đầu.<br>- Báo cáo POS hiển thị doanh thu thô (Gross). Khoản chênh lệch tiền mặt bắt buộc phải được đối soát trên "Biên bản Kiểm đếm Tiền mặt Cuối ca" (Blind Shift Close Variance) và có khả năng tham chiếu tới hóa đơn gốc bị điều chỉnh.<br>- **Yêu cầu nghiệp vụ bắt buộc cần Stage 2 đánh giá**: Cần cơ chế lưu vết ghi nhận chênh lệch giữa tiền thực tế và số liệu hệ thống, ghi nhận lý do, người thực hiện, thời điểm và người xác minh, có khả năng tham chiếu hóa đơn gốc nếu có liên quan, bảo đảm audit trail và khả năng đối soát kế toán (không bắt buộc tên bảng hay khóa ngoại cụ thể, giao Solution Architect thiết kế).<br>- Triệt tiêu nguy cơ nhân viên gian lận thông đồng xóa bill trên máy. | - Hệ thống tự động sinh thực thể `CreditNote` (Bút toán âm) triệt tiêu doanh thu tương ứng.<br>- Báo cáo chốt ca (Z-Report) phản ánh trực tiếp doanh thu thuần tự động.<br>- Cần cơ chế kiểm soát nội bộ nghiêm ngặt: Nếu Quản lý lộ mã PIN, có nguy cơ nhân viên thông đồng hủy hóa đơn để biển thủ tiền mặt. |
| **3. Độ Phức Tạp Vận Hành (Operational Complexity)** | - Quy trình nghiệp vụ kết hợp giữa phần mềm và thủ tục hành chính giấy tờ.<br>- Đòi hỏi Quản lý phải ghi chép biên bản giấy, nhập mã sai lệch khi chốt ca và bàn giao đối chiếu với kế toán mỗi sáng. | - Đòi hỏi đào tạo nhân viên và Quản lý về máy trạng thái hủy, lý do hủy bắt buộc (Reason Codes), và quy trình bàn giao két tự động. |
| **4. Tác Động Triển Khai MVP (MVP Implementation Impact)** | - **Cần Stage 2 đánh giá cấu trúc dữ liệu**: Không tạo máy trạng thái đảo ngược hóa đơn phức tạp, nhưng **vẫn cần thiết kế cấu trúc dữ liệu lưu vết chênh lệch và thông tin đối soát trong Stage 2/3** để đảm bảo khả năng đối soát và audit trail. Loại bỏ nhận định không có căn cứ cho rằng Phương án A "không phát sinh chi phí", "không tác động ERD" hay "không cần lưu dữ liệu". | - **Tăng thêm 7 - 10 ngày công**: Thiết kế kiến trúc Stage 2 và triển khai Stage 3/5: Tạo thực thể điều chỉnh/hủy, bút toán âm, hoàn tiền từng phần, cập nhật lại trạng thái bàn/kho, và kiểm thử hồi quy. |
| **5. Hàm Ý Chuyển Đổi Tương Lai (Future Migration Implications)** | - **Cực kỳ thuận lợi**: Vì CSDL MVP duy trì tính bất biến, việc bổ sung bảng `CreditNotes` ở Phiên bản v1.1 là thao tác mở rộng (Additive Change), không phá vỡ dữ liệu lịch sử của MVP. | - Dữ liệu đã số hóa đầy đủ ngay từ đầu, không cần quy trình đối soát dữ liệu lịch sử khi nâng cấp. |

---

## 4. Deliverable 4: Tóm Tắt Quyết Định OD-15, OD-16, OD-17 (OD-15/16/17 Decision Briefs)

### 4.1. `OD-15`: Định vị Phân hệ Đặt Bàn Trước (`MOD-06`) trong Phạm vi MVP
- **Lựa chọn 1: Đưa vào MVP dưới dạng Phân hệ Tùy chọn (`MVP Optional`)**
  - *Hệ quả*: Sản phẩm hỗ trợ đầy đủ luồng đặt bàn, ghi nhận tiền cọc và lịch hẹn; nhà hàng bình dân không có nhu cầu có thể tắt trong 1 chạm để giao diện tinh gọn. Kiến trúc Stage 2 sẽ thiết kế thực thể `Reservation` tách rời độc lập (loose coupling).
- **Lựa chọn 2: Hoãn toàn bộ `MOD-06` sang Phiên bản v1.1 (`Deferred to Post-MVP`)**
  - *Hệ quả*: Giảm thiểu phạm vi thiết kế ERD trong Stage 2 và kiểm thử trong Stage 5; tuy nhiên sản phẩm MVP sẽ hoàn toàn không thể phục vụ các nhà hàng gọi món cao cấp cần giữ bàn phòng VIP trước.

### 4.2. `OD-16`: Áp dụng Bộ Gói Cấu Hình Khởi Tạo (Onboarding Presets)
- **Lựa chọn 1: Chấp thuận 3 Gói Mẫu (Gia đình, Tiêu chuẩn, Cao cấp) dưới dạng Wizard Templates không ràng buộc**
  - *Hệ quả*: Giúp chủ nhà hàng thiết lập nhanh gian hàng trong lần đăng nhập đầu tiên; người dùng toàn quyền thay đổi từng tham số trước và sau khi lưu; không áp đặt bất kỳ sự cứng nhắc nào lên kiến trúc.
- **Lựa chọn 2: Bác bỏ Presets, chỉ hỗ trợ Cấu hình Thủ công Từng mục**
  - *Hệ quả*: Không tốn công sức thiết kế giao diện Wizard mẫu trong Admin; tuy nhiên người dùng mới sẽ đối mặt với ma trận 14 phân hệ và hơn 50 tham số, dễ dẫn đến thiết lập sai sót.

### 4.3. `OD-17`: Chính sách Thoát Nghiệp Vụ Phân Tầng khi Hủy Kích Hoạt Phân Hệ (Graceful Drain Policy)
- **Lựa chọn 1: Áp dụng Chính sách Thoát Phân Tầng theo Loại Giao dịch**
  - *Hệ quả*: 
    * Với bàn ăn tại chỗ: Chặn tắt phân hệ cốt lõi nếu còn bàn đang ăn.
    * Với lịch đặt bàn tương lai: Cho phép tắt phân hệ nhận mới, đưa các lịch đã đặt vào trạng thái Lưu trữ (Archive) để tra cứu.
    * Với thanh toán VietQR: Cấp cửa sổ 15 phút hứng nốt webhook ngân hàng treo trước khi đóng cổng.
    * Dữ liệu báo cáo lịch sử: Giữ nguyên 100% trên báo cáo tài chính.
    * *Đánh giá*: Vận hành trơn tru, không làm tê liệt thao tác quản trị, bảo toàn dữ liệu.
- **Lựa chọn 2: Áp dụng Khóa Cứng Tuyệt Đối (Monolithic Zero-Record Gate)**
  - *Hệ quả*: Hệ thống chặn tắt phân hệ nếu còn *bất kỳ* bản ghi nào trong CSDL; chủ quán sẽ không bao giờ tắt được phân hệ nếu có 1 khách đặt bàn ăn vào 2 tuần sau.

---

## 5. Deliverable 5: Ma Trận Truy Vết & Đánh Giá Tắc Nghẽn Câu Hỏi Mở (Open Question Traceability Matrix)

Bảng đối soát xác minh tính toàn vẹn danh xưng, nguồn gốc truy vết và đánh giá tác động làm nghẽn tiến độ của từng Câu hỏi Mở:

| Mã OQ | Nguồn Gốc Ban Đầu | Chủ Đề Hiện Tại | Tình Trạng Truy Vết | Làm Nghẽn Phê Duyệt Phạm Vi MVP? | Làm Nghẽn Kiến Trúc Kỹ Thuật Stage 2? | Làm Nghẽn Triển Khai Stage 3/5? | Giải Trình Tác Động Kỹ Thuật & Kiến Trúc |
| :---: | :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| **`OQ-01`** | BRS / Task 02.6 | Tách Hóa Đơn theo % Lẻ hoặc Tiền Tùy Biến | **Nguyên Vẹn 100%** | **KHÔNG** | **KHÔNG** | **CÓ** (Nếu không chốt hoãn) | Kiến trúc Stage 2 chỉ cần thiết kế mô hình thực thể `Bill` / `Invoice` hỗ trợ nạp danh sách `InvoiceItem` và hệ số chia. Thuật toán làm tròn số lẻ chỉ cần chốt khi viết mã nguồn Service ở Stage 5. **Khuyến nghị: Chốt hoãn sang v1.1**. |
| **`OQ-02`** | BRS / Task 02.6 | Tự Động In Nhiệt Bếp khi Bắn Đơn (`AutoPrintOnFire`) | **Nguyên Vẹn 100%** | **KHÔNG** | **KHÔNG** | **CÓ** (Khi cấu hình Terminal) | Đây là tham số cấu hình cấp trạm (`StationConfiguration`). Kiến trúc Stage 2 chỉ cần thiết kế bảng cấu hình có cột `AutoPrintOnFire: bool`. Giá trị mặc định `false` hay `true` không ảnh hưởng cấu trúc CSDL. |
| **`OQ-06`** | Task 02.7.3 / 02.7.5 | Tính Thuế VAT trên Phí Dịch Vụ (`IsServiceChargeTaxable`) | **Nguyên Vẹn 100%** | **KHÔNG** | **KHÔNG** | **CÓ** (Khi viết Engine tính tiền) | Là tham số cấu hình cấp chi nhánh. Stage 2 thiết kế cột `IsServiceChargeTaxable: bool` trong bảng `BranchConfiguration`. Công thức toán học được gọi linh hoạt trong `BillingCalculationService`. |
| **`OQ-07`** | Task 02.7.4 / 02.7.5 | Xử Lý Phiên Bàn Kéo Dài > 24 Giờ Vượt Mốc Ca | **Nguyên Vẹn 100%** | **KHÔNG** | **KHÔNG** | **CÓ** (Khi thiết kế UI cảnh báo) | Thực thể `TableSession` lưu `OpenedAt` và `BusinessDate`. Logic kiểm tra `Now - OpenedAt > 24h` được xử lý trong truy vấn hoặc Background Worker. |
| **`OQ-08`** | Task 02.7.4 / 02.7.5 | Chiến Lược Concurrency khi Nhiều Phục Vụ Cùng Gọi Món | **Nguyên Vẹn 100%** | **KHÔNG** | **CÓ (Thuộc phạm vi Stage 2)** | **CÓ** | Yêu cầu nghiệp vụ gọi món đồng thời không khóa bàn (PDR-014) đã được Owner xác nhận. Việc lựa chọn giải pháp kỹ thuật cụ thể (Optimistic Locking via RowVersion, Delta Command Append, hay Atomic Stored Procedure) được ủy quyền cho Solution Architect nghiên cứu, đánh giá các phương án và quyết định trong Stage 2 (TASK-03.4). |
| **`OQ-09`** | Task 02.7.4 / 02.7.5 | Chính Sách Điều Chỉnh & Hủy Hóa Đơn Đã Thanh Toán | **Nguyên Vẹn 100%** (Gắn với `OD-14`) | **CÓ (Cần định hình)** | **CÓ (Định hình Schema)** | **CÓ** | Nếu chọn Phương án B, ERD Stage 2 bắt buộc phải bổ sung bảng `CreditNotes` và liên kết với `Invoice`. Nếu chọn Phương án A, ERD giữ nguyên hóa đơn bất biến kết hợp bảng lưu vết sai lệch chốt ca. |
| **`OQ-10`** | Task 02.7.4 / 02.7.5 | Phân Vùng Khóa Dữ Liệu Thực Thể Đa Chi Nhánh | **Nguyên Vẹn 100%** | **KHÔNG** | **CÓ (Thuộc phạm vi Stage 2)** | **CÓ** | Ủy quyền cho Solution Architect đánh giá theo từng entity cụ thể trong Stage 2 (TASK-03.2). Không áp đặt cứng rằng mọi bảng CSDL đều phải chứa cả hai khóa `OrgId` và `BranchId`. |

---

## 6. Deliverable 6: Đánh Giá Mức Độ Sẵn Sàng Bước Vào Giai Đoạn 2 (Stage 2 Readiness Assessment)

### 6.1. Phân Loại 4 Cấp Độ Quyết Định Trước Khi Thiết Kế Kiến Trúc

| Cấp Độ Phân Loại | Danh Sách Quyết Định | Giải Trình Cơ Sở Kỹ Thuật & Kiến Trúc |
| :--- | :--- | :--- |
| **1. BẮT BUỘC CHỐT TRƯỚC KHI THIẾT KẾ STAGE 2 (MUST DECIDE BEFORE STAGE 2)** | **`OD-14` (`OQ-09`)**: Chính sách xử lý Hóa đơn đã thanh toán. | **Lý do**: Lựa chọn giữa Phương án A (Bất biến) và Phương án B (In-app Void) trực tiếp quyết định cấu trúc quan hệ thực thể (ERD) của Module Thu ngân trong Stage 2 (có bảng `CreditNotes`, máy trạng thái đảo ngược, và liên kết hoàn kho hay không). |
| **2. CÓ THỂ HOÃN / THAM SỐ HÓA TRONG KHI THIẾT KẾ STAGE 2 (CAN BE DEFERRED DURING STAGE 2)** | - **`OD-15`**: Trạng thái MVP của Đặt bàn (`MOD-06`).<br>- **`OD-16`**: Bộ Presets Onboarding.<br>- **`OD-17`**: Chính sách Graceful Drain.<br>- **Khung 14 Phân hệ**: 10 Core - 4 Optional. | **Lý do**: Kiến trúc Modular Monolith cho phép thiết kế `MOD-06` thành một Bounded Context độc lập có thể bật/tắt qua cờ tính năng; các gói Presets và bộ lọc Drain là các logic tầng ứng dụng (Application Layer), không làm biến dạng mô hình dữ liệu cốt lõi. |
| **3. BẮT BUỘC CHỐT TRƯỚC KHI BẮT ĐẦU TRIỂN KHAI MÃ NGUỒN (MUST DECIDE BEFORE IMPLEMENTATION - STAGE 3/5)** | - **`OQ-01`**: Thuật toán chia bill % lẻ.<br>- **`OQ-02`**: Mặc định in nhiệt trạm bếp.<br>- **`OQ-06`**: Mặc định tính VAT trên phí dịch vụ.<br>- **`OQ-07`**: Ngưỡng giờ cảnh báo bàn mở > 24h. | **Lý do**: Đây thuần túy là các giá trị mặc định của tham số (`Configuration Defaults`) và công thức toán học nội bộ trong Service. Mô hình dữ liệu đã có sẵn cột cờ, chỉ cần truyền giá trị khi viết mã nghiệp vụ. |
| **4. HẬU MVP / PHIÊN BẢN v1.1+ (POST-MVP)** | - **`OQ-04`**: Điều chuyển kho chuỗi đa chi nhánh.<br>- **`OQ-05`**: Cơ chế thanh toán trước cho QR tự gọi món.<br>- **BOM chi tiết**: Định mức gram/ml nguyên liệu thô. | **Lý do**: Đã được thống nhất loại trừ hoàn toàn khỏi phạm vi MVP v1.0 để bảo vệ mục tiêu ra mắt sản phẩm tinh gọn. |

### 6.2. Tuyên Bố Ranh Giới Quản Trị Về Quyền Hạn Kích Hoạt Stage 2
> [!IMPORTANT]
> **TUYÊN BỐ QUẢN TRỊ MINH BẠCH**:  
> Báo cáo này khẳng định **Stage 2 CHƯA ĐƯỢC PHÊ DUYỆT HOẶC CẤP PHÉP TRIỂN KHAI TỰ ĐỘNG**.  
> Việc khởi động Giai đoạn 2 (TASK-03: Thiết kế Kiến trúc Kỹ thuật Hệ thống & ERD) bắt buộc phải có văn bản hoặc chỉ thị phê duyệt chính thức từ Chủ Dự án (Project Owner), tối thiểu đối với Quyết định `OD-14`.

---

## 7. Deliverable 7: Báo Cáo Rủi Ro Còn Lại & Khuyến Nghị Chuyên Môn (Remaining Risks & Recommendations)

| Danh Mục Rủi Ro | Bản Chất Rủi Ro | Mức Độ | Tác Động Tiềm Tàng | Khuyến Nghị Kiểm Soát Của Kiến Trúc Sư |
| :--- | :--- | :---: | :--- | :--- |
| **Rủi ro 1: Trì hoãn Quyết định OD-14** | Chủ dự án chưa kịp đưa ra lựa chọn giữa Phương án A và B. | Trung bình | Gây phân vân khi thiết kế bảng `Invoices` trong TASK-03.2 (Data Models & ERD). | Nếu Chủ dự án cần thêm thời gian, Kiến trúc sư sẽ thiết kế schema theo **Phương án A làm gốc**, đồng thời để ngỏ trường `ParentInvoiceId` và cờ `IsReversed` để sẵn sàng nâng cấp sang Phương án B mà không làm gián đoạn tiến độ. |
| **Rủi ro 2: Bùng nổ Phạm vi Đa Chi Nhánh (Scope Creep)** | Tâm lý muốn đưa thêm tính năng quản lý chuỗi vào CSDL. | Cao | Làm phức tạp hóa các câu lệnh truy vấn và giao diện POS đơn cửa hàng. | Tuân thủ tuyệt đối `OD-01` (Single-store MVP): Phân vùng dữ liệu kỹ thuật sẽ được Solution Architect thẩm định theo từng entity trong TASK-03.2; cấm tuyệt đối đưa màn hình quản lý chuỗi vào UI MVP. |
| **Rủi ro 3: Tranh chấp Ghi đè khi Bắn Đơn Đồng Thời** | Hai phục vụ cùng bắn thêm món vào 1 bàn tại cùng thời điểm. | Cao | Trùng món, mất món, sai lệch số lượng giữ kho. | Được giao cho `TASK-03.4`: Solution Architect sẽ đánh giá các phương án Concurrency (RowVersion, Delta Append, hoặc Atomic Transaction) để bảo đảm không xung đột khi nhiều phục vụ cùng ghi món theo yêu cầu PDR-014. |

---

## 8. Deliverable 8: Báo Cáo Kiểm Tra Cây Làm Việc & Thay Đổi Git (Git Change & Provenance Report)

### 8.1. Kiểm Tra Trực Tiếp Trạng Thái Kho Chứa (`git status`)
Toàn bộ cây làm việc tại thư mục gốc `C:\Git\SmartRestaurant\SmartRestaurant` được kiểm tra chặt chẽ:
- **Tệp Sửa Đổi Chưa Stage (Unstaged Modified Files)**:
  * `.agent-memory/DEVELOPMENT-LOG.md`: Ghi nhật ký chuẩn xác cho Task 02.8.2.
  * `.agent-memory/KNOWN-ISSUES.md`: Giữ các cảnh báo kỹ thuật không đổi.
  * `.agent-memory/NEXT-TASK.md`: Đồng bộ đầu vào sang TASK-03 với điều kiện chờ Owner duyệt.
  * `.agent-memory/PROJECT-STATUS.md`: Ghi nhận hoàn tất Milestone 1.7.
  * `docs/00-index.md`: Đăng ký chỉ mục cho tài liệu hồ sơ quyết định.
  * `docs/product/decisions/PDR-003, 004, 005, 007, 008`: Các chú thích đã hiệu chỉnh từ Task 02.8.1.
- **Tệp Tạo Mới Chưa Theo Dõi (Untracked New Files)**:
  * `docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md`: Báo cáo Task 02.7.5.
  * `docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md`: Báo cáo Task 02.8.
  * `docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md`: Báo cáo Task 02.8.1.
  * `docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`: Hồ sơ quyết định và sẵn sàng Stage 2 (Tài liệu này).

### 8.2. Xác Nhận Tuân Thủ Tuyệt Đối Kỷ Luật Quản Trị
1. **Bảo toàn Tệp Quyết định Gốc**: Tệp `.agent-memory/ARCHITECTURE-DECISIONS.md` **HOÀN TOÀN KHÔNG BỊ CHỈNH SỬA (100% UNTOUCHED)**.
2. **Không Rò Rỉ Mã Nguồn Sớm**: Xác minh chính xác có **đúng 0 tệp C# (`.cs`), 0 tệp project (`.csproj`), 0 solution (`.sln`), và 0 migration database** được tạo ra.
3. **Không Thao Tác Phá Hủy Git**: Không thực thi các lệnh `git commit`, `git reset`, `git stash`, hay `git checkout`.

---

## 9. Trạng Thái Cổng Kiểm Soát Cuối Cùng (Final Status Declaration)

Căn cứ vào kết quả biểu quyết chính thức tại Hội thảo Quyết định Task 02.8.5 ([`TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md`](./TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md)), Giám đốc Kiểm toán Quản trị Sản phẩm & Trưởng Ban Kiến trúc Hệ thống chính thức công bố trạng thái cổng kiểm soát:

```
STAGE 1 COMPLETED & SIGNED OFF — READY FOR STAGE 2 AUTHORIZATION
```

> [!IMPORTANT]
> **XÁC NHẬN HOÀN TẤT GIAI ĐOẠN 1 (STAGE 1 FORMAL SIGN-OFF)**  
> 1. Toàn bộ hoạt động đặc tả, phân tích và biểu quyết quyết định Giai đoạn 1 đã **CHÍNH THỨC HOÀN TẤT & ĐƯỢC CHỦ DỰ ÁN PHÊ DUYỆT 100%**.  
> 2. Các quyết định cốt lõi (`OD-14..17`, `OQ-01..07`, Khung 14 Phân hệ) đã được hợp nhất thành các ranh giới bất biến.  
> 3. Cổng quản trị Giai đoạn 2 đã sẵn sàng được mở khóa để triển khai **TASK-03: Thiết Kế Kiến Trúc Hệ Thống & ERD**.
