# Yêu Cầu Phi Chức Năng Của Sản Phẩm (NFR) — SmartRestaurant
## (Product Non-Functional Requirements — NFR)

- **Phiên Bản Tài Liệu**: 1.0.0
- **Trạng Thái Tài Liệu**: `CONFIRMED` (Chủ Dự án đã phê duyệt chính thức trong Task 02.6)
- **Tài Liệu Liên Quan**: [PRD.md](./PRD.md), [BUSINESS-REQUIREMENTS.md](../99-archive/stage-1-history/superseded-documents/BUSINESS-REQUIREMENTS.md), [MODULE-OVERVIEW.md](../99-archive/stage-1-history/superseded-documents/MODULE-OVERVIEW.md)

---

## 1. Bảo Mật & Ủy Quyền (Security & Authorization)

### NFR-SEC-01: Kiểm Soát Truy Cập Dựa Trên Vai Trò (RBAC) & Nguyên Tắc Đặc Quyền Tối Thiểu
- **Yêu cầu**: Hệ thống phải thực thi cơ chế phân quyền nghiêm ngặt dựa trên vai trò trên toàn bộ các endpoint giao diện và dịch vụ ứng dụng.
- **Các vai trò**: `SystemAdmin` (Quản trị hệ thống), `RestaurantManager` (Quản lý nhà hàng), `FloorCaptain` (Trưởng ca phục vụ), `Waiter` (Nhân viên phục vụ), `Chef` (Đầu bếp), `Cashier` (Thu ngân), `Stockkeeper` (Thủ kho).
- **Thực thi**: Phục vụ không thể xem tổng doanh thu hoặc báo cáo tài chính; thu ngân không thể chỉnh sửa công thức món hoặc thuế suất chi nhánh; đầu bếp không thể hủy hóa đơn.

### NFR-SEC-02: Xác Thực Ủy Quyền Bằng Mã PIN Của Quản Lý
- **Yêu cầu**: Các thao tác vận hành có rủi ro cao (hủy hóa đơn, hủy món sau khi đã bắn bếp, chiết khấu thủ công và duyệt chênh lệch két tiền chốt ca) bắt buộc phải yêu cầu nhập mã PIN bảo mật 4 chữ số của Quản lý.
- **Tính kiểm toán**: Mỗi lần xác thực PIN phải tự động lưu ID Quản lý phê duyệt, dấu ấn thời gian, thực thể bị tác động và lý do giải trình vào bảng kiểm toán bất biến.

### NFR-SEC-03: Phòng Chống Các Lỗ Hổng Web Phổ Biến (OWASP Top 10)
- **Chống CSRF**: Bắt buộc xác thực token chống giả mạo (`[ValidateAntiForgeryToken]`) trên toàn bộ các yêu cầu HTTP làm thay đổi dữ liệu (POST, PUT, DELETE).
- **Chống SQL Injection**: Sử dụng 100% truy vấn được tham số hóa thông qua Entity Framework Core LINQ. Nghiêm cấm hoàn toàn việc nối chuỗi câu lệnh SQL thô.
- **Chống XSS**: Tự động mã hóa ký tự HTML trên các view Razor và escape chuỗi an toàn phía client trên canvas Konva.js.

---

## 2. Phân Lập Dữ Liệu Đa Khách Thuê & Đa Chi Nhánh (Multi-Tenant & Multi-Branch Data Isolation)

### NFR-ISO-01: Phân Vùng Dữ Liệu Logic Theo Chi Nhánh & Doanh Nghiệp (F-01 Resolution)
- **Định hướng kiến trúc (`OD-01`)**: MVP triển khai cho một nhà hàng độc lập tại một địa điểm kinh doanh vật lý duy nhất. Cấu trúc dữ liệu được chuẩn bị sẵn sàng cho khả năng mở rộng đa chi nhánh trong tương lai.
- **Ranh giới quản trị**: Không có yêu cầu bắt buộc mọi database entity phải chứa cả hai trường `OrganizationId` và `BranchId`. Việc phân vùng dữ liệu, ownership và cơ chế cô lập dữ liệu (ví dụ: Global Query Filters vs Composite Keys) theo từng nhóm entity là nội dung kỹ thuật cần được đánh giá ở Stage 2 (`TECH-06`). Không đưa ra quyết định database architecture trong tài liệu sản phẩm này.

---

## 3. Mục Tiêu Hiệu Năng & Độ Trễ (Performance & Latency Targets)

| Mã Định Danh | Phạm Vi Vận Hành | Độ Trễ / Thông Lượng Mục Tiêu | Giả Định & Phương Pháp Kiểm Chứng |
| :--- | :--- | :--- | :--- |
| **NFR-PERF-01** | Độ phản hồi giao diện POS cảm ứng | $\le 100\text{ ms}$ cho thao tác chạm chọn món, mở hộp thoại tùy chọn và cập nhật giỏ hàng. | Đo kiểm trên máy tính bảng Android/iPad chuẩn qua mạng Wi-Fi 5GHz nội bộ. |
| **NFR-PERF-02** | Độ trễ phát sóng đơn SignalR từ POS sang KDS | $\le 500\text{ ms}$ từ lúc phục vụ bấm "Báo bếp" đến khi thẻ món xuất hiện trên màn hình KDS. | Đo kiểm trong môi trường mạng LAN nhà hàng thông thường với $\le 50$ thiết bị kết nối đồng thời. |
| **NFR-PERF-03** | Engine tính tiền & thuế phí | $\le 50\text{ ms}$ để tính toán xong tiền tạm tính, voucher, phí dịch vụ và thuế GTGT. | Tính toán trong bộ nhớ (In-memory) cho các hóa đơn có tối đa 100 dòng món ăn. |
| **NFR-PERF-04** | Đồng bộ trạng thái bàn trên sơ đồ mặt bằng | $\le 300\text{ ms}$ để cập nhật đổi màu huy hiệu trạng thái bàn trên toàn bộ các thiết bị. | Phát sóng trạng thái thị giác tức thì qua SignalR `TableHub`. |
| **NFR-PERF-05** | Tải cao điểm giờ cao điểm (F-03 Resolution) | Chịu tải đồng thời lên đến $50$ đơn gọi món cùng lúc và $200$ bàn hoạt động trên mỗi chi nhánh mà không bị suy giảm hiệu năng. | *Mục tiêu thử nghiệm áp lực (Stress-test Target)*: Giao cho Solution Architect xây dựng kịch bản kiểm thử tải (k6/NBomber) trong Stage 2 để xác định ngưỡng tài nguyên phần cứng phù hợp, tránh áp đặt thành ràng buộc cứng gây lãng phí tài nguyên (over-engineering). |

---

## 4. Kiểm Soát Đồng Thời & Toàn Vẹn Dữ Liệu (Concurrency & Data Consistency)

### NFR-CONC-01: Kiểm Soát Đồng Thời Lạc Quan (Optimistic Concurrency Control)
- **Yêu cầu**: Toàn bộ các thực thể nhạy cảm có khả năng bị sửa đổi đồng thời (`Table`, `DiningSession`, `Order`, `Invoice`, `Shift`) phải chứa token đồng thời dạng mảng byte (`RowVersion` / `ConcurrencyStamp`).
- **Xử lý xung đột**: Nếu hai nhân viên cùng cố gắng cập nhật một phiên bàn tại cùng một thời điểm (ví dụ: phục vụ đang thêm món trong khi thu ngân bấm yêu cầu thanh toán), giao dịch thứ hai sẽ bị từ chối với ngoại lệ xung đột đồng thời rõ ràng, buộc giao diện phải tải lại dữ liệu mới nhất thay vì âm thầm ghi đè dữ liệu.

### NFR-CONC-02: Ranh Giới Giao Dịch Nguyên Tử (Atomic Transaction Boundaries)
- **Yêu cầu**: Thao tác báo bếp, chuyển bàn, gộp bàn, tách hóa đơn và chốt ca bắt buộc phải thực thi bên trong các giao dịch cơ sở dữ liệu nguyên tử (`IDbContextTransaction`).
- **Cam kết Được-Ăn-Cả-Ngã-Về-Không**: Nếu một bước ghi CSDL bị thất bại khi báo bếp (ví dụ: tạo phiếu món thành công nhưng khóa giữ kho thất bại), toàn bộ giao dịch phải được tự động hoàn tác (rollback), ngăn ngừa việc sinh ra các phiếu món mồ côi hoặc làm sai lệch tài khoản hóa đơn.

---

## 5. Tính Bất Biến Khi Thực Thi Lại & Khả Năng Chịu Lỗi (Idempotency & Fault Tolerance)

### NFR-IDEM-01: Tính Bất Biến Của Thao Tác Bắn Đơn & Webhook Thanh Toán
- **Yêu cầu**: Hệ thống phải thực thi khóa bất biến (Idempotency Key) trên các yêu cầu gửi đơn gọi món và các webhook xử lý thanh toán.
- **Chặn đơn trùng lặp**: Nếu nhân viên phục vụ chạm nút "Báo bếp" nhiều lần do màn hình cảm ứng lag hoặc mạng Wi-Fi tự động thử lại, backend sẽ kiểm tra token bất biến và bỏ qua các yêu cầu trùng lặp, đảm bảo nhà bếp không bao giờ nhận trùng 2 phiếu chế biến cho cùng 1 đơn món.

### NFR-IDEM-02: Tự Động Kết Nối Lại SignalR & Tái Đồng Bộ Trạng Thái
- **Yêu cầu**: Tablet phục vụ tiền sảnh và màn hình bếp KDS phải tích hợp cơ chế tự động kết nối lại SignalR với thuật toán dãn cách lũy thừa (0s, 2s, 5s, 10s).
- **Bắt tay đồng bộ lại**: Khi kết nối lại thành công, client tự động gửi yêu cầu `SyncStateRequest(LastKnownTimestamp)` để lấy về toàn bộ các thay đổi trạng thái bàn hoặc thẻ món bếp bị lỡ trong khoảng thời gian mất mạng.

---

## 6. Nhật Ký Kiểm Toán & Tuân Thủ (Audit Logging & Compliance)

### NFR-AUD-01: Dấu Vết Kiểm Toán An Ninh Không Thể Chỉnh Sửa
- **Yêu cầu**: Toàn bộ các hành động mang tính hủy hoại, có tính chất giám sát hoặc liên quan đến tài chính bắt buộc phải sinh ra một bản ghi nhật ký kiểm toán bất biến bao gồm:
  - Dấu ấn thời gian (UTC + độ lệch giờ địa phương).
  - ID người thực hiện, Họ tên người dùng và Vai trò vận hành.
  - Địa chỉ IP thiết bị client và dấu ấn vân tay thiết bị.
  - Phân loại hành động: `BillVoid` (Hủy hóa đơn), `PostFireCancellation` (Hủy món sau nấu), `TableMerge` (Gộp bàn), `ManualDiscount` (Chiết khấu thủ công), `DrawerDiscrepancyOverride` (Duyệt chênh lệch két).
  - Trạng thái cũ vs Trạng thái mới (dữ liệu chênh lệch JSON delta).
  - Mã lý do giải trình bắt buộc và ghi chú bổ sung tùy chọn.

---

## 7. Khả Năng Sử Dụng & Quy Chuẩn Thiết Bị (Usability & Form Factors)

### NFR-USE-01: Tối Ưu Hóa Theo Dạng Thiết Bị
- **Bố cục giao diện đáp ứng (Responsive Layouts)**:
  - **Thiết bị thu ngân để bàn**: Tối ưu cho màn hình máy tính để bàn ($1920 \times 1080$ hoặc $1366 \times 768$) kèm phím tắt bàn phím (F1-F12), đầu đọc mã vạch và máy in nhiệt hóa đơn.
  - **Tablet phục vụ POS**: Tối ưu cho máy tính bảng cảm ứng 10-inch đến 12-inch ($1920 \times 1200$ hoặc $1024 \times 768$) đặt ở chế độ màn hình ngang. Kích thước vùng chạm tối thiểu: $48 \times 48\text{ px}$ với độ tương phản thị giác cao.
  - **Màn hình KDS nhà bếp**: Tối ưu cho màn hình cảm ứng chuyên dụng 15-inch đến 24-inch hoặc màn hình TV treo tường dùng bàn phím bấm số (Bump bar). Giao diện nền tối (Dark Theme) tương phản cao giúp nhìn rõ từ khoảng cách 2 mét trong môi trường bếp nhiều khói dầu và lóa sáng.

### NFR-USE-02: Khả Năng Tiếp Cận & Phản Hồi Âm Thanh - Hình Ảnh
- Đồng hồ đếm thời gian SLA theo màu sắc trực quan phải đi kèm âm thanh chuông báo êm dịu, dễ nhận biết khi có thẻ món mới, món ưu tiên gấp và món bị hủy để đầu bếp nhận biết ngay cả khi đang quay lưng về phía màn hình.

---

## 8. Bản Địa Hóa & Quy Chuẩn Khu Vực Việt Nam (Vietnamese-First)

### NFR-LOC-01: Trải Nghiệm Người Dùng Tiếng Việt 100% (CONFIRMED)
- **Quy định bắt buộc**: Toàn bộ tầng trình diễn trên mọi giao diện người dùng (Cổng quản trị, Tablet POS phục vụ, Màn hình bếp KDS, Quầy pha chế, Quầy thu ngân và giao diện QR khách hàng) phải hiển thị 100% bằng tiếng Việt tự nhiên, chuyên nghiệp.
- **Phạm vi**:
  - Toàn bộ menu điều hướng, biểu mẫu nhập liệu, nút bấm, nhãn hiển thị, gợi ý (placeholders) và tiêu đề bảng dữ liệu.
  - Toàn bộ thông báo toast SweetAlert2, hộp thoại xác nhận, trạng thái rỗng và thông báo lỗi hợp lệ hóa.
  - Toàn bộ các trạng thái vận hành của bàn và nhà bếp (ví dụ: `Trống`, `Có khách`, `Đã đặt trước`, `Đang thanh toán`, `Cần dọn dẹp`, `Chờ chế biến`, `Đang nấu`, `Đã xong - Chờ bưng`).
  - Toàn bộ các ấn phẩm in ấn (Phiếu tạm tính, Hóa đơn thanh toán, Báo cáo chốt ca Z-Report).
- **Tiêu chuẩn chất lượng**: Không có bất kỳ từ tiếng Anh chưa dịch nào; tuân thủ nghiêm ngặt [VIETNAMESE-LOCALIZATION-STANDARD.md](./VIETNAMESE-LOCALIZATION-STANDARD.md).

### NFR-LOC-02: Phân Tách Kỹ Thuật & Sẵn Sàng Đa Ngôn Ngữ Tương Lai
- **Tách biệt mối quan tâm (SoC)**: Chuỗi hiển thị cho người dùng phải được phân tách độc lập khỏi mã nguồn ứng dụng và logic nghiệp vụ. Tuyệt đối không hard-code chuỗi ký tự tiếng Việt thô vào các domain service hoặc controller C#.
- **Tệp tài nguyên Resource Bundles**: Chuỗi hiển thị được quản lý tập trung thông qua các tệp tài nguyên `.resx` kiểu mạnh trong ASP.NET Core hoặc từ điển i18n JSON trên frontend.
- **Quy chuẩn kỹ thuật bằng tiếng Anh**: Các thực thể miền (Domain Entities), bảng CSDL, tên cột, enums (`TableState.Occupied`), API endpoints và commit log Git duy trì bằng tiếng Anh chuẩn để tuân thủ kiến trúc Clean Architecture.
- **Khả năng mở rộng**: Hệ thống mặc định văn hóa `vi-VN` (`CultureInfo("vi-VN")`), nhưng cấu trúc sẵn sàng cho phép bổ sung các ngôn ngữ phụ (Tiếng Anh, Tiếng Hàn, Tiếng Nhật, Tiếng Trung) mà không cần tái cấu trúc logic nghiệp vụ.

### NFR-LOC-03: Định Dạng Vùng Việt Nam, Múi Giờ & Bộ Gõ Tiếng Việt
- **Tiền tệ**: Đồng Việt Nam (`₫` / `VND`), định dạng dấu chấm phân cách hàng nghìn (`#,,### ₫`), không dùng số thập phân lẻ.
- **Ngày & Giờ**: Ngày định dạng `dd/MM/yyyy`, giờ định dạng 24h `HH:mm:ss`.
- **Múi giờ**: Múi giờ mặc định cố định `Asia/Ho_Chi_Minh` (UTC+7). CSDL lưu trữ thời gian UTC và hiển thị giờ địa phương Việt Nam.
- **Bộ gõ & Mã hóa tiếng Việt**: Hỗ trợ đầy đủ UTF-8 trên Web, API và SQL Server (`NVARCHAR` với collation hỗ trợ tiếng Việt). Tương thích mượt mà với cả 2 kiểu gõ Telex và VNI trên cả thiết bị di động và máy tính để bàn.

### NFR-LOC-04: Khả Năng Sử Dụng Cho Nhân Viên Nhà Hàng Phổ Thông
- **Đơn giản hóa**: Nhãn tiếng Việt ngắn gọn, súc tích; tránh tiếng lóng hoặc thông báo lỗi kỹ thuật khó hiểu.
- **Công thái học cảm ứng**: Kích thước nút bấm tối thiểu $\ge 48 \times 48\text{ px}$ với độ tương phản cao, phù hợp thao tác nhanh trong môi trường dầu mỡ hoặc ánh sáng mờ.
- **Biểu tượng trực quan**: Kết hợp chữ viết tiếng Việt cùng biểu tượng đồ họa quen thuộc (ví dụ: biểu tượng máy in cho "In tạm tính", biểu tượng mũi tên cho "Chuyển bàn").

---

## 9. Sao Lưu, Phục Hồi & Duy Trì Vận Hành Liên Tục (Backup & Operational Continuity)

### NFR-BCP-01: Kế Hoạch Khôi Phục Sau Thảm Họa & Sao Lưu Tự Động
- **Sao lưu cơ sở dữ liệu**:
  - Sao lưu toàn phần (Full backup) hàng ngày + Sao lưu vi sai (Differential) hàng giờ + Sao lưu nhật ký giao dịch (Transaction log) mỗi 15 phút trong giờ mở cửa.
  - Mục tiêu điểm khôi phục dữ liệu (RPO): $\le 15\text{ phút}$.
  - Mục tiêu thời gian khôi phục hoạt động (RTO): $\le 2\text{ giờ}$.
- **Thay thế phần cứng khi hỏng hóc**: Nếu một máy tính bảng phục vụ hoặc màn hình KDS bị hỏng vật lý, nhân viên có thể lấy ngay một máy tính bảng dự phòng, truy cập đường dẫn web, nhập mã PIN nhân viên và tiếp tục làm việc ngay lập tức mà không bị mất bất kỳ dữ liệu cục bộ nào.
