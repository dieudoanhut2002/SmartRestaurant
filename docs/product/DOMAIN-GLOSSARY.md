# Domain Glossary & Ubiquitous Language — SmartRestaurant

- **Document Version**: 2.0.0
- **Document Status**: `CONFIRMED`
- **Context**: Standardized business terminology for developers, business analysts, UI designers, and restaurant operators.
- **Reference**: Detailed operational dictionary and forbidden terms available at [VIETNAMESE-LOCALIZATION-STANDARD.md](./VIETNAMESE-LOCALIZATION-STANDARD.md).

---

## 1. Physical & Spatial Concepts (Không Gian & Sơ Đồ Bàn)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Organization** | **Doanh nghiệp / Tổ chức** | Thực thể doanh nghiệp hoặc chuỗi nhượng quyền sở hữu các chi nhánh. Ranh giới multi-tenant cao nhất. | *Tập đoàn* (Quá lớn), *Hệ thống* (Mơ hồ). |
| **Branch** | **Chi nhánh nhà hàng** | Một địa điểm kinh doanh vật lý cụ thể có sơ đồ bàn, kho, nhân viên và két thu ngân riêng. | *Cửa hàng* (Thiên về bán lẻ). |
| **Dining Area** | **Khu vực** | Phân vùng không gian lớn trong nhà hàng (Tầng 1, Sân vườn, Tầng thượng). Mỗi khu vực có canvas riêng. | *Vùng, Địa bàn* (Quá hành chính/cơ quan). |
| **Room** | **Phòng ăn** | Không gian độc lập bên trong khu vực chứa nhiều bàn ăn (Phòng 201, Phòng Tiệc). | *Buồng* (Cũ kỹ, thiếu thẩm mỹ F&B). |
| **VIP Room** | **Phòng VIP** | Phòng ăn riêng tư cao cấp có yêu cầu tiêu dùng tối thiểu và phục vụ riêng. | *Phòng đặc biệt* (Chung chung, không sang trọng). |
| **Table** | **Bàn** | Vị trí ngồi ăn cụ thể của khách, có hình dạng (tròn, chữ nhật), sức chứa và tọa độ. | *Chỗ ngồi, Bàn ăn số* (Dài dòng). |
| **Table Node** | **Điểm bàn đồ họa** | Biểu diễn kỹ thuật số của bàn trên canvas 2D Konva.js (`X`, `Y`, `Width`, `Height`, `Rotation`). | *Nút bàn* (Từ ngữ kỹ thuật khó hiểu). |
| **Obstacle** | **Vật cản kiến trúc** | Cột nhà, vách ngăn, tường, quầy thu ngân, lối đi được vẽ trên sơ đồ bàn để định hướng. | *Chướng ngại vật* (Nghe như quân sự/game). |

---

## 2. Table Lifecycle & Session Concepts (Lượt Khách & Trạng Thái Bàn)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Dining Session** | **Lượt khách** | Chu kỳ phục vụ một đoàn khách tại bàn từ lúc ngồi vào đến khi thanh toán rời đi. | *Phiên bàn, Session* (Thuật ngữ kỹ thuật). |
| **Available** | **Trống** | Bàn sạch sẽ, sẵn sàng đón khách mới (Màu xanh lá). | *Khả dụng, Rảnh rỗi* (Dịch máy thô). |
| **Reserved** | **Đã đặt trước** | Bàn đã có khách hẹn trước giờ đến (Màu xanh dương). | *Bảo lưu, Đã giữ* (Không rõ nghĩa F&B). |
| **Occupied** | **Có khách** | Bàn đang có khách ngồi ăn và gọi món (Màu vàng hổ phách). | *Đang bận, Chiếm đóng* (Dịch thô từ "Occupied"). |
| **BillingRequested** | **Đang thanh toán** | Khách đã yêu cầu phiếu tạm tính, quầy đang xử lý (Màu tím). | *Chờ trả tiền, Yêu cầu hóa đơn* (Dài dòng). |
| **CleaningNeeded** | **Cần dọn dẹp** | Khách đã thanh toán rời đi, bàn cần nhân viên dọn dẹp (Màu cam). | *Bàn bẩn, Chờ vệ sinh* (Dùng từ "bẩn" thiếu tế nhị). |
| **OutOfService** | **Ngừng phục vụ** | Bàn hỏng hoặc khu vực đóng cửa bảo trì (Màu xám). | *Không hoạt động, Hỏng hóc* (Quá nặng nề). |
| **Move Table** | **Chuyển bàn** | Thao tác chuyển toàn bộ khách và món sang bàn mới. | *Dời bàn, Đổi bàn* (Chuyển bàn là từ chuẩn F&B). |
| **Merge Tables** | **Gộp bàn** | Thao tác ghép 2 hay nhiều bàn thành một đơn duy nhất cho đoàn đông. | *Nhập bàn, Nối bàn* (Gộp bàn là từ phổ thông nhất). |
| **Split Table** | **Tách bàn** | Chia một nhóm khách hoặc một số món ra bàn riêng độc lập. | *Phân bàn, Chia bàn* (Tách bàn là chuẩn nghiệp vụ). |
| **Unmerge Tables** | **Hủy gộp bàn** | Thao tác hủy liên kết nhóm bàn đã gộp, đưa các bàn thành viên về trạng thái độc lập và phân bổ lại món/hóa đơn tương ứng (theo PDR-010). | *Tách bàn gộp, Xóa gộp bàn* (Cần phân biệt rõ với Tách bàn - Split Table). |

---

## 3. Catalog & Product Concepts (Thực Đơn & Định Lượng)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Category** | **Danh mục món** | Nhóm các món ăn cùng loại (Khai vị, Món nướng, Lẩu, Đồ uống). | *Chuyên mục, Thể loại* (Thuật ngữ báo chí). |
| **Menu Item** | **Món ăn / Đồ uống** | Sản phẩm ẩm thực niêm yết trên thực đơn kèm giá bán và trạm chế biến. | *Hàng hóa, Mặt hàng* (Thiên về bán lẻ tạp hóa). |
| **Variant** | **Kích cỡ (Size)** | Các phiên bản định lượng của món (Nhỏ, Vừa, Lớn; Nồi lớn, Nồi nhỏ). | *Biến thể, Quy cách* (Từ kỹ thuật). |
| **Modifier Group** | **Nhóm tùy chọn** | Tập hợp các lựa chọn kèm theo món (Nước lẩu, Độ cay, Topping). | *Nhóm sửa đổi, Modifier* (Từ dịch máy/tiếng Anh). |
| **Modifier Option** | **Món ăn kèm / Topping**| Thành phần khách có thể chọn thêm (thêm bắp bò, thêm nấm). | *Món bổ sung* (Dài dòng). |
| **Recipe BOM** | **Định lượng món ăn**| Công thức định lượng nguyên vật liệu để nấu một suất ăn. | *Công thức vật tư, BOM* (BOM là từ viết tắt tiếng Anh). |
| **86 / Out of Stock** | **Tạm hết món** | Trạng thái món hết nguyên liệu, tạm thời không nhận order. | *86, Không có sẵn, Hết hàng* (Tạm hết món lịch sự, chuẩn F&B). |

---

## 4. Ordering & Kitchen Operations (Gọi Món & Bếp KDS)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Order** | **Phiếu gọi món (Đơn gọi)**| Danh sách toàn bộ các món khách đã gọi trong suốt lượt ăn. | *Order, Đơn đặt hàng* (Order là tiếng Anh; Đơn đặt hàng là TMĐT). |
| **Order Round** | **Đợt gọi món** | Từng đợt gọi thêm món của khách (Đợt 1, Đợt 2). | *Vòng gọi, Round* (Đợt gọi món chuẩn tiếng Việt). |
| **Fire to Kitchen** | **Báo bếp / Bắn đơn** | Hành động gửi các món đã chọn trên máy tính bảng xuống KDS bếp. | *Gửi bếp, Fire, Đặt lệnh* (Báo bếp là thuật ngữ quen thuộc nhất). |
| **Hold Dish** | **Tạm giữ món** | Đánh dấu chưa nấu ngay (chờ ăn xong món trước mới báo bếp). | *Hold, Giữ lại* (Tạm giữ món rõ nghĩa nhất). |
| **Kitchen Station** | **Bộ phận bếp (Trạm bếp)**| Trạm ẩm thực chuyên trách: Bếp Nóng, Bếp Nguội, Quầy Bar, Bếp Bánh. | *Phòng ban bếp* (Nghe như hành chính nhân sự). |
| **Kitchen Ticket** | **Phiếu chế biến (Vé KDS)**| Thẻ hiển thị trên KDS chứa các món được định tuyến về trạm đó. | *Chit, Phiếu bếp* (Phiếu chế biến chuyên nghiệp hơn). |
| **Expediter (Pass)**| **Bàn Ra Món (Tiếp Thực)**| Trạm trưởng bếp điều phối món ra bàn, ghép đủ món cho khay bưng. | *Expediter, Pass* (Bàn Ra Món hoặc Bàn Tiếp Thực chuẩn F&B). |
| **Pending (State)** | **Chờ chế biến** | Món mới nhận, đang trong hàng đợi chờ đầu bếp nấu. | *Đang chờ, Pending* (Chờ chế biến rõ ràng cho bếp). |
| **Cooking (State)** | **Đang nấu** | Đầu bếp đã bấm xác nhận bắt đầu chế biến món. | *Đang làm, Đang xử lý, Cooking* (Đang nấu chuẩn nhất). |
| **ReadyToServe** | **Đã xong (Chờ bưng)** | Món đã nấu xong đặt tại bàn ra món, chuông báo phục vụ. | *Sẵn sàng phục vụ, Ready to serve* (Dịch thô tiếng Anh). |
| **Served (State)** | **Đã phục vụ** | Nhân viên tiếp thực đã bưng món đến bàn khách thành công. | *Đã bưng, Đã giao, Served* (Đã phục vụ là từ chuẩn mực). |
| **Rush Order** | **Món ưu tiên (Gấp)** | Đơn món cần làm gấp do khách đợi lâu hoặc tiệc VIP. | *Rush order, Món khẩn cấp* (Món ưu tiên / Món gấp chuẩn F&B). |

---

## 5. Billing & Cashier Concepts (Thu Ngân & Thanh Toán)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Pre-Bill** | **Phiếu tạm tính** | Phiếu in cho khách kiểm tra danh sách món trước khi trả tiền. | *Pre-bill, Phiếu tính tiền sơ bộ* (Phiếu tạm tính là chuẩn F&B VN).|
| **Invoice / Final Bill**| **Hóa đơn thanh toán**| Phiếu thanh toán chính thức có giá trị tài chính khi đã thu tiền. | *Receipt, Bill, Hóa đơn bán lẻ* (Hóa đơn thanh toán trang trọng, chuẩn). |
| **Subtotal** | **Tiền món (Tạm tính)**| Tổng tiền các món chưa trừ giảm giá và chưa cộng thuế/phí. | *Subtotal, Tổng phụ* (Tổng phụ là dịch máy vô nghĩa). |
| **Discount** | **Chiết khấu / Giảm giá**| Tiền giảm theo món hoặc voucher toàn đơn. | *Discount, Bớt tiền* (Bớt tiền quá suồng sã). |
| **Service Charge** | **Phí dịch vụ** | Phí phục vụ theo tỷ lệ (thường là 5%). | *Tip, Tiền boa, Service charge* (Phí dịch vụ là từ ngữ pháp lý). |
| **VAT Tax** | **Thuế GTGT (VAT)** | Thuế giá trị gia tăng (8% hoặc 10%). | *Thuế bán hàng, Tax* (Thuế GTGT là tên chính xác tại VN). |
| **Total Payable** | **Tổng thanh toán** | Số tiền cuối cùng khách phải trả sau thuế và phí. | *Total, Tổng cộng phải trả* (Tổng thanh toán ngắn gọn, dứt khoát). |
| **Split Bill by Items** | **Tách hóa đơn theo món**| Phân chia từng món cụ thể cho từng khách thanh toán riêng. | *Chia món, Tách lẻ* (Tách theo món chuẩn nghiệp vụ). |
| **Split Bill Equally** | **Chia đều hóa đơn (1/N)**| Lấy tổng tiền chia đều cho $N$ người trong bàn. | *Campuchia, Chia đầu người* (Chia đều là chuẩn mực). |
| **Cash Float** | **Tiền mặt đầu ca** | Tiền lẻ chuẩn bị sẵn trong ngăn kéo khi mở ca để thối tiền. | *Tiền trôi nổi, Tiền phao, Cash float* (Dịch "float" thành phao là thô kệch). |
| **Z-Report** | **Báo cáo chốt ca (Z-Report)**| Biên bản tổng kết doanh thu và thu chi khi đóng ca. | *Báo cáo đóng ca, Báo cáo Z* (Báo cáo chốt ca quen thuộc nhất). |
| **Payment Transaction** | **Giao dịch thanh toán**| Bản ghi thanh toán thành phần theo từng phương thức (Tiền mặt, Thẻ, VietQR). | *Giao dịch tiền, Payment* (Giao dịch thanh toán chuẩn kế toán). |

---

## 6. Financial Partitioning & Shift Concepts (Thời Gian Tài Chính & Ca Kíp)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Business Date** | **Ngày kinh doanh** | Trục ngày tài chính dùng để hạch toán doanh thu và ca đêm theo mốc Cutoff. | *Ngày làm việc, Ngày hệ thống* (Mơ hồ giữa ngày lịch và ca đêm). |
| **Cutoff Time** | **Giờ chuyển ngày** | Mốc thời gian phân định ngày kinh doanh (mặc định 06:00 sáng). | *Giờ đóng cửa, Giờ giới nghiêm* (Không phản ánh bản chất tài chính). |
| **Shift Session** | **Ca làm việc** | Khoảng thời gian thu ngân nhận két tiền, mở ca, bán hàng và chốt ca bàn giao. | *Kỳ làm việc, Ca trực* (Ca làm việc chuẩn mực nhất). |

---

## 7. Inventory & Loss Concepts (Tồn Kho & Hao Hụt Thất Thoát)

| Term (EN) | Tên Hiển Thị Chuẩn | Ý Nghĩa Nghiệp Vụ | Thuật Ngữ Cần Tránh (Lý do) |
| :--- | :--- | :--- | :--- |
| **Physical Stock** | **Tồn kho thực tế** | Số lượng hàng hóa thực tế đang nằm trong kho/tủ lạnh chưa xuất kho vật lý. | *Tồn trên sổ, Tồn trong tay* (Dịch thô On-hand). |
| **Reserved Stock** | **Tồn kho tạm giữ** | Số lượng hàng hóa đã giữ chỗ cho các món khách vừa gọi (phiếu bếp đang chờ/nấu). | *Tồn bảo lưu, Khóa kho* (Tồn tạm giữ chuẩn nghiệp vụ). |
| **Available Stock** | **Tồn kho khả dụng** | Số lượng thực tế còn được phép bán: $\text{Khả dụng} = \text{Thực tế} - \text{Tạm giữ}$. | *Hàng rảnh rỗi, Tồn được bán* (Tồn khả dụng chuẩn ERP/F&B). |
| **Consumed Stock** | **Tiêu hao thực tế** | Lượng hàng hóa đã chính thức xuất kho phục vụ món ăn hoàn thành. | *Xuất kho tiêu dùng, Trừ kho* (Tiêu hao thực tế chuẩn kế toán). |
| **Waste / Spoilage**| **Hao hụt / Hủy bỏ** | Món ăn hoặc nguyên liệu bị hỏng, đổ vỡ, cháy khét cần ghi vào Sổ Hủy Hỏng. | *Phế phẩm, Đồ thừa* (Hao hụt/hủy món lịch sự và chuẩn quản trị). |
| **Remake Dish** | **Làm lại món** | Nấu lại món ăn mới do lỗi nhà bếp mà không tính thêm tiền cho khách hàng. | *Đổi món, Nấu lại* (Làm lại món do lỗi bếp chuẩn quy trình KDS). |
