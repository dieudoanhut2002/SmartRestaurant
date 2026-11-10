# SmartRestaurant — Mục Lục Tài Liệu Dự Án (Documentation Index)

Chào mừng bạn đến với mục lục tài liệu tập trung của dự án **SmartRestaurant**. Toàn bộ các đặc tả kỹ thuật, yêu cầu nghiệp vụ, thiết kế kiến trúc và sổ tay vận hành chính thức đều được tổ chức phân cấp tại đây.

---

## 1. Quản Trị Dự Án & Bộ Quy Tắc Chỉ Thị

| Tài liệu | Mô tả | Phạm vi |
| :--- | :--- | :--- |
| **[GEMINI.md](../GEMINI.md)** | Bộ quy tắc dự án có thẩm quyền cao nhất, quản trị kỹ thuật, chiến lược bộ nhớ và giao thức an toàn. | Quản trị Lõi Dự Án |
| **[AGENTS.md](../AGENTS.md)** | Chuẩn mực kỹ nghệ, quy tắc kiến trúc sạch (Clean Architecture), quy ước viết mã và chỉ thị đầu ra. | Quy chuẩn Kỹ nghệ & Chất lượng |

---

## 2. Khám Phá Sản Phẩm & Đặc Tả Nghiệp Vụ (Giai Đoạn 1 - Stage 1)

### 2.1. Bộ Hồ Sơ Cơ Sở Đặc Tả Sản Phẩm Chính Thức (Product Specification Baseline Suite — v1.0.0-FROZEN)
| Tài liệu | Tiêu đề | Trạng thái | Mô tả |
| :--- | :--- | :--- | :--- |
| **[docs/product/PRODUCT-SPECIFICATION-BASELINE.md](./product/PRODUCT-SPECIFICATION-BASELINE.md)** | Hồ Sơ Cơ Sở Đặc Tả Sản Phẩm Chính Thức (Master Baseline) | `CONFIRMED & SEALED` | Điểm quy chiếu trung tâm, tuyên ngôn niêm phong v1.0.0-FROZEN, quản trị thay đổi (CR) và mục lục cơ sở. |
| **[docs/product/MVP-SCOPE-BASELINE.md](./product/MVP-SCOPE-BASELINE.md)** | Cơ Sở Phạm Vi MVP & Lộ Trình Phát Hành | `CONFIRMED & SEALED` | Phạm vi MVP Core (10 phân hệ), MVP Optional (4 phân hệ), phân kỳ Post-MVP (v1.1, v1.2+, v2.0) và loại trừ phạm vi. |
| **[docs/product/BUSINESS-REQUIREMENTS-BASELINE.md](./product/BUSINESS-REQUIREMENTS-BASELINE.md)** | Cơ Sở Yêu Cầu Nghiệp Vụ Thống Nhất | `CONFIRMED & SEALED` | Danh mục 56 yêu cầu nghiệp vụ chi tiết thuộc 11 miền chức năng, có mã định danh và nguồn gốc rõ ràng. |
| **[docs/product/BUSINESS-RULES-BASELINE.md](./product/BUSINESS-RULES-BASELINE.md)** | Cơ Sở Quy Tắc Nghiệp Vụ & Bất Biến Vận Hành | `CONFIRMED & SEALED` | Bộ 18 quy tắc nghiệp vụ bất biến (`BR-RULE-01..18`), 15 bất biến cốt lõi (`INV-01..15`) và kết quả kiểm toán 6 miền. |
| **[docs/product/MODULE-CAPABILITY-BASELINE.md](./product/MODULE-CAPABILITY-BASELINE.md)** | Cơ Sở Phân Hệ & Bản Đồ Năng Lực Nghiệp Vụ | `CONFIRMED & SEALED` | Ranh giới 14 phân hệ, bản đồ 14 năng lực nghiệp vụ (Capability Map), phụ thuộc 1 chiều và Graceful Drain (`OD-17`). |
| **[docs/product/REQUIREMENT-TRACEABILITY-MATRIX.md](./product/REQUIREMENT-TRACEABILITY-MATRIX.md)** | Ma Trận Truy Xuất Nguồn Gốc Yêu Cầu Hai Chiều | `CONFIRMED & SEALED` | Ma trận truy xuất nguồn gốc 12 cột, ánh xạ từ Nhu cầu $\rightarrow$ Yêu cầu $\rightarrow$ Quy tắc $\rightarrow$ PDR $\rightarrow$ Kiến trúc Stage 2. |
| **[docs/product/OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md](./product/OPEN-QUESTIONS-AND-DEFERRED-REGISTER.md)** | Sổ Đăng Ký Câu Hỏi Mở & Yêu Cầu Hoãn | `CONFIRMED & SEALED` | Sổ quản lý vòng đời toàn bộ quyết định (`OD-14..17`), câu hỏi đã chốt (`OQ-01..07`), yêu cầu hoãn và chủ đề Stage 2. |
| **[docs/product/STAGE-2-PRODUCT-HANDOFF.md](./product/STAGE-2-PRODUCT-HANDOFF.md)** | Hồ Sơ Bàn Giao Thiết Kế Kỹ Thuật Giai Đoạn 2 | `CONFIRMED & SEALED` | Hồ sơ bàn giao Solution Architect: Bối cảnh nghiệp vụ, NFRs, ràng buộc bất biến và 13 chủ đề kỹ thuật điều tra. |
| **[docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md](./product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-REPORT.md)** | Báo Cáo Thực Hiện Hợp Nhất Cơ Sở Đặc Tả (Task 02.9) | `CONFIRMED & SEALED` | Báo cáo kiểm toán, đối soát Git baseline, kiểm toán nhất quán quy tắc và xác nhận ranh giới Cổng Stage 2 (NOT AUTHORIZED / LOCKED). |
| **[docs/product/TASK-02.9.1-INDEPENDENT-PRODUCT-SPECIFICATION-AUDIT.md](./product/TASK-02.9.1-INDEPENDENT-PRODUCT-SPECIFICATION-AUDIT.md)** | Báo Cáo Kiểm Toán Độc Lập Cơ Sở Đặc Tả Sản Phẩm (Task 02.9.1) | `CONFIRMED & AUDITED` | Kiểm toán độc lập 100% (10 tiêu chí), thẩm định tính truy vết, bảo toàn D1-D5, NFRs, và xác nhận 4 trạng thái quản trị. |
| **[docs/product/TASK-02.9.2-GOVERNANCE-FINDING-RESOLUTION-REPORT.md](./product/TASK-02.9.2-GOVERNANCE-FINDING-RESOLUTION-REPORT.md)** | Báo Cáo Giải Quyết Toàn Diện Phát Hiện Kiểm Toán Quản Trị (Task 02.9.2) | `CONFIRMED & SEALED` | Giải quyết dứt điểm 4 phát hiện (F-01..04), hiệu chỉnh có kiểm soát bộ cơ sở, xác nhận Clean Pass và niêm phong Giai đoạn 1. |
| **[docs/product/TASK-02.9.2.1-INDEPENDENT-GOVERNANCE-INTEGRITY-VERIFICATION.md](./product/TASK-02.9.2.1-INDEPENDENT-GOVERNANCE-INTEGRITY-VERIFICATION.md)** | Báo Cáo Kiểm Toán Độc Lập Tính Toàn Vẹn Quản Trị (Task 02.9.2.1) | `CONFIRMED & CERTIFIED` | Thẩm định thực chứng độc lập 10/10 tiêu chí (F-01..04, D1..D5, REV-11, FOH), xác nhận Stage 1 Owner-Approved và Stage 2 Locked. |
| **[docs/product/TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-TRACEABILITY-REPORT.md](./product/TASK-02.8.6.1-GOVERNANCE-CORRECTION-AND-TRACEABILITY-REPORT.md)** | Báo Cáo Hiệu Chỉnh Quản Trị & Truy Nguyên Quyết Định (Task 02.8.6.1) | `CONFIRMED & SEALED` | Xử lý dứt điểm 4 sai lệch quản trị, đối soát truy nguyên OD-14/OD-13, chuẩn hóa Preset OD-16 theo REV-06 & OQ-02, rào giậu kỹ thuật OQ-08/10, và niêm phong Stage 2 Locked. |
| **[docs/product/TASK-02.9.3-FINAL-STAGE-1-GOVERNANCE-AND-BASELINE-INTEGRITY-AUDIT.md](./product/TASK-02.9.3-FINAL-STAGE-1-GOVERNANCE-AND-BASELINE-INTEGRITY-AUDIT.md)** | Báo Cáo Kiểm Toán Độc Lập Toàn Diện Cuối Cùng (Task 02.9.3) | `CONFIRMED & AUDITED` | Kiểm toán pháp y toàn diện 70 tệp, phán quyết PASS WITH FINDINGS, bảo toàn 23 quyết định của Owner, đề xuất đóng hồ sơ Stage 1. |
| **[docs/product/TASK-02.9.3.1-FINAL-AUDIT-FINDING-CLARIFICATION-AND-EVIDENCE-REPORT.md](./product/TASK-02.9.3.1-FINAL-AUDIT-FINDING-CLARIFICATION-AND-EVIDENCE-REPORT.md)** | Báo Cáo Làm Rõ Phát Hiện Kiểm Toán & Thẩm Định Bằng Chứng (Task 02.9.3.1) | `CONFIRMED & SEALED` | Thẩm định thực chứng 7 vấn đề trọng yếu, làm rõ ranh giới Owner approval vs AI proposal, xác nhận 8 commits và zero code leakage. |
| **[docs/product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md](./product/TASK-03.0-STAGE-1-DOCUMENT-CONSOLIDATION-AND-ARCHIVE-PLAN.md)** | Kế Hoạch Hợp Nhất & Lưu Trữ Hồ Sơ Giai Đoạn 1 (Task 03.0) | `PROPOSED PLAN` | Quy hoạch lưu trữ 24 tệp lịch sử, bảo tồn 100% bằng chứng, ma trận dịch chuyển 1:1, bảo toàn 191 liên kết và script kiểm thử tự động. |

### 2.2. Bộ Tài Liệu Đặc Tả Sản Phẩm Thành Phần (Component Product Specifications)
| Tài liệu | Tiêu đề | Trạng thái | Mô tả |
| :--- | :--- | :--- | :--- |
| **[docs/product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md](./product/FINAL-GOVERNANCE-TRACEABILITY-AUDIT.md)** | Báo Cáo Kiểm Toán Tổng Thể Tính Truy Vết Quản Trị & Ranh Giới Phê Duyệt | `CONFIRMED` | Kiểm toán tuân thủ toàn diện Stage 1, danh mục 10 điểm kiểm soát người gác cổng, và ranh giới chuyển tiếp Stage 2. |
| **[docs/product/PRD.md](./product/PRD.md)** | Tài Liệu Yêu Cầu Sản Phẩm (PRD) | `CONFIRMED` | Tầm nhìn sản phẩm, đối tượng người dùng, mục tiêu kinh doanh, chỉ số KPI và phạm vi giải pháp. |
| **[docs/product/PRODUCT-REVIEW-REPORT.md](./product/PRODUCT-REVIEW-REPORT.md)** | Báo Cáo Đánh Giá & Thẩm Định Nghiệp Vụ Chuyên Sâu | `CONFIRMED` | Phản biện độc lập, 4 vấn đề CRITICAL, 6 vấn đề HIGH, rà soát 12 kịch bản vận hành thực tế. |
| **[docs/product/PRODUCT-REVIEW-VALIDATION.md](./product/PRODUCT-REVIEW-VALIDATION.md)** | Báo Cáo Thẩm Định & Đánh Giá Sản Phẩm | `CONFIRMED` | Kiểm toán tính nhất quán chéo giữa các tài liệu, thử nghiệm áp lực đối tượng vận hành, và mức độ sẵn sàng Stage 2. |
| **[docs/product/VIETNAMESE-LOCALIZATION-STANDARD.md](./product/VIETNAMESE-LOCALIZATION-STANDARD.md)** | Tiêu Chuẩn Bản Địa Hóa Tiếng Việt (Vietnamese-First) | `CONFIRMED` | Từ điển thuật ngữ chuẩn mực, định dạng vùng miền (VNĐ, ngày tháng), in ấn và chuẩn giao diện người dùng. |
| **[docs/product/BUSINESS-REQUIREMENTS.md](./product/BUSINESS-REQUIREMENTS.md)** | Đặc Tả Yêu Cầu Nghiệp Vụ (BRS) | `CONFIRMED` | Yêu cầu chức năng chi tiết, các tác nhân vận hành, 16 quy tắc nghiệp vụ cốt lõi và luồng ngoại lệ. |
| **[docs/product/MVP-SCOPE.md](./product/MVP-SCOPE.md)** | Phạm Vi MVP & Lộ Trình Phát Hành | `CONFIRMED` | Ma trận ưu tiên tính năng MoSCoW, các loại trừ phạm vi rõ ràng và các mốc phát hành phiên bản. |
| **[docs/product/DOMAIN-GLOSSARY.md](./product/DOMAIN-GLOSSARY.md)** | Thuật Ngữ Miền Phổ Quát (Ubiquitous Glossary) | `CONFIRMED` | Bảng từ vựng vận hành chuẩn mực song ngữ Anh - Việt cho toàn bộ hệ thống. |
| **[docs/product/USER-JOURNEYS.md](./product/USER-JOURNEYS.md)** | Hành Trình Người Dùng Toàn Diện (User Journeys) | `CONFIRMED` | 10 hành trình khép kín của các đối tượng tác nhân bao quát toàn bộ hoạt động hàng ngày của nhà hàng. |
| **[docs/product/OPEN-QUESTIONS.md](./product/OPEN-QUESTIONS.md)** | Danh Mục Câu Hỏi Mở & Nhật Ký Quyết Định | `RESOLVED & CONSOLIDATED` | Toàn bộ câu hỏi mở MVP đã được Chủ Dự Án giải quyết và phê chuẩn chính thức. |
| **[docs/product/MODULE-OVERVIEW.md](./product/MODULE-OVERVIEW.md)** | Tổng Quan & Ranh Giới Các Module Nghiệp Vụ | `CONFIRMED` | Ranh giới 14 module nghiệp vụ cốt lõi, thực thể dữ liệu và quan hệ phụ thuộc. |
| **[docs/product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md](./product/MODULE-CONFIGURATION-AND-FEATURE-MANAGEMENT.md)** | Phân Tích Cấu Hình Module & Quản Lý Tính Năng Nghiệp Vụ | `CONFIRMED` | Phân tầng phạm vi cấu hình 7 cấp, danh mục tham số tổng thể (14 module), ma trận tính năng và quy tắc kiểm toán. |
| **[docs/product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md](./product/MODULE-GOVERNANCE-RECONCILIATION-AUDIT.md)** | Báo Cáo Đối Soát & Kiểm Toán Quản Trị Module | `CONFIRMED` | Kiểm toán nhất quán chéo, xác minh hạ cấp vận hành êm dịu và đối soát phạm vi MVP đơn điểm. |
| **[docs/product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md](./product/OWNER-DECISION-DOSSIER-AND-STAGE-2-READINESS.md)** | Hồ Sơ Quyết Định Của Chủ Nhà Hàng & Đánh Giá Độ Sẵn Sàng Stage 2 | `CONFIRMED` | Hồ sơ tổng hợp các quyết định mở của Chủ nhà hàng (OD-14..17, OQ-01..07), bảng điểm sẵn sàng 6 chiều và WBS TASK-03. |
| **[docs/product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md](./product/FINAL-EVIDENCE-RECONCILIATION-REPORT.md)** | Báo Cáo Đối Soát Bằng Chứng Cuối Cùng & Xác Minh Ranh Giới Quản Trị | `CONFIRMED` | Đối soát bằng chứng vật lý, xác minh không rò rỉ mã nguồn, chứng minh bộ nhớ nguyên vẹn và kiểm tra ủy quyền Stage 2. |
| **[docs/product/TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md](./product/TASK-02.8.2.2-CONTROLLED-GOVERNANCE-CORRECTIONS-REPORT.md)** | Báo Cáo Hiệu Chỉnh Quản Trị Có Kiểm Soát | `CONFIRMED` | Báo cáo thực hiện các hiệu chỉnh từ COR-01 đến COR-06 giải quyết dứt điểm các sai lệch quản trị. |
| **[docs/product/TASK-02.8.2.3-INDEPENDENT-POST-CORRECTION-VERIFICATION.md](./product/TASK-02.8.2.3-INDEPENDENT-POST-CORRECTION-VERIFICATION.md)** | Báo Cáo Kiểm Toán Độc Lập Sau Hiệu Chỉnh | `CONFIRMED` | Kiểm toán độc lập 100% từ AUDIT-01 đến AUDIT-10 chứng nhận niêm phong quản trị Stage 1. |
| **[docs/product/TASK-02.8.3-VIETNAMESE-LOCALIZATION-REPORT.md](./product/TASK-02.8.3-VIETNAMESE-LOCALIZATION-REPORT.md)** | Báo Cáo Việt Hóa Toàn Diện Tài Liệu Sản Phẩm (Task 02.8.3) | `CONFIRMED` | Báo cáo kiểm toán, đối soát và hoàn tất Việt hóa có kiểm soát toàn bộ tài liệu Markdown Stage 1. |
| **[docs/product/TASK-02.8.4-INDEPENDENT-VIETNAMESE-QUALITY-VERIFICATION.md](./product/TASK-02.8.4-INDEPENDENT-VIETNAMESE-QUALITY-VERIFICATION.md)** | Báo Cáo Kiểm Tra Độc Lập Chất Lượng Việt Hóa Tài Liệu (Task 02.8.4) | `CONFIRMED` | Thẩm định chất lượng độc lập 8 tiêu chí kiểm toán, bảo toàn 100% ý nghĩa nghiệp vụ và ranh giới quản trị Stage 1. |
| **[docs/product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md](./product/TASK-02.8.5-PROJECT-OWNER-DECISION-CONSOLIDATION-RECORD.md)** | Biên Bản Hợp Nhất Quyết Định Của Chủ Dự Án (Task 02.8.5) | `CONFIRMED` | Biểu quyết chính thức chốt 100% các quyết định mở (OD-14..17, OQ-01..07) và phê chuẩn Cơ cấu 14 Phân hệ. |
| **[docs/product/OWNER-DECISION-WORKSHOP.md](./product/OWNER-DECISION-WORKSHOP.md)** | Hồ Sơ Quyết Định Sản Phẩm Cho Chủ Dự Án (Workshop Dossier) | `CONFIRMED` | Bộ hồ sơ 8 quyết định mở (OD-14..17, OQ-01..07) kèm phiếu biểu quyết đã được Chủ Dự Án phê chuẩn chính thức. |
| **[docs/product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md](./product/TASK-02.8.6-OWNER-DECISION-IMPLEMENTATION-REPORT.md)** | Báo Cáo Triển Khai Quyết Định Của Chủ Dự Án Vào Tài Liệu Sản Phẩm (Task 02.8.6) | `CONFIRMED` | Triển khai chính thức quyết định của Chủ Dự Án vào hồ sơ quyết định và đồng bộ hóa có kiểm soát toàn bộ tài liệu đặc tả Stage 1. |
| **[docs/product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md](./product/TASK-02.9-PRODUCT-SPECIFICATION-BASELINE-CONSOLIDATION.md)** | Hồ Sơ Hợp Nhất Cơ Sở Đặc Tả Sản Phẩm Chính Thức (Task 02.9) | `CONFIRMED & SEALED` | Tài liệu cơ sở đặc tả sản phẩm Giai đoạn 1 niêm phong chính thức (v1.0.0-FROZEN); hợp nhất 14 phân hệ, 18 quy tắc, 15 bất biến, ma trận truy vết và niêm phong Cổng Stage 2 (NOT AUTHORIZED / LOCKED). |
| **[docs/product/NON-FUNCTIONAL-REQUIREMENTS.md](./product/NON-FUNCTIONAL-REQUIREMENTS.md)** | Yêu Cầu Phi Chức Năng (NFR) | `CONFIRMED` | Bảo mật, cô lập dữ liệu, chỉ tiêu hiệu năng và quy tắc xử lý đồng thời. |

### 2.3. Các Quy Trình Vận Hành Trọng Yếu (Operational Workflows)
| Tài liệu | Tiêu đề | Trạng thái | Phạm vi |
| :--- | :--- | :--- | :--- |
| **[docs/product/workflows/FLOOR-PLAN-WORKFLOW.md](./product/workflows/FLOOR-PLAN-WORKFLOW.md)** | Quy Trình Quản Lý Sơ Đồ Mặt Bằng & Bàn Ăn | `CONFIRMED` | Trạng thái bàn, sơ đồ Konva.js, chuyển bàn, gộp bàn, hủy gộp bàn, và đặt bao trọn phòng VIP. |
| **[docs/product/workflows/ORDER-WORKFLOW.md](./product/workflows/ORDER-WORKFLOW.md)** | Quy Trình Bán Hàng POS & Gọi Món | `CONFIRMED` | Gọi món cảm ứng, nhóm tùy chọn, đợt gọi món, báo bếp, làm lại món, và ủy quyền hủy món. |
| **[docs/product/workflows/KITCHEN-WORKFLOW.md](./product/workflows/KITCHEN-WORKFLOW.md)** | Quy Trình Vận Hành Bếp & Màn Hình KDS | `CONFIRMED` | Điều phối đa trạm, thẻ KDS, đồng hồ SLA, phiếu làm lại món, và nhịp độ bàn ra món expediter. |
| **[docs/product/workflows/CASHIER-PAYMENT-WORKFLOW.md](./product/workflows/CASHIER-PAYMENT-WORKFLOW.md)** | Quy Trình Thu Ngân, Tính Tiền & Thanh Toán | `CONFIRMED` | Tách hóa đơn không khóa bàn, VietQR đến muộn và sổ treo đối soát, báo cáo chốt ca Z-Report. |

### 2.4. Biên Bản Quyết Định Sản Phẩm (PDRs)
| Mã Quyết Định | Tài liệu | Trạng thái | Tóm tắt |
| :--- | :--- | :--- | :--- |
| **PDR-001** | [docs/product/decisions/PDR-001-modular-monolith-deployment-model.md](./product/decisions/PDR-001-modular-monolith-deployment-model.md) | `CONFIRMED` | Mô hình triển khai Modular Monolith với phân vùng dữ liệu đa tenant/chi nhánh từ đầu. |
| **PDR-002** | [docs/product/decisions/PDR-002-hierarchical-area-room-table-model.md](./product/decisions/PDR-002-hierarchical-area-room-table-model.md) | `PROPOSED` | Mô hình không gian phân cấp Khu vực $\rightarrow$ Phòng $\rightarrow$ Bàn (Yêu cầu nghiệp vụ: CONFIRMED). |
| **PDR-003** | [docs/product/decisions/PDR-003-independent-station-kitchen-routing.md](./product/decisions/PDR-003-independent-station-kitchen-routing.md) | `PROPOSED` | Điều phối trạm bếp động kèm màn hình Master Expediter tại bàn ra món (Yêu cầu nghiệp vụ: CONFIRMED). |
| **PDR-004** | [docs/product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md](./product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md) | `CONFIRMED` (Nghiệp vụ) | Khóa tồn kho 2 bước (`Tồn khả dụng = Tồn thực tế - Tạm giữ`) & Món đếm được trong MVP (Kỹ thuật: `PROPOSED`). |
| **PDR-005** | [docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md](./product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md) | `CONFIRMED` (Nghiệp vụ) | Quyết toán đa kênh kèm Tách theo món và Chia đều ($1/N$) (Kỹ thuật: `PROPOSED`). |
| **PDR-006** | [docs/product/decisions/PDR-006-vietnamese-language-ui.md](./product/decisions/PDR-006-vietnamese-language-ui.md) | `CONFIRMED` | Toàn bộ tầng trình diễn và giao diện người dùng hiển thị 100% bằng tiếng Việt. |
| **PDR-007** | [docs/product/decisions/PDR-007-business-date-shift-management.md](./product/decisions/PDR-007-business-date-shift-management.md) | `CONFIRMED` (Nghiệp vụ) | Áp dụng Ngày Kinh Doanh (Business Date) có thể cấu hình cho ca qua đêm (Kỹ thuật: `PROPOSED`). |
| **PDR-008** | [docs/product/decisions/PDR-008-vietqr-delayed-webhook-handling.md](./product/decisions/PDR-008-vietqr-delayed-webhook-handling.md) | `CONFIRMED` (Nghiệp vụ) | Xử lý webhook VietQR đến muộn, điều chuyển vào sổ treo và đối soát thủ công (Kỹ thuật: `PROPOSED`). |
| **PDR-009** | [docs/product/decisions/PDR-009-multi-bill-table-session-split-billing.md](./product/decisions/PDR-009-multi-bill-table-session-split-billing.md) | `CONFIRMED` (Nghiệp vụ) | Phiên bàn đa hóa đơn và chính sách tách hóa đơn không khóa bàn (Kỹ thuật: `PROPOSED`). |
| **PDR-010** | [docs/product/decisions/PDR-010-unmerge-tables.md](./product/decisions/PDR-010-unmerge-tables.md) | `CONFIRMED` (Nghiệp vụ) | Chính sách hủy gộp bàn và tái phân bổ món ăn (Kỹ thuật: `PROPOSED`). |
| **PDR-011** | [docs/product/decisions/PDR-011-area-and-vip-room-surcharges.md](./product/decisions/PDR-011-area-and-vip-room-surcharges.md) | `CONFIRMED` (Nghiệp vụ) | Phụ phí theo tỷ lệ phần trăm và số tiền cố định cho phòng VIP/khu vực (Kỹ thuật: `PROPOSED`). |
| **PDR-012** | [docs/product/decisions/PDR-012-direct-service-items-foh.md](./product/decisions/PDR-012-direct-service-items-foh.md) | `CONFIRMED` (Nghiệp vụ) | Điều phối mặt hàng phục vụ trực tiếp tại sảnh bỏ qua KDS Quầy Bar (Kỹ thuật: `PROPOSED`). |
| **PDR-013** | [docs/product/decisions/PDR-013-shared-room-and-entire-room-booking.md](./product/decisions/PDR-013-shared-room-and-entire-room-booking.md) | `CONFIRMED` (Nghiệp vụ) | Bàn độc lập trong phòng dùng chung và khóa bao trọn phòng (Kỹ thuật: `PROPOSED`). |
| **PDR-014** | [docs/product/decisions/PDR-014-concurrent-order-append-business-rules.md](./product/decisions/PDR-014-concurrent-order-append-business-rules.md) | `CONFIRMED` (Nghiệp vụ) | Nhân viên cùng gọi món đồng thời trên bàn chung không khóa chặn (Kỹ thuật: `PROPOSED`). |
| **PDR-015** | [docs/product/decisions/PDR-015-remake-dish-workflow.md](./product/decisions/PDR-015-remake-dish-workflow.md) | `CONFIRMED` (Nghiệp vụ) | Quy trình làm lại món do lỗi bếp không tính tiền khách & ghi sổ hủy hỏng (Giao diện: `PROPOSED`). |

---

## 3. Kiến Trúc Kỹ Thuật & Thiết Kế Giải Pháp (Giai Đoạn 2 - Sắp Tới)

| Tài liệu | Tiêu đề | Trạng thái | Giai đoạn mục tiêu |
| :--- | :--- | :--- | :--- |
| *docs/02-architecture/system-architecture.md* | Kiến Trúc Hệ Thống Modular Monolith & Thiết Kế Thành Phần | `PLANNED` | Giai đoạn 2 |
| *docs/02-architecture/data-models-and-erd.md* | Thiết Kế Lược Đồ Quan Hệ & Mô Hình Thực Thể ERD | `PLANNED` | Giai đoạn 2 |
| *docs/02-architecture/realtime-signalr-mesh.md* | Đặc Tả Các Hub SignalR Thời Gian Thực & Hợp Đồng Sự Kiện | `PLANNED` | Giai đoạn 2 |

---

## 4. Phân Loại Trạng Thái Quyết Định
- `CONFIRMED`: Đã được Chủ nhà hàng (Project Owner) phê duyệt chính thức.
- `PROPOSED`: Được Kiến trúc sư sản phẩm đề xuất, đang chờ ký duyệt.
- `OPEN QUESTION`: Đang trong quá trình đánh giá tích cực hoặc cần làm rõ thêm từ Chủ nhà hàng.
