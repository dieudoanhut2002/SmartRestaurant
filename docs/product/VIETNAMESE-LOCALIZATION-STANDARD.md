# Vietnamese-First Localization Standard — SmartRestaurant

- **Document Version**: 2.0.0
- **Document Status**: `CONFIRMED` (Mandated by Project Owner)
- **Primary Market**: Vietnam Food & Beverage (F&B) Industry
- **Owner**: Senior Product Architect & Business Analyst
- **Scope**: Presentation Layer, Client Applications, Hardware Printing, Regional Formatting, and Technical Localization Architecture

---

## 1. Vietnamese-First Product Language Policy

SmartRestaurant is designed, architected, and built with **Vietnamese as the primary, default, and initial production language** across the entire platform.

### 1.1. Core Directives
1. **100% Vietnamese User Experience**: All user-facing screens, controls, alerts, and printouts must be in natural, professional, grammatically correct Vietnamese.
2. **Elimination of Superfluous English**: FOH staff, line cooks, bussers, and cashiers must never encounter untranslated English text (e.g., "Submit", "Cancel", "Loading...", "Table Occupied", "Ready to serve", "Bad request").
3. **No Literal Machine Translations**: Use established Vietnamese hospitality industry vernacular that restaurant owners, managers, servers, and chefs naturally use every day (e.g., *"Báo bếp"* instead of *"Gửi yêu cầu chế biến"*; *"Chốt ca"* instead of *"Đóng phiên làm việc"*; *"Bàn ra món"* instead of *"Trạm tiếp nhận"*; *"Tạm hết món"* instead of *"Không khả dụng"*).
4. **Clean Technical Decoupling (i18n-Ready)**: While Vietnamese is the exclusive initial production language, application code must not hard-code Vietnamese strings in business logic. User-facing text must be separated into resource catalogs (`.resx`, JSON localization bundles) so secondary languages (e.g., English, Korean, Japanese, Chinese) can be introduced seamlessly in future releases without modifying business logic.

---

## 2. Standardized Vietnamese Business Terminology Glossary

To prevent inconsistent, confusing, or awkward wording across different screens, SmartRestaurant establishes this authoritative terminology standard.

### 2.1. Spatial & Floor Plan Terminology (Khu Vực & Bàn Ăn)

| Thuật ngữ Hiển thị Chuẩn | Ý nghĩa Nghiệp vụ | Ngữ cảnh Sử dụng | Thuật ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Khu vực** | Phân vùng không gian lớn trong nhà hàng (Tầng 1, Sân vườn, Tầng thượng). | Quản lý sơ đồ bàn, bộ lọc bàn trên POS. | *Vùng, Địa bàn* (Quá hành chính/cơ quan). |
| **Phòng ăn** | Không gian độc lập bên trong khu vực. | Sơ đồ tầng, đặt bàn tiệc. | *Buồng* (Cũ kỹ, thiếu thẩm mỹ F&B). |
| **Phòng VIP** | Phòng ăn riêng tư cao cấp có tiện ích hoặc điều kiện riêng. | Đặt bàn VIP, gán phục vụ riêng. | *Phòng đặc biệt* (Chung chung, không sang trọng). |
| **Bàn** | Đơn vị vị trí ngồi ăn cụ thể của khách. | Toàn bộ giao diện FOH, POS, KDS. | *Chỗ ngồi, Bàn ăn số* (Dài dòng). |
| **Bàn chính** | Bàn đại diện nhận đơn khi gộp nhiều bàn lại với nhau. | Nghiệp vụ gộp bàn cho đoàn lớn. | *Bàn chủ, Bàn mẹ* (Dịch theo nghĩa kỹ thuật, không tự nhiên). |
| **Bàn phụ (Bàn ghép)**| Các bàn được gộp vào bàn chính. | Nghiệp vụ gộp bàn. | *Bàn con, Bàn nhánh* (Thiếu tính nghiệp vụ F&B). |
| **Trống** | Trạng thái bàn sạch sẽ, sẵn sàng đón khách mới. | Nhãn trạng thái màu xanh lá. | *Khả dụng, Rảnh rỗi* (Dịch máy thô). |
| **Đã đặt trước** | Trạng thái bàn đã được khách hẹn trước giờ đến. | Nhãn trạng thái màu xanh dương. | *Bảo lưu, Đã giữ* (Không rõ nghĩa F&B). |
| **Có khách** | Trạng thái bàn đang có khách ngồi ăn và gọi món. | Nhãn trạng thái màu vàng hổ phách. | *Đang bận, Chiếm đóng* (Dịch thô từ "Occupied"). |
| **Đang thanh toán** | Trạng thái khách đã nhận phiếu tạm tính, quầy đang xử lý tiền. | Nhãn trạng thái màu tím. | *Chờ trả tiền, Yêu cầu hóa đơn* (Dài dòng). |
| **Cần dọn dẹp** | Trạng thái khách đã trả tiền rời đi, bàn cần nhân viên dọn dẹp. | Nhãn trạng thái màu cam (Báo busser). | *Bàn bẩn, Chờ vệ sinh* (Dùng từ "bẩn" thiếu tế nhị). |
| **Ngừng phục vụ** | Trạng thái bàn bị hỏng hoặc khu vực tạm đóng cửa. | Nhãn trạng thái màu xám. | *Không hoạt động, Hỏng hóc* (Quá nặng nề). |
| **Chuyển bàn** | Thao tác chuyển toàn bộ khách và món sang bàn mới. | Menu thao tác bàn trên POS. | *Dời bàn, Đổi bàn* (Chuyển bàn là từ chuẩn F&B). |
| **Gộp bàn** | Thao tác ghép 2 hay nhiều bàn thành một đơn duy nhất. | Menu thao tác bàn trên POS. | *Nhập bàn, Nối bàn* (Gộp bàn là từ ngữ phổ thông nhất). |
| **Tách bàn** | Chia một nhóm khách hoặc một số món ra bàn riêng. | Menu thao tác bàn trên POS. | *Phân bàn, Chia bàn* (Tách bàn là chuẩn nghiệp vụ). |
| **Bao trọn phòng** | Khóa toàn bộ bàn trong một phòng để phục vụ tiệc riêng. | Nghiệp vụ đặt tiệc phòng VIP. | *Khóa phòng, Thuê nguyên phòng* (Bao trọn phòng chuẩn ngữ cảnh). |

---

### 2.2. Catalog & Menu Terminology (Thực Đơn & Món Ăn)

| Thuật ngữ Hiển thị Chuẩn | Ý nghĩa Nghiệp vụ | Ngữ cảnh Sử dụng | Thuật ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Thực đơn** | Danh mục tổng hợp toàn bộ các món ăn và đồ uống. | Cổng quản trị, giao diện gọi món. | *Menu* (Hạn chế dùng từ mượn tiếng Anh trên UI). |
| **Danh mục món** | Nhóm các món ăn cùng loại (Khai vị, Món nướng, Lẩu, Đồ uống). | Phân loại món trên màn hình POS. | *Chuyên mục, Thể loại* (Thuật ngữ báo chí/tin tức). |
| **Món ăn** / **Đồ uống** | Từng sản phẩm cụ thể phục vụ khách. | Danh sách sản phẩm. | *Hàng hóa, Mặt hàng* (Nghe như bán lẻ/tạp hóa). |
| **Kích cỡ (Size)** | Các phiên bản định lượng của món (Nhỏ, Vừa, Lớn; Nồi lớn, Nồi nhỏ).| Chọn biến thể món. | *Biến thể, Quy cách* (Từ kỹ thuật). |
| **Nhóm tùy chọn** | Tập hợp các lựa chọn kèm theo món (Nước lẩu, Độ cay, Topping). | Cửa sổ chọn tùy biến khi gọi món. | *Nhóm sửa đổi, Modifier* (Từ dịch máy/tiếng Anh). |
| **Món ăn kèm / Topping**| Thành phần khách có thể chọn thêm (thêm bắp bò, thêm nấm). | Lựa chọn tùy biến. | *Món bổ sung* (Dài dòng). |
| **Ghi chú bếp** | Yêu cầu đặc biệt của khách cho đầu bếp (Không hành, ít cay). | Nhập liệu ghi chú trên từng món. | *Lời nhắn, Hướng dẫn nấu* (Ghi chú bếp ngắn gọn, chuẩn). |
| **Tạm hết món (Hết món)**| Trạng thái món hết nguyên liệu, tạm thời không nhận order. | Màn hình POS và quản lý món. | *86, Không có sẵn, Hết hàng* (Tạm hết món lịch sự, chuẩn F&B). |
| **Định lượng món (BOM)**| Công thức định lượng nguyên vật liệu để nấu một suất ăn. | Quản lý kho và giá thành món ăn. | *Công thức vật tư, BOM* (BOM là từ viết tắt tiếng Anh). |

---

### 2.3. Ordering & Kitchen Terminology (Gọi Món & Bếp KDS)

| Thuật ngữ Hiển thị Chuẩn | Ý nghĩa Nghiệp vụ | Ngữ cảnh Sử dụng | Thuật ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Phiếu gọi món (Đơn gọi)**| Danh sách các món khách đã yêu cầu cho một lượt ăn. | Màn hình POS nhân viên phục vụ. | *Order, Đơn đặt hàng* (Order là tiếng Anh; Đơn đặt hàng là TMĐT). |
| **Đợt gọi món** | Từng lượt gọi thêm đồ của khách trong bữa ăn (Đợt 1, Đợt 2). | Quản lý các món thêm sau. | *Vòng gọi, Round* (Đợt gọi món chuẩn tiếng Việt). |
| **Báo bếp** (hoặc **Bắn đơn**)| Hành động xác nhận gửi món xuống màn hình KDS nhà bếp. | Nút bấm chính trên POS phục vụ. | *Gửi bếp, Fire, Đặt lệnh* (Báo bếp là thuật ngữ quen thuộc nhất). |
| **Tạm giữ món** | Đánh dấu món chưa nấu ngay (chờ ăn xong khai vị mới nấu). | Thao tác trên từng món trong đơn. | *Hold, Giữ lại* (Tạm giữ món rõ nghĩa nhất). |
| **Báo nấu món giữ** | Kích hoạt món đang tạm giữ bắt đầu nấu. | Thao tác kích hoạt món giữ. | *Fire món, Thả món* (Từ lóng nội bộ/tiếng Anh). |
| **Hủy món** | Hủy một món đã gọi nhưng khách không dùng hoặc lỗi. | Thao tác điều chỉnh đơn món. | *Xóa món, Void* (Xóa làm hiểu nhầm là xóa khỏi CSDL). |
| **Món ưu tiên (Gấp)** | Đơn món cần làm gấp do khách đợi lâu hoặc tiệc VIP. | Thẻ KDS nhấp nháy báo động. | *Rush order, Món khẩn cấp* (Món ưu tiên / Món gấp chuẩn F&B). |
| **Bếp Nóng** | Trạm bếp phụ trách xào, rán, nướng, lẩu. | Tên trạm KDS bếp nóng. | *Hot Kitchen* (Không để tiếng Anh trên màn hình). |
| **Bếp Nguội / Salad** | Trạm phụ trách gỏi, nộm, sashimi, khai vị nguội. | Tên trạm KDS bếp nguội. | *Cold Kitchen* (Không để tiếng Anh). |
| **Quầy Pha Chế (Bar)**| Trạm phụ trách cà phê, trà, cocktail, đồ uống. | Tên trạm KDS quầy bar. | *Bar station* (Quầy Pha Chế chuẩn tiếng Việt). |
| **Bếp Bánh / Tráng Miệng**| Trạm phụ trách tráng miệng, bánh ngọt, kem. | Tên trạm KDS đồ tráng miệng. | *Dessert station* (Không để tiếng Anh). |
| **Bàn Ra Món (Tiếp Thực)**| Trạm trưởng bếp điều phối món ra bàn, ghép đủ món cho khay. | Màn hình KDS Master Expediter. | *Expediter, Pass* (Bàn Ra Món hoặc Bàn Tiếp Thực chuẩn F&B). |
| **Chờ chế biến** | Món mới nhận, đang trong hàng đợi chờ đầu bếp nấu. | Trạng thái món trên KDS. | *Đang chờ, Pending* (Chờ chế biến rõ ràng cho bếp). |
| **Đang nấu** | Đầu bếp đã bấm xác nhận bắt đầu chế biến món. | Trạng thái món trên KDS. | *Đang làm, Đang xử lý, Cooking* (Đang nấu chuẩn nhất). |
| **Đã xong (Chờ bưng)** | Món đã nấu xong đặt tại bàn ra món, chờ nhân viên bưng. | Trạng thái món trên KDS. | *Sẵn sàng phục vụ, Ready to serve* (Dịch thô tiếng Anh). |
| **Đã phục vụ** | Món ăn đã được đặt lên bàn khách thành công. | Trạng thái hoàn thành chu trình món. | *Đã bưng, Đã giao, Served* (Đã phục vụ là từ chuẩn mực). |

---

### 2.4. Cashier & Financial Terminology (Thu Ngân & Thanh Toán)

| Thuật ngữ Hiển thị Chuẩn | Ý nghĩa Nghiệp vụ | Ngữ cảnh Sử dụng | Thuật ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Thu ngân** | Nhân viên phụ trách thu tiền và quản lý két tiền mặt. | Vai trò người dùng, màn hình thanh toán.| *Kế toán thu, Cashier* (Cashier là tiếng Anh). |
| **Phiếu tạm tính** | Phiếu in cho khách kiểm tra danh sách món trước khi trả tiền. | Phiếu in sơ bộ đưa tại bàn. | *Pre-bill, Phiếu tính tiền sơ bộ* (Phiếu tạm tính là chuẩn F&B VN).|
| **Hóa đơn thanh toán** | Chứng từ thanh toán chính thức khi khách đã trả tiền. | Phiếu in hóa đơn cuối cùng. | *Receipt, Bill, Hóa đơn bán lẻ* (Hóa đơn thanh toán trang trọng, chuẩn). |
| **Tiền món (Tạm tính)** | Tổng tiền các món chưa trừ giảm giá và chưa cộng thuế. | Dòng tính toán đầu tiên trên hóa đơn. | *Subtotal, Tổng phụ* (Tổng phụ là dịch máy vô nghĩa). |
| **Chiết khấu / Giảm giá**| Số tiền được giảm theo chương trình ưu đãi hoặc voucher. | Dòng giảm trừ trên hóa đơn. | *Discount, Bớt tiền* (Bớt tiền quá suồng sã). |
| **Phí dịch vụ** | Khoản thu phục vụ tính theo tỷ lệ (thường là 5%). | Dòng phí trên hóa đơn. | *Tip, Tiền boa, Service charge* (Phí dịch vụ là từ ngữ pháp lý). |
| **Thuế GTGT (VAT)** | Thuế giá trị gia tăng theo quy định (8% hoặc 10%). | Dòng thuế trên hóa đơn. | *Thuế bán hàng, Tax* (Thuế GTGT là tên chính xác tại VN). |
| **Tổng thanh toán** | Số tiền cuối cùng khách phải trả sau thuế và phí. | Dòng tổng tiền lớn nhất trên màn hình/bill. | *Total, Tổng cộng phải trả* (Tổng thanh toán ngắn gọn, dứt khoát). |
| **Tách theo món** | Tách hóa đơn bằng cách chọn từng món cho từng người trả. | Màn hình tách hóa đơn. | *Chia món, Tách lẻ* (Tách theo món chuẩn nghiệp vụ). |
| **Chia đều (1/N)** | Tách hóa đơn bằng cách chia đều tổng tiền cho $N$ người. | Màn hình tách hóa đơn. | *Campuchia, Chia đầu người* (Campuchia là từ lóng; Chia đều là chuẩn mực). |
| **Tiền mặt** | Phương thức thanh toán bằng tiền giấy VNĐ. | Lựa chọn phương thức thanh toán. | *Cash* (Dùng tiếng Việt). |
| **Thẻ ngân hàng (POS)** | Quẹt thẻ tín dụng/ghi nợ qua máy quẹt thẻ ngân hàng. | Lựa chọn phương thức thanh toán. | *Quẹt thẻ, Card* (Thẻ ngân hàng rõ ràng, chuẩn mực). |
| **Chuyển khoản VietQR** | Quét mã QR thanh toán liên ngân hàng Napas 24/7 tức thì. | Lựa chọn phương thức thanh toán. | *Banking, Chuyển khoản* (Nêu rõ VietQR để khách dễ nhận biết). |
| **Mở ca** | Bắt đầu ca làm việc của thu ngân, nhập số tiền chuẩn bị ban đầu. | Màn hình quản lý ca. | *Khởi tạo ca, Open shift* (Mở ca là từ phổ thông nhất). |
| **Chốt ca** | Kết thúc ca làm việc, đếm tiền thực tế và in báo cáo. | Màn hình quản lý ca. | *Đóng ca, Kết ca, Close shift* (Chốt ca là từ ngữ tiêu chuẩn ngành). |
| **Tiền mặt đầu ca** | Số tiền lẻ có sẵn trong ngăn kéo khi mở ca để thối tiền. | Nhập số tiền khi mở ca. | *Tiền trôi nổi, Tiền phao, Cash float* (Dịch "float" thành phao là thô kệch). |
| **Rút tiền giữa ca** | Nộp bớt tiền mặt từ ngăn kéo vào két an toàn của quản lý. | Thao tác an toàn quỹ trong ca. | *Rút két, Cash drop* (Rút tiền giữa ca rõ ràng mục đích). |
| **Kiểm kê tiền** | Đếm chi tiết số tờ từng mệnh giá khi chốt ca. | Màn hình chốt ca thu ngân. | *Đếm tiền, Kiểm quỹ* (Kiểm kê tiền trang trọng, chính xác). |
| **Chênh lệch tiền mặt** | Khoảng lệch giữa tiền thực tế đếm được và số liệu hệ thống tính.| Báo cáo chốt ca. | *Tiền thừa/thiếu, Lệch quỹ* (Chênh lệch tiền mặt là chuẩn kế toán).|
| **Báo cáo chốt ca (Z-Report)**| Biên bản tổng kết doanh thu và thu chi khi đóng ca. | Phiếu in chốt ca. | *Báo cáo đóng ca, Báo cáo Z* (Báo cáo chốt ca quen thuộc nhất). |

---

## 3. Quy Chuẩn Định Dạng Khu Vực Việt Nam (Vietnam-Specific Conventions)

### 3.1. Tiền Tệ & Định Dạng Số (Currency & Numbers)
- **Ký hiệu tiền tệ**: `₫` hoặc `VND`.
- **Quy tắc phân cách**:
  - Dấu chấm (`.`) dùng làm dấu phân cách hàng nghìn, hàng triệu, hàng tỷ.
  - Không hiển thị số thập phân lẻ (không dùng `.00` vì đồng Việt Nam không có đơn vị xu/hào trong lưu thông).
- **Mẫu hiển thị chuẩn**:
  - Đầy đủ: `1.250.000 ₫` hoặc `1.250.000 VND`.
  - Trên màn hình POS nhỏ (rút gọn): `1.250k` (chỉ dùng trong chế độ xem nhanh, trên hóa đơn in bắt buộc ghi đầy đủ `1.250.000 ₫`).

### 3.2. Ngày Tháng, Thời Gian & Múi Giờ (Date, Time & Timezone)
- **Múi giờ chuẩn**: `Asia/Ho_Chi_Minh` (UTC+7). Mọi mốc thời gian lưu trữ phải lưu UTC trong CSDL và chuyển đổi chính xác sang `Asia/Ho_Chi_Minh` khi hiển thị cho người dùng.
- **Định dạng ngày**: `dd/MM/yyyy` (Ví dụ: `02/11/2026`). Tuyệt đối không dùng định dạng kiểu Mỹ `MM/dd/yyyy` gây nhầm lẫn nghiêm trọng cho nhân viên.
- **Định dạng thời gian**: 24 giờ (`HH:mm:ss` hoặc `HH:mm`). Ví dụ: `08:30`, `19:45`.

### 3.3. Địa Chỉ & Số Điện Thoại (Address & Phone)
- **Thứ tự địa chỉ chuẩn Việt Nam**: `Số nhà/Ngõ ngách $\rightarrow$ Đường/Phố $\rightarrow$ Phường/Xã $\rightarrow$ Quận/Huyện/Thị xã $\rightarrow$ Tỉnh/Thành phố`.
- **Số điện thoại**: Định dạng số di động 10 số của các nhà mạng Việt Nam (`09x`, `08x`, `07x`, `03x`, `05x`), hiển thị có khoảng trắng ngăn cách dễ đọc: `0908 123 456` hoặc số cố định `(024) 3823 4567`.

### 3.4. Bộ Gõ Tiếng Việt & Mã Hóa Ký Tự (Unicode & Vietnamese Input)
- **Mã hóa ký tự**: Toàn bộ hệ thống (Web, API, Cơ sở dữ liệu SQL Server) sử dụng **UTF-8** (`NVARCHAR` trong SQL Server với collation hỗ trợ tiếng Việt như `Latin1_General_100_CI_AS_SC_UTF8` hoặc `SQL_Latin1_General_CP1_CI_AS`).
- **Bộ gõ**: Đảm bảo toàn bộ các ô nhập liệu (Text Input, Text Area, Tìm kiếm) hoạt động mượt mà với các bộ gõ tiếng Việt phổ biến tại Việt Nam (Unikey, EVKey, bộ gõ mặc định trên iOS/Android/macOS/Windows) ở cả 2 kiểu gõ **Telex** và **VNI**.

---

## 4. Phân Tích Yêu Cầu Thuế & Hóa Đơn Điện Tử (Tax & E-Invoice Analysis)

### 4.1. Quy Định Thuế Giá Trị Gia Tăng (VAT) Hiện Hành
- Hỗ trợ các mức thuế suất chuẩn tại Việt Nam: **8%** (theo chính sách giảm thuế kích cầu) và **10%** (mức thuế suất tiêu chuẩn).
- Cho phép cấu hình giá bán món ăn là **"Giá đã bao gồm thuế GTGT"** hoặc **"Giá chưa bao gồm thuế GTGT (cộng thêm ở cuối hóa đơn)"** tùy theo mô hình kinh doanh của từng nhà hàng.

### 4.2. Định Hướng Tích Hợp Hóa Đơn Điện Tử Khởi Tạo Từ Máy Tính Tiền (E-Invoice Phase-In)
- **Cơ sở pháp lý**: Nghị định 123/2020/NĐ-CP và Thông tư 78/2021/TT-BTC của Bộ Tài chính quy định về hóa đơn điện tử khởi tạo từ máy tính tiền có kết nối chuyển dữ liệu điện tử với cơ quan thuế đối với ngành nghề ăn uống, nhà hàng.
- **Phân kỳ triển khai**:
  - **Giai đoạn 1 (MVP)**: Hệ thống in Hóa đơn thanh toán nội bộ hoàn chỉnh (đầy đủ mã tra cứu, số hóa đơn, thông tin người mua/bán, chi tiết thuế GTGT). Cấu trúc dữ liệu hóa đơn được thiết kế sẵn sàng các trường: Mã số thuế người mua, Tên công ty, Địa chỉ xuất hóa đơn, Email nhận hóa đơn.
  - **Giai đoạn 2 (v1.1+)**: Tích hợp API xuất hóa đơn điện tử trực tiếp sang các nhà cung cấp giải pháp HĐĐT phổ biến tại Việt Nam (VNPT-Invoice, Viettel Sinvoice, MISA meInvoice, Hilo Invoice) với chứng thư số HSM/Server.
- > [!IMPORTANT]
  > *Ghi chú tuân thủ pháp lý*: Không tự ý bịa đặt các thuật toán ký số hoặc chuẩn XML/JSON thuế khi chưa có tài liệu kỹ thuật kết nối chính thức từ nhà cung cấp giải pháp HĐĐT được cấp phép.

---

## 5. Nguyên Tắc Kiến Trúc Kỹ Thuật Cho Bản Địa Hóa (Technical Localization Principles)

Để đảm bảo hệ thống vừa tối ưu 100% cho thị trường Việt Nam hiện tại, vừa sẵn sàng mở rộng đa ngôn ngữ (Multilingual i18n) trong tương lai:

```mermaid
flowchart TD
    subgraph CoreBackend["Backend Business Core (English Only)"]
        Domain[Domain Entities: Order, Table, Invoice]
        Enums[Domain Enums: TableState.Occupied, PrepState.Cooking]
        Logic[Business Calculations: Tax, Discounts]
    end

    subgraph LocalizationCatalog["Localization & Resource Layer"]
        ResxVI[vi-VN Resource Catalog: "Đang nấu", "Có khách", "Thu ngân"]
        ResxEN[en-US Resource Catalog: "Cooking", "Occupied", "Cashier"]
        Provider[IStringLocalizer / Culture Provider: Default = vi-VN]
    end

    subgraph Presentation["User-Facing Presentation (100% Vietnamese)"]
        WebAdmin[Admin Portal: Tiếng Việt 100%]
        POS[Waiter POS: Tiếng Việt 100%]
        KDS[Kitchen Display: Tiếng Việt 100%]
        Print[ESC/POS Thermal Bill: Tiếng Việt UTF-8]
    end

    Domain --> Logic
    Logic --> Provider
    ResxVI --> Provider
    ResxEN -.->|Future Release| Provider
    Provider --> WebAdmin & POS & KDS & Print
```

### 5.1. Các Nguyên Tắc Bắt Buộc:
1. **Không Hard-code Chuỗi Tiếng Việt Trong Mã Nguồn**: Toàn bộ chuỗi hiển thị giao diện, thông báo lỗi, nhãn nút bấm phải được đưa vào file Resource (`.resx` trong ASP.NET Core) hoặc từ điển JSON cho frontend, gắn với khóa định danh bằng tiếng Anh có cấu trúc rõ ràng:
   - Ví dụ: `Tables.Status.Occupied` $\rightarrow$ `"Có khách"`.
   - Ví dụ: `Orders.Validation.TableRequired` $\rightarrow$ `"Vui lòng chọn bàn trước khi gọi món."`.
   - Ví dụ: `Cashier.Shift.DiscrepancyShortage` $\rightarrow$ `"Thiếu hụt tiền mặt so với hệ thống: {0} ₫"`.
2. **Định Danh Trong CSDL Và Mã Nguồn Hoàn Toàn Bằng Tiếng Anh**:
   - Tên bảng: `Orders`, `RestaurantTables`, `Invoices`, `MenuItems`.
   - Tên cột: `TotalAmount`, `TableState`, `PrepStatus`, `CreatedAt`.
   - Giá trị Enum: `TableState.Available`, `TableState.Occupied`, `TableState.CleaningNeeded`.
   - Tuyệt đối không dùng tiếng Việt không dấu (như `BanAn`, `TrangThai`, `HoaDon`) làm định danh CSDL hoặc biến code.
3. **Cơ Chế Mở Rộng Đa Ngôn Ngữ Tương Lai**:
   - Sử dụng `RequestLocalizationMiddleware` chuẩn của ASP.NET Core với văn hóa mặc định là `vi-VN` (`CultureInfo("vi-VN")`).
   - Sau này khi cần hỗ trợ thêm tiếng Anh cho khách du lịch quốc tế, chỉ cần bổ sung file `Resources.en-US.resx` mà không phải sửa một dòng mã nguồn nghiệp vụ nào.

---

## 6. Tiêu Chuẩn Trải Nghiệm Người Dùng (UX/UI) Cho Nhân Viên Nhà Hàng Việt Nam

Đặc thù nhân viên nhà hàng (phục vụ bàn là sinh viên làm thêm, phụ bếp, nhân viên chạy bàn) thường không có kỹ năng công nghệ cao và phải thao tác trong môi trường giờ cao điểm ồn ào, dầu mỡ, vội vã. Giao diện phải tuân thủ nghiêm ngặt các tiêu chuẩn UX sau:

1. **Nhãn Ngắn Gọn, Dễ Đọc Từ Xa**:
   - Nút bấm to, chữ đậm, kích thước vùng chạm tối thiểu **$48 \times 48\text{ px}$** trên màn hình cảm ứng tablet.
   - Tránh câu dài: dùng *"Báo bếp"* thay vì *"Xác nhận gửi các món này xuống bếp"*; dùng *"Thanh toán"* thay vì *"Tiến hành thanh toán hóa đơn"*.
2. **Màu Sắc Nhận Diện Trực Quan Cao**:
   - Trạng thái bàn và món phải có sự tương phản màu sắc rõ rệt để nhân viên nhìn lướt qua trong 1 giây là nhận biết được ngay (Xanh lá = Trống, Vàng = Có khách, Tím = Đang thanh toán, Đỏ = Quá giờ/Hủy).
3. **Biểu Tượng Kết Hợp Chữ Viết (Icon + Text)**:
   - Các thao tác quan trọng luôn kết hợp biểu tượng đồ họa quen thuộc cùng với chữ tiếng Việt (Ví dụ: Biểu tượng máy in kèm chữ *"In tạm tính"*; Biểu tượng mũi tên kèm chữ *"Chuyển bàn"*).
4. **Thông Báo Thân Thiện, Tránh Lỗi Kỹ Thuật (Friendly Error Feedback)**:
   - Không bao giờ hiển thị các mã lỗi kỹ thuật như `NullReferenceException`, `Error 500`, `Bad Request`.
   - Chuyển đổi thành thông điệp tiếng Việt lịch sự, chỉ dẫn rõ ràng hành động tiếp theo:
     - *Kém*: "Lỗi 400: Dữ liệu không hợp lệ."
     - *Chuẩn*: "Vui lòng chọn ít nhất một món ăn trước khi nhấn Báo bếp."
