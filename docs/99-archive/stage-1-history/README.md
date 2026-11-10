# SmartRestaurant — Phân Hệ Lưu Trữ Lịch Sử Giai Đoạn 1
## STAGE 1 HISTORICAL ARCHIVE REPOSITORY (docs/99-archive/stage-1-history/)

- **Dự Án**: SmartRestaurant — Hệ Thống Quản Lý Vận Hành Nhà Hàng Độc Lập
- **Thời Điểm Niêm Phong**: 2026-11-10
- **Thẩm Quyền Quản Lý**: Chủ Dự Án (Project Owner)
- **Chế Độ Bảo Tồn**: **STRICT READ-ONLY (Chỉ Đọc Tuyệt Đối — Bất Biến Lịch Sử)**
- **Mục Đích**: Lưu trữ toàn bộ các báo cáo thực thi nhiệm vụ, biên bản kiểm toán độc lập, hồ sơ workshop và tài liệu bản thảo sơ khai của Giai đoạn 1 phục vụ tra cứu hồi tố và kiểm toán pháp lý.

---

> [!CAUTION]
> **CHỈ THỊ BẮT BUỘC DÀNH CHO CÁC PHIÊN LÀM VIỆC CỦA AI AGENT TƯƠNG LAI:**
> 1. Toàn bộ các tệp trong thư mục `docs/99-archive/` là **HỒ SƠ LỊCH SỬ ĐÃ NIÊM PHONG**.
> 2. Các tệp này **KHÔNG PHẢI LÀ YÊU CẦU NGHIỆP VỤ HOẠT ĐỘNG HIỆN HÀNH**.
> 3. Tuyệt đối **KHÔNG ĐƯỢC ĐỌC ĐỆ QUY** thư mục này trong các phiên làm việc thông thường.
> 4. Nguồn sự thật duy nhất về nghiệp vụ hiện hành là [**`docs/PRODUCT-BASELINE-v1.0.0.md`**](../../PRODUCT-BASELINE-v1.0.0.md).
> 5. AI Agent chỉ được phép truy cập thư mục này khi và chỉ khi **Chủ Dự Án đưa ra chỉ thị bằng văn bản riêng** yêu cầu tra cứu bằng chứng kiểm toán cụ thể.

---

## 1. Cấu Trúc Phân Hệ Lưu Trữ (Archive Hierarchy)

```
docs/99-archive/stage-1-history/
├── README.md                # Tệp chỉ mục quản trị phân hệ lưu trữ (Tệp này)
├── task-reports/            # 14 báo cáo thực thi nhiệm vụ, đối soát bằng chứng & workshop
├── audit-reports/           # 10 báo cáo kiểm toán độc lập & thẩm định tính toàn vẹn
└── superseded-documents/    # 5 tài liệu bản thảo tiền thân đã bị thay thế hoàn toàn
```

---

## 2. Danh Mục Hồ Sơ Lưu Trữ Chi Tiết (Detailed Archive Register)

### 2.1. Thư Mục Báo Cáo Thực Thi Nhiệm Vụ (`task-reports/` — 14 tệp)

| STT | Tên Tệp Lưu Trữ | Nhiệm Vụ Nguồn | Tóm Tắt Nội Dung Lịch Sử | Trạng Thái Lưu Trữ |
| :---: | :--- | :---: | :--- | :---: |
| 1 | `PRODUCT-REVIEW-REPORT.md` | Task 02.5 pt 2 | Báo cáo phản biện nghiệp vụ độc lập lần 1; ghi nhận 4 Critical và 6 High. | `ARCHIVED RECORD` |
| 2 | `PRODUCT-REVIEW-VALIDATION.md` | Task 02.5 pt 1 | Báo cáo thẩm định nghiệp vụ chéo giai đoạn khám phá. | `ARCHIVED RECORD` |
| 3 | `OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md` | Task 02.8.2 | Hồ sơ tổng hợp các quyết định mở chuẩn bị cho Workshop biểu quyết. | `ARCHIVED RECORD` |
| 4 | `FINAL-EVIDENCE-RECONCILIATION-REPORT.md` | Task 02.8.2.1 | Báo cáo đối soát bằng chứng vật lý và bộ nhớ dự án. | `ARCHIVED RECORD` |
| 5 | `TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md` | Task 02.8.2.2 | Báo cáo xử lý 6 sai lệch quản trị COR-01..06. | `ARCHIVED RECORD` |
| 6 | `TASK-02.8.3-VIETNAMESE-LOCALIZATION-REPORT.md` | Task 02.8.3 | Báo cáo hoàn tất Việt hóa Markdown toàn diện. | `ARCHIVED RECORD` |
| 7 | `OWNER-DECISION-WORKSHOP.md` | Task 02.8.5 | Hồ sơ 8 lá phiếu biểu quyết gốc có chữ ký phê duyệt của Chủ Dự Án. | `ARCHIVED LEGAL RECORD` |
| 8 | `TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md` | Task 02.8.5 | Biên bản hợp nhất quyết định phê chuẩn 100% của Chủ Dự Án. | `ARCHIVED LEGAL RECORD` |
| 9 | `TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md` | Task 02.8.6 | Báo cáo triển khai đồng bộ hóa quyết định Owner vào tài liệu sản phẩm. | `ARCHIVED RECORD` |
| 10 | `TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-TRACEABILITY-REPORT.md` | Task 02.8.6.1 | Báo cáo hiệu chỉnh sai lệch quản trị OD-13/14 và Preset OD-16. | `ARCHIVED RECORD` |
| 11 | `TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md` | Task 02.9 | Hồ sơ hợp nhất cơ sở đặc tả sản phẩm v1.0.0-FROZEN. | `ARCHIVED RECORD` |
| 12 | `TASK-02.9.2-GOVERNANCE-FINDING-RESOLUTION-REPORT.md` | Task 02.9.2 | Báo cáo giải quyết dứt điểm 4 phát hiện kiểm toán F-01..F-04. | `ARCHIVED RECORD` |
| 13 | `TASK-02.9.3.1-FINAL-AUDIT-FINDING-CLARIFICATION-AND-EVIDENCE-REPORT.md` | Task 02.9.3.1 | Báo cáo thẩm định thực chứng 7 vấn đề bằng chứng mục tiêu. | `ARCHIVED RECORD` |
| 14 | `TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md` | Task 03.0 | Kế hoạch quy hoạch hợp nhất và lưu trữ tài liệu Giai đoạn 1. | `ARCHIVED RECORD` |

### 2.2. Thư Mục Báo Cáo Kiểm Toán Độc Lập (`audit-reports/` — 10 tệp)

| STT | Tên Tệp Lưu Trữ | Nhiệm Vụ Nguồn | Tóm Tắt Kết Quả Kiểm Toán | Phán Quyết Kiểm Toán |
| :---: | :--- | :---: | :--- | :---: |
| 1 | `FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md` | Task 02.7.5 | Kiểm toán tuân thủ 10 cổng người gác cổng Stage 1. | `PASS` |
| 2 | `MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md` | Task 02.8.1 | Kiểm toán tính nhất quán phân hệ và Graceful Drain. | `PASS` |
| 3 | `TASK-02.8.2.3-INDEPENDENT-POST-CORRECTION-VERIFICATION.md` | Task 02.8.2.3 | Kiểm toán độc lập 100% sau các hiệu chỉnh quản trị. | `PASS (10/10 PASS)` |
| 4 | `TASK-02.8.4-INDEPENDENT-VIETNAMESE-QUALITY-VERIFICATION.md` | Task 02.8.4 | Thẩm định độc lập 8 tiêu chí chất lượng Việt hóa tài liệu. | `PASS (8/8 PASS)` |
| 5 | `TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-DECISION-TRACEABILITY-VERIFICATION.md` | Task 02.8.6.1 | Thẩm định thực chứng truy nguyên 23 quyết định của Owner. | `PASS (CLEAN SEAL)` |
| 6 | `TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md` | Task 02.9 | Báo cáo đối soát Git baseline và kiểm toán quy tắc. | `PASS (10/10 PASS)` |
| 7 | `TASK-02.9.1-INDEPENDENT-PRODUCT-SPECIFICATION-AUDIT.md` | Task 02.9.1 | Kiểm toán độc lập cơ sở đặc tả (0 Critical, 0 High, 1 Med, 1 Low). | `PASS WITH FINDINGS` |
| 8 | `TASK-02.9.2.1-INDEPENDENT-GOVERNANCE-INTEGRITY-AUDIT.md` | Task 02.9.2.1 | Kiểm toán tính toàn vẹn quản trị sau Task 02.9.2 (bản tóm tắt). | `PASS` |
| 9 | `TASK-02.9.2.1-INDEPENDENT-GOVERNANCE-INTEGRITY-VERIFICATION.md` | Task 02.9.2.1 | Kiểm toán thực chứng 10/10 tiêu chí và kiểm tra zero code leakage. | `PASS (10/10 PASS)` |
| 10 | `TASK-02.9.3-FINAL-STAGE-1-GOVERNANCE-AND-BASELINE-INTEGRITY-AUDIT.md` | Task 02.9.3 | Kiểm toán pháp y toàn diện 70 tệp trước khi đóng hồ sơ Stage 1. | `PASS WITH FINDINGS` |

### 2.3. Thư Mục Tài Liệu Bản Thảo Đã Bị Thay Thế (`superseded-documents/` — 5 tệp)

| STT | Tên Tệp Lưu Trữ | Tên Tệp Gốc | Tài Liệu Thay Thế Chính Thức | Lý Do Lưu Trữ |
| :---: | :--- | :--- | :--- | :--- |
| 1 | `LEGACY-PRD-INITIAL-DRAFT.md` | `docs/01-requirements/product-requirements-document.md` | `PRODUCT-BASELINE-v1.0.0.md` | Bản thảo PRD sơ khai Task 01; đã bị thay thế hoàn toàn. |
| 2 | `BUSINESS-REQUIREMENTS.md` | `docs/product/BUSINESS-REQUIREMENTS.md` | `PRODUCT-BASELINE-v1.0.0.md` §7 | Bản thảo BRS tiền baseline; đã được hợp nhất đầy đủ. |
| 3 | `MVP-SCOPE.md` | `docs/product/MVP-SCOPE.md` | `PRODUCT-BASELINE-v1.0.0.md` §3 | Bản thảo Scope tiền baseline; đã được hợp nhất đầy đủ. |
| 4 | `MODULE-OVERVIEW.md` | `docs/product/MODULE-OVERVIEW.md` | `PRODUCT-BASELINE-v1.0.0.md` §3, §12 | Trùng lặp nội dung với Capability Map và Configuration. |
| 5 | `OPEN-QUESTIONS.md` | `docs/product/OPEN-QUESTIONS.md` | `PRODUCT-BASELINE-v1.0.0.md` §4, §15 | Bảng câu hỏi mở cũ; toàn bộ đã được biểu quyết và chốt phương án. |

---

## 3. Liên Kết Quay Về Tài Liệu Hoạt Động

- **Mục Lục Trung Tâm**: [docs/00-index.md](../../00-index.md)
- **Quy Chế Quản Trị Dự Án**: [docs/PROJECT-GOVERNANCE.md](../../PROJECT-GOVERNANCE.md)
- **Bộ Cơ Sở Đặc Tả Sản Phẩm Duy Nhất**: [docs/PRODUCT-BASELINE-v1.0.0.md](../../PRODUCT-BASELINE-v1.0.0.md)
