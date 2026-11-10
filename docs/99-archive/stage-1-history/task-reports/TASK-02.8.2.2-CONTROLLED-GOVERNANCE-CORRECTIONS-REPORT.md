# Báo Cáo Hiệu Chỉnh Quản Trị Có Kiểm Soát
## TASK 02.8.2.2: CONTROLLED GOVERNANCE CORRECTIONS REPORT

- **Dự Án**: SmartRestaurant — Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Giai Đoạn**: Stage 1 — Khám Phá Sản Phẩm & Đặc Tả Miền Nghiệp Vụ (Product Discovery & Business Domain Specifications)
- **Vai Trò Thực Hiện**: Chuyên Viên Quản Trị Tài Liệu Sản Phẩm Cấp Cao (Senior Product Governance Documentation Specialist)
- **Tài Liệu Báo Cáo Căn Cứ**: [`FINAL-EVIDENCE-RECONCILIATION-REPORT.md`](./FINAL-EVIDENCE-RECONCILIATION-REPORT.md)
- **Ngày Thực Hiện**: 2026-11-05
- **Tình Trạng Thẩm Quyền Sau Hiệu Chỉnh**: `STAGE 1 OWNER SIGN-OFF PENDING (GATE TO STAGE 2 LOCKED)`

---

## 1. Mục Đích & Nguyên Tắc Thực Thi

Nhiệm vụ `TASK 02.8.2.2` thực hiện hiệu chỉnh có kiểm soát đối với các sai lệch quản trị được phát hiện tại vòng kiểm tra cuối (`FINAL-EVIDENCE-RECONCILIATION-REPORT.md`), bảo đảm:
- 100% tuân thủ ranh giới Stage 1, tuyệt đối không thực hiện công việc thiết kế kiến trúc kỹ thuật.
- Tuyệt đối không chỉnh sửa mã nguồn C#, không tạo project/solution, không tạo migration database.
- **Tuyệt đối không sửa đổi bất kỳ tệp nào trong `.agent-memory/`** trong Task này theo đúng chỉ thị bắt buộc.
- Không tự chuyển trạng thái `OPEN` hoặc `AI/BA PROPOSAL` thành `OWNER-CONFIRMED`.
- Bảo toàn nguyên vẹn các quyết định gốc `D1–D5` và `REV-05` đến `REV-11`.

---

## 2. Chi Tiết Thực Hiện Từng Mục Hiệu Chỉnh (COR-01 Đến COR-06)

### 2.1. COR-01 — Tách Biệt Hoàn Toàn Quyết Định D3 và D4
- **Tệp và Vị Trí Đã Hiệu Chỉnh**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Mục 2 (Deliverable 2: Owner Decision Register), bảng đăng ký quyết định, dòng 84–85.
  * [`docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md`](./decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md): Mục 3 Ghi chú ranh giới phê duyệt, dòng 42.
- **Nội Dung Trước Hiệu Chỉnh**:
  ```markdown
  | **`D3/D4`** | **Tách Hóa Đơn Không Khóa Bàn & Multi-Tender** | Owner Sign-off | `OWNER-CONFIRMED` | Cho phép tách bill theo món & tách đều 1/N; thanh toán nhiều hình thức (Tiền mặt, VietQR, Thẻ). |
  ```
  *(D3 bị gộp vào D4 và bị xóa bỏ hoàn toàn định danh xử lý VietQR treo).*
- **Nội Dung Sau Hiệu Chỉnh**:
  ```markdown
  | **`D3`** | **Xử Lý VietQR Đến Chậm & Hàng Đợi Tiền Treo (Suspense Ledger)** | Owner Sign-off (PDR-008) | `OWNER-CONFIRMED` | Cho phép thu tiền mặt khi QR timeout. Giao dịch QR về muộn đưa vào Hàng đợi Tiền treo; thông báo tức thì cho thu ngân; đối soát thủ công có phê duyệt. |
  | **`D4`** | **Phiên Bàn Đa Hóa Đơn (Multi-Bill) & Tách Bill Không Khóa Bàn** | Owner Sign-off (PDR-009) | `OWNER-CONFIRMED` | Bàn không bị khóa khi tách bill. Phục vụ tiếp tục gọi món vào bill chưa thanh toán. Hỗ trợ tách theo món & tách đều $1/N$; thanh toán Multi-Tender. |
  ```
  *Tại PDR-005*: Làm rõ D4/REV-04 và OD-13 là căn cứ chính cho việc tách bill và tính bất biến; D3/REV-03 là căn cứ chính sách khi phương thức thanh toán VietQR được lựa chọn.
- **Bằng Chứng Quyết Định Owner Sử Dụng**:
  * `D3 / REV-03`: Biên bản Task 02.6 chốt phương án xử lý VietQR treo tại [`PDR-008`](./decisions/PDR-008-vietqr-delayed-webhook-handling.md).
  * `D4 / REV-04`: Biên bản Task 02.6 chốt mô hình phiên bàn đa hóa đơn không khóa bàn tại [`PDR-009`](./decisions/PDR-009-multi-bill-table-session-split-billing.md).
- **Trạng Thái Thẩm Quyền Sau Hiệu Chỉnh**: `OWNER-CONFIRMED` (Độc lập cho cả D3 và D4).

---

### 2.2. COR-02 — Sửa Thẩm Quyền Khấu Trừ Kho (Inventory Deduction)
- **Tệp và Vị Trí Đã Hiệu Chỉnh**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Mục 1.2 bảng 5 khái niệm (dòng 58–60) và Mục 1.2 khối cảnh báo (dòng 62–64).
- **Nội Dung Trước Hiệu Chỉnh**:
  ```markdown
  | **4. Inventory Deduction (Khấu trừ sổ kho)** | Bút toán kế toán chuyển đổi từ ReservedStock sang ConsumedStock... | ... | **Theo Chính sách Trạm Bếp (PDR-004)** | OWNER-CONFIRMED (PDR-004 Step 2) |
  ...
  Điểm trừ kho được cấu hình theo chính sách trạm bếp (ví dụ: quầy Bar trừ ngay khi in phiếu, bếp Nóng trừ khi hoàn thành) theo đúng PDR-004 Step 2.
  ```
  *(Tự ý gán nhãn OWNER-CONFIRMED cho chính sách trừ kho theo trạm).*
- **Nội Dung Sau Hiệu Chỉnh**:
  ```markdown
  | **3. Waste Recording (Ghi nhận Hao phí)** | Ghi sổ tổn thất khi món đã nấu nhưng bị đổ, cháy, hoặc khách hủy sau khi bếp đã nấu. | Ghi sổ hao hụt vào `WasteLog` khi làm lại món do lỗi bếp theo REV-11 / PDR-015 | Khi hủy/làm lại món sau chế biến | `OWNER-CONFIRMED (PDR-004/015)` |
  | **4. Inventory Deduction (Khấu trừ sổ kho)** | Bút toán kế toán chuyển đổi từ `ReservedStock` sang `ConsumedStock` và giảm `PhysicalStock`. | `PhysicalQuantity -= Qty`<br>`ReservedQuantity -= Qty` | Chưa định nghĩa trong Stage 1; Solution Architect sẽ đánh giá và thiết kế trong Stage 2 | `AI/BA PROPOSAL / TECHNICAL DESIGN REQUIRED — STAGE 2` |
  | **5. KDS Status Transitions (Chuyển trạng thái KDS)** | Tiến trình hiển thị thị giác trên màn hình bếp (`Chờ nấu` $\rightarrow$ `Đang nấu` $\rightarrow$ `Chờ cung ứng` $\rightarrow$ `Đã phục vụ`). | `OrderItem.KitchenStatus`<br>(Chỉ phục vụ điều phối nhịp độ bếp) | Đầu bếp chạm vào màn hình KDS | `AI/BA PROPOSAL (Vận hành thị giác)` |

  > [!IMPORTANT]
  > **Xác nhận Ranh giới Quản trị về Khấu trừ Kho (COR-02)**:  
  > Quyết định D2/D5 của Chủ Dự án chỉ xác nhận công thức Tồn kho Khả dụng, thời điểm tạm giữ (Reservation) khi gửi bếp, và giải phóng khi hủy đơn hợp lệ. Chủ Dự án KHÔNG ấn định thời điểm chuyển đổi từ Reserved sang Consumed, không mặc định KDS ReadyToServe là sự kiện trừ kho, và không tự quyết định việc Bar trừ kho khi in phiếu hay Bếp Nóng trừ kho khi hoàn thành KDS. Toàn bộ thời điểm và cơ chế kích hoạt trừ kho thực tế được phân loại chính xác là AI/BA PROPOSAL / TECHNICAL DESIGN REQUIRED — STAGE 2 và thuộc thẩm quyền đánh giá của Solution Architect trong Stage 2.
  ```
- **Bằng Chứng Quyết Định Owner Sử Dụng**:
  * Quyết định `D2/D5`: Khóa kho 2 pha (`Khả dụng = Vật lý - Giữ trước`); giữ khi gửi bếp; hủy hợp lệ giải phóng; BOM chi tiết gram/ml hoãn sang v1.1.
  * Quyết định `REV-11`: Làm lại món do lỗi bếp không tính tiền khách, ghi nhận hao phí vào `WasteLog`.
- **Trạng Thái Thẩm Quyền Sau Hiệu Chỉnh**:
  * Công thức $\text{Available} = \text{Physical} - \text{Reserved}$, thời điểm Reservation khi gửi bếp, và giải phóng khi hủy hợp lệ: **`OWNER-CONFIRMED`**.
  * Chính sách sự kiện trừ kho và chuyển Reserved sang Consumed: **`AI/BA PROPOSAL / TECHNICAL DESIGN REQUIRED — STAGE 2`**.

---

### 2.3. COR-03 — Sửa Phân Tích Quyết Định OD-14 (Hóa Đơn Đã Thanh Toán)
- **Tệp và Vị Trí Đã Hiệu Chỉnh**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Mục 3 (Deliverable 3: OD-14 Paid Bill Correction Comparison), bảng so sánh dòng 105–113.
- **Nội Dung Trước Hiệu Chỉnh**:
  ```markdown
  Tác Động Triển Khai MVP (Phương án A): 0 ngày phát sinh. Mô hình thực thể Invoice và Payment ở Stage 2 cực kỳ tinh gọn, không cần máy trạng thái phức tạp...
  Hóa đơn trong CSDL giữ nguyên 100% không đổi. Không thêm máy trạng thái hay hóa đơn điều chỉnh...
  ```
  *(Tuyên bố chủ quan không có bằng chứng, ép buộc tên bảng cụ thể).*
- **Nội Dung Sau Hiệu Chỉnh**:
  ```markdown
  | **2. Hàm Ý Kiểm Toán Tài Chính (Financial Audit Implications)** | - Hóa đơn trong CSDL giữ nguyên 100% doanh thu ban đầu.<br>- Báo cáo POS hiển thị doanh thu thô (Gross). Khoản chênh lệch tiền mặt bắt buộc phải được đối soát trên "Biên bản Kiểm đếm Tiền mặt Cuối ca" (Blind Shift Close Variance) và có khả năng tham chiếu tới hóa đơn gốc bị điều chỉnh.<br>- **Yêu cầu nghiệp vụ bắt buộc cần Stage 2 đánh giá**: Cần cơ chế lưu vết ghi nhận chênh lệch giữa tiền thực tế và số liệu hệ thống, ghi nhận lý do, người thực hiện, thời điểm và người xác minh, có khả năng tham chiếu hóa đơn gốc nếu có liên quan, bảo đảm audit trail và khả năng đối soát kế toán (không bắt buộc tên bảng hay khóa ngoại cụ thể, giao Solution Architect thiết kế).<br>- Triệt tiêu nguy cơ nhân viên gian lận thông đồng xóa bill trên máy. | - Hệ thống tự động sinh thực thể `CreditNote` (Bút toán âm) triệt tiêu doanh thu tương ứng.<br>- Báo cáo chốt ca (Z-Report) phản ánh trực tiếp doanh thu thuần tự động.<br>- Cần cơ chế kiểm soát nội bộ nghiêm ngặt: Nếu Quản lý lộ mã PIN, có nguy cơ nhân viên thông đồng hủy hóa đơn để biển thủ tiền mặt. |
  | **3. Độ Phức Tạp Vận Hành (Operational Complexity)** | - Quy trình nghiệp vụ kết hợp giữa phần mềm và thủ tục hành chính giấy tờ.<br>- Đòi hỏi Quản lý phải ghi chép biên bản giấy, nhập mã sai lệch khi chốt ca và bàn giao đối chiếu với kế toán mỗi sáng. | - Đòi hỏi đào tạo nhân viên và Quản lý về máy trạng thái hủy, lý do hủy bắt buộc (Reason Codes), và quy trình bàn giao két tự động. |
  | **4. Tác Động Triển Khai MVP (MVP Implementation Impact)** | - **Cần Stage 2 đánh giá cấu trúc dữ liệu**: Không tạo máy trạng thái đảo ngược hóa đơn phức tạp, nhưng **vẫn cần thiết kế cấu trúc dữ liệu lưu vết chênh lệch và thông tin đối soát trong Stage 2/3** để đảm bảo khả năng đối soát và audit trail. Loại bỏ nhận định không có căn cứ cho rằng Phương án A "không phát sinh chi phí", "không tác động ERD" hay "không cần lưu dữ liệu". | - **Tăng thêm 7 - 10 ngày công**: Thiết kế kiến trúc Stage 2 và triển khai Stage 3/5: Tạo thực thể điều chỉnh/hủy, bút toán âm, hoàn tiền từng phần, cập nhật lại trạng thái bàn/kho, và kiểm thử hồi quy. |
  ```
  *Bổ sung tuyên bố trung lập*: Kiến trúc sư không thiên vị bất kỳ phương án nào; quyết định thuộc thẩm quyền độc quyền của Chủ Dự án.
- **Bằng Chứng Quyết Định Owner Sử Dụng**:
  * Quyết định `OD-13`: Hóa đơn đã thanh toán (`IsSettled = true`) là bất biến tuyệt đối.
  * Quyết định `BillingRequested Option 2`: Hóa đơn chưa thanh toán cho phép sửa; thêm món nạp vào bill hiện hữu hoặc tạo sub-bill mới; hóa đơn đã thanh toán không sửa trực tiếp.
- **Trạng Thái Thẩm Quyền Sau Hiệu Chỉnh**: **`OPEN — OWNER DECISION REQUIRED`**.

---

### 2.4. COR-04 — Sửa Ranh Giới Kỹ Thuật OQ-08 (Concurrency)
- **Tệp và Vị Trí Đã Hiệu Chỉnh**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Mục 5 ma trận OQ (dòng 153) và Mục 7 ma trận rủi ro (dòng 184).
- **Nội Dung Trước Hiệu Chỉnh**:
  ```markdown
  OQ-08: Đã được ủy quyền giải quyết trong Stage 2 (TASK-03.4) bằng mô hình Optimistic Concurrency Control (RowVersion) kết hợp Delta Command Append.
  Đã lên kế hoạch giải quyết triệt để tại TASK-03.4: Sử dụng RowVersion cho bảng TableSession kết hợp lệnh Append độc lập cho bảng OrderItem.
  ```
  *(Trình bày sớm giải pháp kỹ thuật cụ thể như một quyết định đã chốt).*
- **Nội Dung Sau Hiệu Chỉnh**:
  ```markdown
  OQ-08: Yêu cầu nghiệp vụ gọi món đồng thời không khóa bàn (PDR-014) đã được Owner xác nhận. Việc lựa chọn giải pháp kỹ thuật cụ thể (Optimistic Locking via RowVersion, Delta Command Append, Stored Procedure, hoặc In-memory Channels) là các phương án kỹ thuật để Solution Architect nghiên cứu, đánh giá độc lập trong Stage 2 (TASK-03.4). Không có phương án kỹ thuật nào được coi là đã được Owner phê duyệt trước.
  ...
  Rủi ro 3: Được giao cho TASK-03.4: Solution Architect sẽ đánh giá các phương án Concurrency (RowVersion, Delta Append, Stored Procedure, hoặc In-memory Channels) để bảo đảm không xung đột khi nhiều phục vụ cùng ghi món theo yêu cầu PDR-014.
  ```
- **Bằng Chứng Quyết Định Owner Sử Dụng**:
  * Quyết định [`PDR-014`](./decisions/PDR-014-concurrent-order-append-business-rules.md): Yêu cầu nghiệp vụ cho phép phục vụ gọi món đồng thời vào cùng một bàn mà không gây khóa bàn.
- **Trạng Thái Thẩm Quyền Sau Hiệu Chỉnh**:
  * Yêu cầu nghiệp vụ: **`OWNER-CONFIRMED (PDR-014)`**.
  * Giải pháp công nghệ (`RowVersion`, `Delta Append`, `Stored Procedure`): **`TECHNICAL DESIGN REQUIRED — STAGE 2 (Proposal)`**.

---

### 2.5. COR-05 — Sửa Ranh Giới Kỹ Thuật OQ-10 (Multi-Branch Partitioning)
- **Tệp và Vị Trí Đã Hiệu Chỉnh**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Mục 5 ma trận OQ (dòng 155) và Mục 7 ma trận rủi ro (dòng 182).
- **Nội Dung Trước Hiệu Chỉnh**:
  ```markdown
  Tuân thủ tuyệt đối OD-01: Giữ OrganizationId và BranchId ở tầng CSDL để phân vùng kỹ thuật...
  ```
  *(Áp đặt cứng OrganizationId và BranchId cho mọi bảng CSDL).*
- **Nội Dung Sau Hiệu Chỉnh**:
  ```markdown
  OQ-10: OD-01 chỉ xác nhận phạm vi kinh doanh MVP là một nhà hàng độc lập tại một địa điểm. Không áp đặt OrganizationId và BranchId trên mọi bảng CSDL. Việc lựa chọn khóa, quan hệ và chiến lược phân vùng dữ liệu phải được Solution Architect đánh giá theo từng entity cụ thể trong Stage 2 (TASK-03.2). Tuyệt đối không tự ý mở rộng MVP thành sản phẩm quản lý chuỗi hay đa chi nhánh.
  ...
  Rủi ro 2: Tuân thủ tuyệt đối OD-01 (Single-store MVP): Phân vùng dữ liệu kỹ thuật sẽ được Solution Architect thẩm định theo từng entity trong TASK-03.2; cấm tuyệt đối đưa màn hình quản lý chuỗi vào UI MVP.
  ```
- **Bằng Chứng Quyết Định Owner Sử Dụng**:
  * Quyết định `OD-01 / CONTR-01`: Giới hạn phạm vi kinh doanh MVP trong duy nhất 01 nhà hàng độc lập tại 01 địa điểm duy nhất.
- **Trạng Thái Thẩm Quyền Sau Hiệu Chỉnh**:
  * Ranh giới cửa hàng đơn lập: **`OWNER-CONFIRMED (OD-01)`**.
  * Chiến lược khóa phân vùng thực thể CSDL: **`TECHNICAL DESIGN REQUIRED — STAGE 2 (Proposal)`**.

---

### 2.6. COR-06 — Kiểm Tra Trạng Thái Stage Gate & Ghi Nhận Sai Lệch Bộ Nhớ
- **Nguyên Tắc Bắt Buộc**: **Không thực hiện bất kỳ chỉnh sửa nào trong `.agent-memory/` trong Task này** theo đúng chỉ thị.
- **Ghi Nhận Sai Lệch Trong Bộ Nhớ Dự Án**:
  * Tệp [`.agent-memory/PROJECT-STATUS.md`](../../.agent-memory/PROJECT-STATUS.md) trước đây từng ghi `Current Stage: Stage Gate Passed` và `Stage 2: GATE APPROVED (TASK-03 READY)`.
  * **Đánh Giá Kiểm Toán**: Đây là sai lệch nghiêm trọng về mặt quản trị. Khi Chủ Dự án chưa ký duyệt nghiệm thu Stage 1, việc ghi nhận "Gate Passed" hoặc "Gate Approved" là vi phạm ranh giới thẩm quyền phê duyệt.
  * **Yêu Cầu Hành Động**: Cần trình Chủ Dự án (Project Owner) phê duyệt một tác vụ cấp quyền riêng biệt để đồng bộ lại trạng thái chính thức của `.agent-memory/PROJECT-STATUS.md`.
- **Trạng Thái Chuẩn Xác Hiện Tại Được Xác Nhận**:
  * **STAGE 1 STATUS**: `STAGE 1 OWNER SIGN-OFF PENDING` (Toàn bộ tài liệu đã được niêm phong, sẵn sàng chờ Chủ Dự án ký duyệt).
  * **STAGE 2 AUTHORIZATION**: `NOT GRANTED (GATE LOCKED)` (Tuyệt đối không tự ý kích hoạt TASK-03 hay viết mã nguồn).

---

## 3. Kết Quả Kiểm Tra Bổ Sung

### 3.1. Kiểm Tra Quyết Định D1 và OQ-07 (Ngày Kinh Doanh & Bàn Kéo Dài > 24 Giờ)
- **Căn cứ Thẩm quyền D1**: Quyết định gốc `D1` chỉ xác nhận: Ngày Kinh Doanh có giờ cắt ca mặc định là 06:00 sáng. Phiên bàn mở thuộc ca nào thì giữ nguyên Ngày Kinh Doanh của ca đó xuyên suốt ca đêm.
- **Xử lý Sai lệch**: Tại dòng 82 của [`OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md), câu chữ đã được điều chỉnh chuẩn xác:
  ```markdown
  | **`D1`** | **Ranh giới Ngày Kinh Doanh & Giờ Cắt Ca** | Owner Sign-off | `OWNER-CONFIRMED` | Mặc định Cutoff 06:00 sáng. Phiên bàn mở giữ nguyên Ngày Kinh Doanh (`BusinessDate`) tại thời điểm mở xuyên suốt ca đêm. (Lưu ý: Việc xử lý phiên bàn kéo dài > 24h thuộc OQ-07 đang OPEN, không thuộc phạm vi phê duyệt của D1). |
  ```
- **Trạng thái OQ-07**: Giữ nguyên trạng thái **`OPEN — OWNER DECISION REQUIRED`**. Ý tưởng hiển thị banner cảnh báo màu hổ phách chỉ là đề xuất của BA (`AI/BA PROPOSAL`), không được coi là quyết định đã phê duyệt.

### 3.2. Kiểm Tra Quyết Định REV-11 (Remake Dish & Hao Phí)
- **Căn cứ Thẩm quyền REV-11**: Chủ dự án chỉ phê duyệt: Làm lại món do lỗi bếp không tính thêm tiền cho khách; có nhận diện/thẻ riêng trên KDS; ghi nhận hao hụt vào `WasteLog`.
- **Xử lý Sai lệch**: Đã loại bỏ các câu chữ tự ý mở rộng yêu cầu "bắt buộc mã PIN quản lý cho mọi thao tác hao hụt chung" tại bảng khái niệm 1.2, giữ đúng phạm vi đã được phê duyệt tại [`PDR-015`](./decisions/PDR-015-remake-dish-workflow.md).

---

## 4. Danh Sách Các Vấn Đề Còn OPEN Cần Chủ Dự Án Ra Quyết Định

Bảng tổng hợp toàn bộ các quyết định chính sách còn mở, không bị tự động giải quyết:

| Mã Quyết Định | Chủ Đề Nghiệp Vụ | Tóm Tắt Lựa Chọn Cần Chủ Dự Án Quyết Định | Phân Loại Quản Trị |
| :---: | :--- | :--- | :---: |
| **`OD-14`** | Chính sách Hóa đơn đã thanh toán | **Phương án A**: Bất biến CSDL, đối soát lệch két ca giấy.<br>**Phương án B**: In-app Void/Refund với bút toán âm điện tử. | `OPEN — OWNER DECISION REQUIRED` |
| **`OD-15`** | Phân vị Đặt bàn trước (`MOD-06`) | **Phương án A**: Phân hệ Tùy chọn MVP (`MVP Optional`).<br>**Phương án B**: Hoãn toàn bộ sang Phiên bản v1.1. | `OPEN — OWNER DECISION REQUIRED` |
| **`OD-16`** | Bộ Presets Khởi tạo Onboarding | **Phương án A**: Áp dụng 3 Gói gợi ý Wizard không ràng buộc.<br>**Phương án B**: Bác bỏ Presets, cấu hình thủ công 100%. | `OPEN — OWNER DECISION REQUIRED` |
| **`OD-17`** | Chính sách Thoát Graceful Drain | **Phương án A**: Thoát phân tầng theo loại giao dịch.<br>**Phương án B**: Khóa cứng tuyệt đối nếu còn bản ghi. | `OPEN — OWNER DECISION REQUIRED` |
| **`OQ-01`** | Tách bill theo % lẻ (vd 60/40) | **Phương án A**: Hoãn sang v1.1; MVP hỗ trợ Tách theo món & Tách đều $1/N$.<br>**Phương án B**: Bắt buộc đưa vào MVP. | `OPEN — OWNER DECISION REQUIRED` |
| **`OQ-02`** | Tự động in nhiệt trạm bếp | **Phương án A**: Cấu hình trạm, mặc định `false` (Ưu tiên KDS).<br>**Phương án B**: Mặc định `true` (Luôn tự động in giấy). | `OPEN — OWNER DECISION REQUIRED` |
| **`OQ-06`** | Tính thuế VAT trên phí dịch vụ | **Phương án A**: Mặc định `IsServiceChargeTaxable: true`.<br>**Phương án B**: Mặc định `false` (Phí dịch vụ miễn thuế). | `OPEN — OWNER DECISION REQUIRED` |
| **`OQ-07`** | Bàn kéo dài > 24h qua mốc ca | **Phương án A**: Giữ BusinessDate gốc + Banner cảnh báo Quản lý.<br>**Phương án B**: Tự động cưỡng bức đóng bàn tại mốc cắt ca. | `OPEN — OWNER DECISION REQUIRED` |
| **Khung Phân Hệ** | Phân tầng 14 Phân hệ Nghiệp vụ | Phê chuẩn Khung cơ cấu 10 Phân hệ Cốt lõi (Core) và 4 Phân hệ Tùy chọn (Optional). | `AI/BA PROPOSAL (Awaiting Sign-off)` |

---

## 5. Báo Cáo An Toàn Git & Nguồn Gốc Thay Đổi (Git Safety & Provenance)

### 5.1. Kiểm Tra Trạng Thái Cây Làm Việc (`git status --porcelain`)
Kiểm tra thực tế sau khi hoàn tất các hiệu chỉnh:

```text
 M .agent-memory/DEVELOPMENT-LOG.md
 M .agent-memory/KNOWN-ISSUES.md
 M .agent-memory/NEXT-TASK.md
 M .agent-memory/PROJECT-STATUS.md
 M docs/00-index.md
 M docs/product/decisions/PDR-003-independent-station-kitchen-routing.md
 M docs/product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md
 M docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md
 M docs/product/decisions/PDR-007-business-date-shift-management.md
 M docs/product/decisions/PDR-008-vietqr-delayed-webhook-handling.md
?? docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md
?? docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md
?? docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md
?? docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md
?? docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md
?? docs/product/TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md
```

### 5.2. Phân Định Nguồn Gốc Thay Đổi Thuộc Phạm Vi Task 02.8.2.2:
- **Tệp Hiệu Chỉnh Trong Task Này**:
  * [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](./OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md): Đã hiệu chỉnh COR-01, COR-02, COR-03, COR-04, COR-05, D1/OQ-07, REV-11.
  * [`docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md`](./decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md): Hiệu chỉnh làm rõ ranh giới D4 vs D3.
  * [`docs/product/TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md`](./TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md): Tạo báo cáo kiểm toán này.
- **Tệp Thay Đổi Có Sẵn Từ Các Task Trước (Không thuộc phạm vi chỉnh sửa mới của Task này)**:
  * `PDR-003`, `PDR-004`, `PDR-007`, `PDR-008`: Hiệu chỉnh ranh giới thẩm quyền từ Task 02.7.5.
  * `FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md` (Task 02.7.5).
  * `MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md` (Task 02.8).
  * `MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md` (Task 02.8.1).
  * `FINAL-EVIDENCE-RECONCILIATION-REPORT.md` (Task 02.8.2.1).
  * `.agent-memory/`: Tuyệt đối không thực hiện bất kỳ chỉnh sửa nào trong Task này theo đúng chỉ thị.

---

## 6. Xác Nhận Tuyệt Đối Ranh Giới Quản Trị (Governance Boundary Certification)

Chuyên viên Quản trị Tài liệu Sản phẩm Cấp cao xin trân trọng xác nhận và cam kết danh dự:
1. **Không Viết Mã Nguồn**: Xác nhận có **đúng 0 tệp C# (`.cs`), 0 tệp project (`.csproj`), 0 solution (`.sln`)** tồn tại trong kho lưu trữ.
2. **Không Thiết Kế Database Sớm**: Xác nhận có **đúng 0 kịch bản SQL Migration / EF Core** tồn tại; không có API hay schema nào được tạo ra.
3. **Không Bắt Đầu Kiến Trúc Sớm**: Xác nhận thư mục `docs/02-architecture/` **hoàn toàn chưa tồn tại**.
4. **Không Can Thiệp Tệp Quyết Định Gốc**: Xác nhận tệp `.agent-memory/ARCHITECTURE-DECISIONS.md` **hoàn toàn nguyên vẹn 100% (Zero diff against HEAD)**.
5. **Không Sửa Bộ Nhớ Trong Task Này**: Xác nhận không thực thi bất kỳ thao tác chỉnh sửa nào đối với `.agent-memory/` trong suốt quá trình thực hiện Task 02.8.2.2.
6. **Không Thao Tác Phá Hủy Git**: Không có lệnh `git commit`, `git reset`, `git stash`, hay `git checkout` nào được thực thi.

---

## 7. Điều Kiện Dừng & Trình Báo Cáo Cho Chủ Dự Án

```
================================================================================
FINAL GOVERNANCE STATUS:
STAGE 1 BASELINE AUDITED & SEALED — PENDING PROJECT OWNER SIGN-OFF
STAGE 2 TRANSITION GATE: LOCKED (AUTHORIZATION NOT GRANTED)
================================================================================
```

> [!IMPORTANT]
> **TIẾN TRÌNH ĐÃ HOÀN TẤT VÀ DỪNG LẠI HOÀN TOÀN.**  
> - Hệ thống **KHÔNG TỰ Ý CHẠY** Task 02.8.3, Task 02.9 hay bất kỳ tác vụ nào khác.  
> - Hệ thống **KHÔNG TỰ Ý CHUYỂN SANG GIAI ĐOẠN 2 (TASK-03)**.  
> - Báo cáo này cùng toàn bộ Hồ sơ Quyết định tại [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md) trân trọng kính trình **Chủ Dự Án (Project Owner)** xem xét, cho ý kiến chỉ đạo và ký duyệt chính thức.
