# Báo Cáo Hiệu Chỉnh Quản Trị & Truy Nguyên Quyết Định
## TASK 02.8.6.1: GOVERNANCE CORRECTION & TRACEABILITY REPORT

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Giai Đoạn**: Stage 1 — Khám Phá Sản Phẩm & Đặc Tả Miền Nghiệp Vụ (Product Discovery & Business Domain Specifications)
- **Mã Nhiệm Vụ**: `TASK-02.8.6.1`
- **Loại Nhiệm Vụ**: Governance Correction & Decision Traceability Verification
- **Vai Trò Đảm Nhiệm**: Chuyên Viên Kiểm Toán Độc Lập Quản Trị Sản Phẩm & Người Duy Trì Tài Liệu (Independent Product Governance Auditor & Documentation Maintainer)
- **Ngày Hoàn Tất**: 2026-11-06
- **Quyết Định Nền Tảng Của Chủ Dự Án**:
  > *"Phê duyệt bước 1 nhưng chưa khởi động Giai đoạn 2, chờ tôi yêu cầu cụ thể"*
- **Phán Quyết Quản Trị**: **`PASS (CLEAN TRACEABILITY & FULL GOVERNANCE INTEGRITY SEAL)`**
- **Trạng Thái Quản Trị Bắt Buộc**:
  * **Stage 1**: **`OWNER-APPROVED (v1.0.0-FROZEN)`**
  * **Stage 2**: **`NOT AUTHORIZED / LOCKED`**
  * *(Nghiêm cấm sử dụng các trạng thái: `UNLOCKED`, `AUTHORIZED FOR IMMEDIATE EXECUTION`, `READY TO START TASK-03`)*

---

## 1. CÁC SAI LỆCH ĐƯỢC PHÁT HIỆN (DEVIATIONS DETECTED)

Quá trình kiểm toán độc lập đối với các tài liệu bàn giao thuộc Task 02.8.5, Task 02.8.6 và Task 02.9 đã phát hiện 4 nhóm sai lệch quản trị sau:

### 1.1. Sai Lệch Trạng Thái Quản Trị Cổng Giai Đoạn 2 (Governance Status Misalignment)
- **Hiện tượng**: Trong một số tài liệu lịch sử hình thành tại thời điểm hoàn tất Hội Thảo Task 02.8.5 và Báo cáo Task 02.8.6, xuất hiện các cụm từ:
  * `Stage 2 Gate: UNLOCKED & AUTHORIZED FOR IMMEDIATE EXECUTION`
  * `TRANSITION GATE TO STAGE 2: UNLOCKED & AUTHORIZED FOR EXECUTION`
  * `TASK APPROVED FOR IMMEDIATE COMMENCEMENT: TASK-03`
  * `Stage 2 transition gate is UNLOCKED and fully authorized for immediate execution` (tại `.agent-memory/NEXT-TASK.md#L76`).
- **Căn nguyên**: Các văn bản này được soạn thảo dựa trên kỳ vọng chuyển tiếp thông thường sau khi hoàn thành biểu quyết Workshop. Tuy nhiên, sau khi xem xét Báo cáo Kiểm toán Độc lập Task 02.9.1, **Chủ Dự Án đã ban hành chỉ thị dứt khoát**:
  > *"Phê duyệt nghiệm thu Stage 1 với các finding được ghi nhận và theo dõi. Đây không phải là ủy quyền bắt đầu Stage 2."*
- **Đánh giá**: Việc duy trì chuỗi ký tự `UNLOCKED` hoặc `AUTHORIZED FOR IMMEDIATE EXECUTION` là mâu thuẫn trực tiếp với quyết định quản trị của Chủ Dự Án và vi phạm ranh giới đóng băng của Stage 1.

### 1.2. Mismatch Tham Chiếu Giữa OD-13, OD-14 và BillingRequested Option 2 (`AUDIT-FINDING-ID-01`)
- **Hiện tượng**:
  * Tại [`BUSINESS-RULES-BASELINE.md#L23`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/BUSINESS-RULES-BASELINE.md#L23), quy tắc `BR-RULE-06` (Bàn Đang Thanh Toán Vẫn Cho Phép Sửa Đổi) được ghi nhận căn cứ là `OD-13 Option 2`.
  * Tại [`BUSINESS-REQUIREMENTS-BASELINE.md#L55`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/BUSINESS-REQUIREMENTS-BASELINE.md#L55), yêu cầu `BR-POS-005` ghi nhận căn cứ `OD-13 Option 2`.
  * Tại [`REQUIREMENT-TRACEABILITY-MATRIX.md#L44`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/REQUIREMENT-TRACEABILITY-MATRIX.md#L44), yêu cầu `BR-POS-005` ghi căn cứ `OD-13 Op2`.
- **Căn nguyên**: `OD-13` vốn là mã bất biến CSDL từ Task 02.6 cho nguyên tắc hóa đơn đã thanh toán là chỉ đọc, hoàn toàn không có "Option 2". Khái niệm "Option 2" thực chất là **Phương án 2 của nghiệp vụ BillingRequested** trong [`PDR-009`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/decisions/PDR-009-multi-bill-table-session-split-billing.md) §2 (thuộc quyết định `D4`). Đồng thời, quyết định chính sách xử lý khiếu nại sau thanh toán tại Hội Thảo Task 02.8.5 Ballot 1 mang mã chính thức là `OD-14`.

### 1.3. Mâu Thuẫn Trong Mô Tả Gói Cấu Hình Onboarding Presets (OD-16) Với REV-06 & OQ-02
- **Hiện tượng**:
  * Tại [`OWNER-DECISION-WORKSHOP.md#L168`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-WORKSHOP.md#L168), [`TASK-02.8.5`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md#L59), [`TASK-02.8.6`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md#L99), và sơ đồ [`TASK-02.9 Baseline`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md#L433), mô tả của **Gói VIP Fine Dining** ghi: *"phụ phí phòng VIP theo giờ"* hoặc *"10% hoặc phí giờ cố định"*.
- **Mâu thuẫn**: Quyết định `REV-06` ([`PDR-011`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/decisions/PDR-011-area-and-vip-room-surcharges.md) §2.1) đã khẳng định dứt khoát: MVP chỉ hỗ trợ phụ phí theo tỷ lệ phần trăm (+X%) hoặc số tiền cố định (+Y VNĐ) chụp ảnh tại thời điểm tạo đơn; **tính phí thuê phòng theo giờ (ví dụ: 100.000 ₫/giờ) được loại trừ rõ ràng sang phiên bản sau và KHÔNG có trong MVP**. Việc mô tả preset "theo giờ" đã ngấm ngầm đưa tính năng bị hoãn vào phạm vi gợi ý MVP.
- **Về in nhiệt (`OQ-02`)**: Cần bảo đảm mô tả cấu hình các gói thể hiện rõ nguyên tắc `AutoPrintOnFire: false` (KDS-first, máy in nhiệt làm dự phòng phần cứng).

### 1.4. Nguy Cơ Hiểu Lệch Ranh Giới Kỹ Thuật OQ-08 & OQ-10
- **Hiện tượng**: Cách diễn đạt trong Task 02.8.6 ("Ủy quyền cho Kiến trúc sư lựa chọn... tại TASK-03") nếu không có rào giậu quản trị có thể bị hiểu nhầm là trao quyền cho Solution Architect được tự ý bắt đầu Giai đoạn 2 hoặc tự ý thay đổi quyết định nghiệp vụ của Chủ Dự Án.

---

## 2. CÁC TÀI LIỆU ĐÃ SỬA VÀ NỘI DUNG SỬA (DOCUMENTS MODIFIED & CHANGES APPLIED)

Để khắc phục triệt để các sai lệch trên mà không làm mất lịch sử và không làm sai lệch quyết định của Chủ Dự Án, các hiệu chỉnh quản trị có kiểm soát (Controlled Governance Corrections) đã được thực hiện:

```mermaid
flowchart TD
    GOV["Hiệu Chỉnh Quản Trị Task 02.8.6.1"]
    
    GOV --> F1["1. Sửa Nhãn Trạng Thái Cổng Stage 2<br/>(LOCKED / NOT AUTHORIZED)<br/>- OWNER-DECISION-WORKSHOP.md<br/>- TASK-02.8.5 Consolidation Record<br/>- TASK-02.8.6 Implementation Report<br/>- OWNER-DECISION-DOSSIER.md<br/>- .agent-memory/NEXT-TASK.md<br/>- .agent-memory/PROJECT-STATUS.md<br/>- docs/00-index.md"]
    
    GOV --> F2["2. Phân Định Mã Quyết Định Chuẩn Xác<br/>- BR-RULE-06: PDR-009 (Op2) / D4<br/>- BR-RULE-17: OD-14 (Op A) / OD-13<br/>- BR-POS-005: PDR-009 (Op2) / D4<br/>- BR-BIL-005: OD-14 / OD-13"]
    
    GOV --> F3["3. Chuẩn Hóa Preset OD-16 Khớp REV-06 & OQ-02<br/>- Xóa phụ phí theo giờ trong Gói VIP<br/>- Chuẩn hóa: % hoặc số tiền cố định per REV-06<br/>- KDS-first, AutoPrintOnFire: false per OQ-02"]
    
    GOV --> F4["4. Rào Giậu Quản Trị OQ-08 & OQ-10<br/>- Bổ sung Important Guard Note<br/>- SA không có quyền tự mở khóa Stage 2"]
```

### Bảng Chi Tiết Nội Dung Hiệu Chỉnh Theo Từng Tệp:

| STT | Đường Dẫn Tệp | Vị Trí / Dòng | Nội Dung Trước Khi Hiệu Chỉnh | Nội Dung Sau Khi Hiệu Chỉnh Quản Trị |
| :---: | :--- | :---: | :--- | :--- |
| 1 | [`.agent-memory/NEXT-TASK.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/.agent-memory/NEXT-TASK.md) | Dòng 76 | `Stage 2 transition gate is UNLOCKED and fully authorized for immediate execution.` | `Stage 2 transition gate is NOT AUTHORIZED / LOCKED. Commencing Task 03 requires explicit, separate authorization from the Project Owner. Entire system remains on STRICT STANDBY.` |
| 2 | [`docs/product/OWNER-DECISION-WORKSHOP.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-WORKSHOP.md) | Dòng 10, 168, 484–495 | Cổng Stage 2 ghi `UNLOCKED & AUTHORIZED`; Gói VIP ghi `phụ phí phòng VIP theo giờ`; Section 9 ghi `UNLOCKED & AUTHORIZED FOR EXECUTION`. | Bổ sung Governance Correction note; Cổng Stage 2 thiết lập `NOT AUTHORIZED / LOCKED`; Gói VIP sửa thành `phụ phí phòng VIP (% hoặc số tiền cố định theo lượt per REV-06; không tính theo giờ)`; Section 9 niêm phong `NOT AUTHORIZED / LOCKED`. |
| 3 | [`docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md) | Dòng 59, 136–153 | Gói VIP ghi `phụ phí phòng VIP theo giờ`; Mục 4 ghi `TRANSITION GATE TO STAGE 2: UNLOCKED & AUTHORIZED FOR EXECUTION`. | Gói VIP sửa thành `% hoặc số tiền cố định theo lượt per REV-06`; Bổ sung Governance Correction note; Cổng Stage 2 niêm phong `NOT AUTHORIZED / LOCKED`. |
| 4 | [`docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md) | Dòng 17, 99, 152–156, 200, 215, 304–320 | Header ghi `UNLOCKED`; Gói VIP ghi `theo giờ`; thiếu rào giậu SA; bảng §7 ghi `mở khóa Cổng Stage 2 UNLOCKED`; Section 12 ghi `UNLOCKED`. | Header niêm phong `NOT AUTHORIZED / LOCKED`; Gói VIP sửa theo `REV-06`; Bổ sung `Stage 2 Technical Delegation Guard` cho `OQ-08/10`; Bảng §7 và Section 12 sửa chuẩn xác: `Stage 1 = OWNER-APPROVED`, `Stage 2 = NOT AUTHORIZED / LOCKED`. |
| 5 | [`docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md) | Dòng 224 | `Cổng quản trị Giai đoạn 2 đã sẵn sàng được mở khóa để triển khai TASK-03...` | `Cổng quản trị Giai đoạn 2 ở trạng thái NOT AUTHORIZED / LOCKED. Việc triển khai TASK-03 chỉ được thực hiện khi có văn bản hoặc chỉ thị phê duyệt riêng của Chủ Dự Án.` |
| 6 | [`docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md) | Dòng 433–434 | `P3_C["Phụ Thu Phòng VIP: BẬT (10% hoặc phí giờ cố định)"]` | `P3_C["Phụ Thu Phòng VIP: BẬT (10% hoặc phí cố định theo lượt per REV-06)"]`<br/>`P3_D["KDS Bếp: BẬT (KDS-first, AutoPrintOnFire: false, In Nhiệt Dự Phòng)"]` |
| 7 | [`docs/product/BUSINESS-RULES-BASELINE.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/BUSINESS-RULES-BASELINE.md) | Dòng 23, 34 | `BR-RULE-06` căn cứ `OD-13 Option 2`; `BR-RULE-17` căn cứ `OD-14 Option A`. | `BR-RULE-06` căn cứ **`PDR-009 (Op2) / D4`** (liên hệ bất biến `OD-13`); `BR-RULE-17` căn cứ **`OD-14 (Op A) / OD-13`**. |
| 8 | [`docs/product/BUSINESS-REQUIREMENTS-BASELINE.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/BUSINESS-REQUIREMENTS-BASELINE.md) | Dòng 55 | `BR-POS-005` ghi `(OD-13 Option 2)` | `BR-POS-005` ghi `(PDR-009 BillingRequested Option 2 / D4)` |
| 9 | [`docs/product/REQUIREMENT-TRACEABILITY-MATRIX.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/REQUIREMENT-TRACEABILITY-MATRIX.md) | Dòng 44, 61 | `BR-POS-005` ghi `OD-13 Op2`; `BR-BIL-005` ghi `OD-14`. | `BR-POS-005` ghi **`PDR-009 (Op2)/D4`**; `BR-BIL-005` ghi **`OD-14 / OD-13`** và **`OD-14 / PDR-009`**. |
| 10 | [`docs/product/STAGE-2-PRODUCT-HANDOFF.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/STAGE-2-PRODUCT-HANDOFF.md) | Dòng 87–94 | Ràng buộc 6 ghi `OD-13 Option 2`; thiếu quyết định `OQ-07`. | Ràng buộc 6 sửa thành `PDR-009 (BillingRequested Option 2) / D4`; Ràng buộc 7 ghi `OD-14 Option A (kết hợp OD-13)`; Bổ sung Ràng buộc 13 cho `OQ-07 Option A`. |
| 11 | [`docs/product/PRODUCT-SPECIFICATION-BASELINE.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/PRODUCT-SPECIFICATION-BASELINE.md) | Dòng 104, 113–120 | Điểm 6 chỉ ghi `OD-14`; thiếu `OQ-07` trong danh mục Hội Thảo. | Điểm 6 làm rõ liên hệ giữa `BillingRequested Op2`, `OD-13` và `OD-14`; Bổ sung `OQ-07 (Phương án A)` vào danh mục quyết định Hội Thảo Task 02.8.5. |
| 12 | [`.agent-memory/PROJECT-STATUS.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/.agent-memory/PROJECT-STATUS.md) | Dòng 39 | `... Stage 2 Gate Unlocked.` | `... Stage 2 Gate NOT AUTHORIZED / LOCKED per Owner Directive.` |
| 13 | [`.agent-memory/DEVELOPMENT-LOG.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/.agent-memory/DEVELOPMENT-LOG.md) | Dòng 306, 323, 349 | Ghi nhận các tuyên bố `UNLOCKED & AUTHORIZED` lịch sử. | Thêm chú thích Governance Correction giải thích việc hiệu chỉnh trạng thái sang `NOT AUTHORIZED / LOCKED` theo quyết định Owner. |
| 14 | [`docs/00-index.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/00-index.md) | Dòng 29, 60 | `... và xác nhận mở khóa Cổng Stage 2.` | `... và xác nhận ranh giới Cổng Stage 2 (NOT AUTHORIZED / LOCKED).` |

---

## 3. BẰNG CHỨNG TRUY NGUYÊN OD-14 / OD-13 (PROVENANCE TRACEABILITY EVIDENCE)

Để giải quyết triệt để sự mơ hồ về mặt ký hiệu học giữa hai mã quyết định `OD-13` và `OD-14`, kiểm toán viên độc lập thực hiện đối soát pháp y qua 4 câu hỏi trọng tâm:

```mermaid
flowchart LR
    subgraph T026["1. Bất Biến Gốc Task 02.6"]
        OD13["<b>OD-13</b>: Paid Bill Immutability<br/>(PDR-009 §3 — Không có Option)"]
        BILL_OP2["<b>BillingRequested Option 2</b><br/>(PDR-009 §2, D4)"]
    end
    subgraph WORKSHOP["2. Quyết Định Mới Task 02.8.5"]
        OQ09["Câu hỏi mở OQ-09: Xử lý khiếu nại sau thanh toán"] --> OD14["<b>OD-14 (Ballot 1)</b>: Paid Bill Void/Refund Policy<br/>Owner phê chuẩn: <b>Phương Án A</b><br/>- CSDL bất biến (OD-13)<br/>- Biên bản giấy ngoài két<br/>- Log ShiftClosingVarianceLog chốt ca mù<br/>- Hoãn CreditNote sang v1.1"]
    end
    subgraph RULES["3. Ánh Xạ Quy Tắc Cơ Sở (Baseline)"]
        OD13 & BILL_OP2 --> BR06["<b>BR-RULE-06</b>: Cho phép sửa món khi in tạm tính<br/>(Căn cứ: PDR-009 Op2 / D4, liên hệ OD-13)"]
        OD13 & OD14 --> BR17["<b>BR-RULE-17</b>: Xử lý khiếu nại hóa đơn đã đóng tiền<br/>(Căn cứ: OD-14 Op A / OD-13)"]
    end
```

### Câu Hỏi 1: OD-14 có phải mã quyết định chính thức sau Task 02.8.5 không?
- **Kết luận**: **CÓ, HOÀN TOÀN CHÍNH THỨC**.
- **Bằng chứng thực chứng**:
  * Tại [`TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md#L37-L45`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md#L37-L45) §2.1: Quyết định cốt lõi Cấp 1 đầu tiên được định danh chính thức là **`OD-14: Chính sách Xử lý Khiếu nại & Hủy/Hoàn tiền Hóa đơn đã Thanh toán`**, với kết quả biểu quyết được Chủ Dự Án phê chuẩn là **`PHƯƠNG ÁN A`**.
  * Tại [`OWNER-DECISION-WORKSHOP.md#L47-L50`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-WORKSHOP.md#L47-L50) §3.1: Mã định danh chính thức là `OD-14`.
  * Tại [`OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md#L28`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md#L28): Quyết định được đăng ký với mã `OD-14` mang trạng thái `OWNER-CONFIRMED (Phương án A)`.

### Câu Hỏi 2: OD-13 có phải mã cũ đã được thay thế không?
- **Kết luận**: **KHÔNG PHẢI MÃ CŨ BỊ THAY THẾ**.
- **Bằng chứng thực chứng**:
  * Mã **`OD-13`** được thiết lập từ Task 02.6 ([`PDR-009`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/decisions/PDR-009-multi-bill-table-session-split-billing.md) §3) là **Bất biến CSDL tuyệt đối (Database Invariant)**: *"Hóa đơn đã thanh toán (`IsSettled = true`) là bất biến, nghiêm cấm nhân viên thu ngân hay quản lý chỉnh sửa/xóa trực tiếp trên phần mềm"*.
  * Trong toàn bộ hồ sơ chuẩn bị quyết định [`OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md#L87-L88`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md#L87-L88) và [`OWNER-DECISION-WORKSHOP.md#L38`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OWNER-DECISION-WORKSHOP.md#L38) §2, `OD-13` được liệt kê độc lập bên cạnh `OD-14`:
    - Dòng `OD-13`: *Tính Bất Biến của Hóa Đơn Đã Thanh Toán* (`OWNER-CONFIRMED`).
    - Dòng `OD-14`: *Chính sách Xử lý Khiếu nại & Hủy Hóa Đơn Đã Thanh Toán* (`OWNER-CONFIRMED — Phương án A`).
  * `OD-13` đóng vai trò là tiền đề nền tảng (ràng buộc bất biến), còn `OD-14` là chính sách quy trình vận hành và kế toán khi có sai lệch tiền thực tế.

### Câu Hỏi 3: Việc chuyển mã có bằng chứng trong biên bản hay không?
- **Kết luận**: **KHÔNG CÓ VIỆC CHUYỂN MÃ**.
- **Bằng chứng thực chứng**:
  * Biên bản Workshop Task 02.8.5 và Báo cáo Triển khai Task 02.8.6 hoàn toàn không có bất kỳ dòng nào tuyên bố "thay thế OD-13 bằng OD-14" hay "đổi tên OD-13 thành OD-14".
  * Sự nhầm lẫn chỉ xuất hiện do việc biên tập sơ suất đã gán nhãn `OD-13 Option 2` cho quy tắc `BR-RULE-06`. Nhãn đúng phải là `BillingRequested Option 2` (thuộc quyết định `D4` / `PDR-009`).
  * Nhóm duy trì tài liệu đã sửa nhãn này về đúng nguồn gốc: **`PDR-009 (Op2) / D4`**.

### Câu Hỏi 4: Quyết định về hóa đơn đã thanh toán và phần Credit Note v1.1 có được phân tách nhất quán không?
- **Kết luận**: **HOÀN TOÀN NHẤT QUÁN 100%**.
- **Bằng chứng thực chứng**:
  * **Trong phạm vi MVP (v1.0)**: CSDL bất biến tuyệt đối (`OD-13`); mọi khiếu nại sai lệch tiền mặt được xử lý bằng "Biên bản điều chỉnh sai lệch két giấy" ký tay ngoài quầy giữa Quản lý và Thu ngân; hệ thống ghi nhận khoản tiền lệch vào `ShiftClosingVarianceLog` khi kiểm tiền mù cuối ca (`OD-14 Option A`, `BR-RULE-17`).
  * **Phân kỳ hoãn sang v1.1**: Toàn bộ quy trình số hóa hoàn tiền trong ứng dụng, máy trạng thái đảo ngược doanh thu và thực thể bút toán âm `CreditNote` được hoãn chính thức sang phiên bản **`v1.1`** (mã đăng ký: [`DEF-CREDITNOTE`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md#L49)).
  * Sự phân tách này được duy trì nhất quán trên 100% các tài liệu: `TASK-02.8.5` (§2.1), `OWNER-DECISION-WORKSHOP` (§3.1), `TASK-02.8.6` (§3.1), `PRODUCT-SPECIFICATION-BASELINE` (§4 điểm 6), và `STAGE-2-PRODUCT-HANDOFF` (§4 ràng buộc 7).

---

## 4. KẾT QUẢ KIỂM TRA OD-16 VỚI REV-06 VÀ OQ-02 (PRESETS CONSISTENCY AUDIT)

Kiểm toán viên đã rà soát chi tiết 3 bộ cấu hình mẫu gợi ý khởi tạo (Onboarding Presets) thuộc Quyết định `OD-16 (Phương án A)` đối chiếu với các quyết định nghiệp vụ đã được Chủ Dự Án xác nhận:

### 4.1. Đối Chiếu Với Quyết Định `REV-06` (Phụ Phí Phòng VIP & Khu Vực — `PDR-011`)
- **Nội dung `REV-06` đã duyệt**:
  * Phụ phí theo tỷ lệ phần trăm (+X% tính trên tổng tiền món ăn).
  * Phụ phí số tiền cố định (+Y VNĐ cộng vào hóa đơn).
  * **Loại trừ phạm vi bắt buộc**: Tính phí thuê phòng theo giờ (ví dụ: 100.000 ₫/giờ) bị loại trừ rõ ràng sang phiên bản sau và **KHÔNG có trong MVP**.
- **Phát hiện mâu thuẫn**:
  * Trong mô tả Gói 3 (VIP Fine Dining) tại một số văn bản ghi: *"phụ phí phòng VIP theo giờ"* hoặc *"10% hoặc phí giờ cố định"*. Đây là lỗi sao chép ý niệm chưa được thanh lọc.
- **Biện pháp xử lý đã thực hiện**:
  * Sửa 100% các mô tả preset VIP thành: **`Phụ phí phòng VIP (tỷ lệ % hoặc số tiền cố định theo lượt per REV-06; không hỗ trợ tính theo giờ trong MVP)`**.
  * Cam kết tuyệt đối không đưa tính năng tính phí theo giờ bị hoãn vào phạm vi gợi ý MVP.

### 4.2. Đối Chiếu Với Quyết Định `OQ-02` (Mặc Định KDS-First & Tắt In Nhiệt Bếp Tự Động)
- **Nội dung `OQ-02` đã duyệt**: Mặc định tham số `AutoPrintOnFire: false` (ưu tiên KDS điện tử; không in nhiệt đồng thời khi báo bếp; máy in nhiệt ESC/POS chỉ làm dự phòng phần cứng hoặc in lại thủ công).
- **Kiểm tra tính tương thích của 3 Presets**:
  * **Gói 1 (Quán Ăn Bình Dân / Gia Đình)**: `MOD-09` (KDS) tắt $\rightarrow$ Máy in nhiệt bếp được bật làm phương tiện chế biến chính. Hoàn toàn phù hợp logic (khi không có màn hình cảm ứng thì in giấy là kênh duy nhất).
  * **Gói 2 (Nhà Hàng Tiêu Chuẩn)**: `MOD-09` (KDS) bật $\rightarrow$ Tuân thủ nghiêm ngặt `AutoPrintOnFire: false` (KDS-first, bếp không giấy).
  * **Gói 3 (Nhà Hàng Cao Cấp / VIP)**: `MOD-09` (KDS) bật $\rightarrow$ Tuân thủ nghiêm ngặt `AutoPrintOnFire: false`, máy in nhiệt đóng vai trò dự phòng phần cứng (Hardware backup / on-demand reprint).
- **Kết luận**: Sau khi chuẩn hóa câu chữ, 100% 3 Presets hoàn toàn nhất quán với `REV-06` và `OQ-02`.

---

## 5. KẾT QUẢ KIỂM TRA RANH GIỚI OQ-08 VÀ OQ-10 (TECHNICAL DELEGATION BOUNDARIES)

Kiểm toán viên xác nhận vị trí, bản chất và thẩm quyền xử lý của hai câu hỏi kỹ thuật `OQ-08` và `OQ-10`:

```mermaid
flowchart TD
    subgraph OWNER_BOUNDARY["Ranh Giới Bất Biến Thuộc Thẩm Quyền Chủ Dự Án (Stage 1)"]
        OWNER_08["Yêu cầu nghiệp vụ gọi món đồng thời không khóa bàn<br/><b>OWNER-CONFIRMED (PDR-014)</b>"]
        OWNER_10["Phạm vi kinh doanh MVP là 01 nhà hàng độc lập tại 01 địa điểm<br/><b>OWNER-CONFIRMED (OD-01 / CONTR-01)</b>"]
    end
    subgraph SA_BOUNDARY["Ranh Giới Nghiên Cứu Kỹ Thuật Của Solution Architect (Stage 2)"]
        TECH_08["OQ-08 (TECH-01): Đánh giá RowVersion vs Delta Append vs Channels<br/><b>PROPOSED — INVESTIGATION ONLY</b>"]
        TECH_10["OQ-10 (TECH-06): Đánh giá Global Query Filters vs Composite Keys<br/><b>PROPOSED — INVESTIGATION ONLY</b>"]
    end
    subgraph FORBIDDEN["Ranh Giới Bị Nghiêm Cấm Đối Với Solution Architect"]
        NO_CHANGE["1. KHÔNG được thay đổi quyết định nghiệp vụ của Owner"]
        NO_UNLOCK["2. KHÔNG được tự ý cấp phép mở khóa Stage 2"]
        NO_CODE["3. KHÔNG được triển khai code/schema khi chưa có lệnh Owner"]
    end
    OWNER_08 --> TECH_08
    OWNER_10 --> TECH_10
    TECH_08 & TECH_10 -.-> FORBIDDEN
```

### 5.1. Định Danh Bản Chất `OQ-08` (Cơ Chế Kiểm Soát Đồng Thời Đơn Hàng Bàn Chung)
- **Nghiệp vụ cốt lõi**: Yêu cầu nhiều nhân viên phục vụ cùng lúc gọi món trên một bàn ăn mà không bị khóa cứng giao diện là quyết định nghiệp vụ đã được Chủ Dự Án phê duyệt ([`PDR-014`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/decisions/PDR-014-concurrent-order-append-business-rules.md), `REV-10`).
- **Phạm vi kỹ thuật**: Việc lựa chọn công nghệ cụ thể (`RowVersion` lạc quan, lệnh nối thêm delta commands, hay kênh bộ nhớ in-process channels) là đề xuất kỹ thuật được bàn giao cho Solution Architect nghiên cứu so sánh trong Stage 2 (`TASK-03.4`).

### 5.2. Định Danh Bản Chất `OQ-10` (Ranh Giới Khóa Phân Vùng Dữ Liệu Đa Chi Nhánh)
- **Nghiệp vụ cốt lõi**: Quyết định `OD-01` giới hạn MVP cho một nhà hàng độc lập tại một địa điểm duy nhất. Không có yêu cầu bắt buộc mọi database entity phải chứa cả hai trường `OrganizationId` và `BranchId`.
- **Phạm vi kỹ thuật**: Việc lựa chọn thực thể nào cần khóa kép (danh mục dùng chung) và thực thể nào chỉ cần khóa đơn (giao dịch vận hành) là nội dung nghiên cứu kỹ thuật của Solution Architect trong Stage 2 (`TASK-03.2`).

### 5.3. Tuyên Bố Ranh Giới Quản Trị Bắt Buộc (Mandatory Delegation Guard)
Solution Architect **TUYỆT ĐỐI KHÔNG CÓ THẨM QUYỀN**:
1. Thay đổi hoặc diễn giải lại các quyết định nghiệp vụ đã được Chủ Dự Án chốt (`PDR-014`, `OD-01`).
2. Tự cấp phép (self-authorize) mở khóa Giai đoạn 2.
3. Tự ý khởi tạo cấu trúc thư viện .NET, viết mã nguồn C#, tạo Entity Framework migrations hoặc scaffold CSDL khi Giai đoạn 2 chưa được Chủ Dự Án chính thức ủy quyền bằng văn bản.

---

## 6. TỔNG HỢP KHÁC BIỆT KHO LƯU TRỮ (GIT DIFF SUMMARY)

Dưới đây là thống kê pháp y Git toàn bộ các thay đổi hiệu chỉnh quản trị trong nhiệm vụ:

```
 .agent-memory/DEVELOPMENT-LOG.md                   | 34 ++++++++++++++++++++++
 .agent-memory/NEXT-TASK.md                         |  6 ++--
 .agent-memory/PROJECT-STATUS.md                    |  8 +++--
 docs/00-index.md                                   |  4 +--
 docs/product/BUSINESS-REQUIREMENTS-BASELINE.md     |  2 +-
 docs/product/BUSINESS-RULES-BASELINE.md            | 14 ++++-----
 docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md |  2 +-
 docs/product/OWNER-DECISION-WORKSHOP.md            | 18 +++++++-----
 docs/product/PRODUCT-SPECIFICATION-BASELINE.md     |  3 +-
 docs/product/REQUIREMENT-TRACEABILITY-MATRIX.md    |  4 +--
 docs/product/STAGE-2-PRODUCT-HANDOFF.md            |  5 +++-
 docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md | 20 ++++++++-----
 docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md | 21 +++++++++-----
 docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md  |  6 ++--
 docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md         |  2 +-
 15 files changed, 99 insertions(+), 50 deletions(-)
```

### Các Tệp Báo Cáo Quản Trị Được Tạo Mới:
- [`docs/product/TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-TRACEABILITY-REPORT.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/product/TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-TRACEABILITY-REPORT.md) (Báo cáo chính thức của nhiệm vụ này).

---

## 7. XÁC NHẬN KHÔNG CÓ CODE HOẶC KIẾN TRÚC STAGE 2 ĐƯỢC TẠO (ZERO CODE ATTESTATION)

Kiểm toán viên độc lập thực hiện lệnh kiểm tra pháp y đệ quy trên toàn bộ workspace:

```powershell
# 1. Kiểm tra sự tồn tại của thư mục kiến trúc Stage 2
Test-Path "docs/02-architecture"
# Kết quả thực tế: False

# 2. Quét đệ quy tìm kiếm mã nguồn và tệp cấu trúc dự án
Get-ChildItem -Recurse -Include *.cs, *.csproj, *.sln, *.sql
# Kết quả thực tế: 0 files found
```

### Chứng Nhận Kỷ Luật Kho Lưu Trữ (100% Clean Repository):
1. **Mã nguồn C# (`.cs`)**: **Chính xác 0 tệp**.
2. **Tệp dự án / Solution (`.csproj`, `.sln`)**: **Chính xác 0 tệp**.
3. **Kịch bản cơ sở dữ liệu / Migrations (`.sql`)**: **Chính xác 0 tệp**.
4. **Thư mục thiết kế kỹ thuật Stage 2 (`docs/02-architecture/`)**: **Hoàn toàn chưa tồn tại (`False`)**.
5. **Bộ quy tắc tối cao (`GEMINI.md`, `AGENTS.md`)**: **0 diff so với HEAD**.
6. **Kiểm tra liên kết nội bộ**: **58/58 liên kết trong [`docs/00-index.md`](file:///C:/Git/SmartRestaurant/SmartRestaurant/docs/00-index.md) hợp lệ, 0 liên kết gãy**.

---

## 8. BIÊN BẢN PHÁN QUYẾT QUẢN TRỊ CUỐI CÙNG (FINAL GOVERNANCE STATUS DECLARATION)

```
========================================================================================
             BIÊN BẢN PHÁN QUYẾT HIỆU CHỈNH QUẢN TRỊ & TRUY NGUYÊN QUYẾT ĐỊNH
                   DỰ ÁN SMARTRESTAURANT — TASK 02.8.6.1 GOVERNANCE SEAL
========================================================================================

TRẠNG THÁI QUẢN TRỊ CHÍNH THỨC:
  - GIAI ĐOẠN 1 (STAGE 1) : OWNER-APPROVED (v1.0.0-FROZEN)
  - GIAI ĐOẠN 2 (STAGE 2) : NOT AUTHORIZED / LOCKED

BẢNG TỔNG HỢP KIỂM TOÁN NGHIỆM THU:
----------------------------------------------------------------------------------------
1. Tuyên bố trạng thái Cổng Stage 2     : ĐÃ LOẠI BỎ 100% CỤM TỪ UNLOCKED TRÁI PHÉP
2. Quyết định OD-14 vs OD-13            : 100% MINH BẠCH & CÓ BẰNG CHỨNG GỐC RÕ RÀNG
3. Quyết định Onboarding Presets (OD-16): 100% KHỚP VỚI REV-06 (KHÔNG TÍNH GIỜ) & OQ-02
4. Ranh giới ủy quyền OQ-08 & OQ-10     : 100% RÀO GIẬU AN TOÀN, CHỐNG TỰ CẤP PHÉP
5. Rò rỉ mã nguồn / Kiến trúc Stage 2   : ĐÚNG 0 TỆP (ZERO CODE / ZERO SCHEMA)
6. Toàn vẹn quyết định Chủ Dự Án        : BẢO TOÀN NGUYÊN VẸN 23 QUYẾT ĐỊNH & 14 PHÂN HỆ
----------------------------------------------------------------------------------------
PHÁN QUYẾT CHÍNH THỨC : PASS (CLEAN TRACEABILITY & FULL GOVERNANCE INTEGRITY SEAL)

CHỈ THỊ VẬN HÀNH TIẾP THEO:
Toàn bộ hệ thống SmartRestaurant duy trì chế độ chờ nghiêm ngặt (STRICT STANDBY).
Nghiêm cấm khởi động TASK-03, viết code hoặc scaffold dự án cho đến khi nhận được
văn bản chỉ thị kích hoạt chính thức từ Chủ Dự Án.
========================================================================================
```