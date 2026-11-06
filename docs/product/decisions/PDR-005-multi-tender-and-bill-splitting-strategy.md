# Biên Bản Quyết Định Sản Phẩm: PDR-005 — Chiến Lược Thanh Toán Đa Kênh & Tách Hóa Đơn

- **Mã định danh quyết định (Decision ID)**: PDR-005
- **Tiêu đề**: Quyết toán đa kênh (Multi-Tender) và công cụ tách hóa đơn 2 cấp độ
- **Trạng thái**: PROPOSED
- **Ngày lập**: 2026-11-02
- **Tác giả**: Kiến trúc sư trưởng giải pháp & Chuyên viên phân tích nghiệp vụ

---

## 1. Bối Cảnh Nghiệp Vụ (Context)
Thanh toán cho các đoàn khách đi ăn chung là một trong những thời điểm phát sinh nhiều va chạm và tắc nghẽn nhất trong vận hành nhà hàng. Thực khách thường xuyên yêu cầu chia hóa đơn, thanh toán kết hợp giữa tiền mặt và chuyển khoản ngân hàng điện tử (VietQR/Napas 24/7), hoặc tách hóa đơn theo từng món ăn cụ thể. Công cụ tính hóa đơn phải xử lý được các phép tính phân bổ, bóc tách thuế theo từng dòng và hạch toán đa phương thức thanh toán mà không làm thất thoát tiền do làm tròn số hoặc gây ức chế cho nhân viên thu ngân.

---

## 2. Các Phương Án Được Đánh Giá (Options Considered)

### Phương án A: Chỉ hỗ trợ quyết toán đơn thức duy nhất (Monolithic Single-Tender Settlement Only)
- Mỗi hóa đơn bắt buộc phải thanh toán toàn bộ bằng duy nhất một phương thức (100% Tiền mặt hoặc 100% Thẻ/Chuyển khoản) và tuyệt đối không hỗ trợ tách hóa đơn.
- *Ưu điểm*: Phát triển cực kỳ đơn giản.
- *Nhược điểm*: Hoàn toàn không thể chấp nhận được trong ngành dịch vụ ẩm thực hiện đại; chắc chắn sẽ gây ra phàn nàn từ khách hàng và gây ùn tắc quầy thu ngân.

### Phương án B: Hỗ trợ Tách theo món + Tách đều ($1/N$) + Thanh toán đa kênh (Khuyến nghị cho MVP)
- Hỗ trợ 2 mô hình tách hóa đơn tối ưu:
  1. **Tách theo từng món được chọn (Split by Selected Items)**: Chuyển các món ăn cụ thể sang các hóa đơn con độc lập kèm phân bổ thuế và chiết khấu theo tỷ lệ tương ứng.
  2. **Tách đều ($1/N$) (Split Evenly)**: Chia đều tổng số dư cần thanh toán thành $N$ phần bằng nhau, phần tiền lẻ phát sinh do làm tròn được cộng gộp vào hóa đơn con cuối cùng.
- Hỗ trợ **Thanh toán đa kênh (Multi-Tender Payments)**: Cho phép quyết toán bất kỳ hóa đơn nào bằng nhiều bản ghi thanh toán khác nhau (ví dụ: 500k Tiền mặt + 500k Chuyển khoản VietQR).
- *Ưu điểm*: Đáp ứng >98% yêu cầu thanh toán thực tế tại các nhà hàng; ngăn chặn các ca tính toán phần trăm tùy tiện phức tạp; tuân thủ chuẩn mực quy tắc làm tròn thuế GTGT theo pháp luật Việt Nam.
- *Nhược điểm*: Giao diện quầy thu ngân cần thiết kế màn hình quản lý đa hóa đơn mạch lạc.

### Phương án C: Hỗ trợ chia phần trăm tùy ý không đồng đều & chia tiền lẻ tùy tiện
- Hỗ trợ chia tỷ lệ phần trăm tùy ý (ví dụ: Khách A trả 63,5%, Khách B trả 36,5%) hoặc nhập các số tiền lẻ bất kỳ đến từng đơn vị Đồng.
- *Ưu điểm*: Linh hoạt tối đa trên lý thuyết.
- *Nhược điểm*: Độ phức tạp cực cao trong việc đối soát phân bổ thuế và chiết khấu ở cấp độ dòng món; xác suất phát sinh chênh lệch do làm tròn số rất lớn; rất hiếm khi được yêu cầu trong thực tế nhà hàng ăn uống.

---

## 3. Khuyến Nghị & Quyết Định (Recommendation & Decision)
**Lựa chọn Phương án B (Tách theo món + Tách đều + Thanh toán đa kênh)** cho phạm vi MVP. Trì hoãn Phương án C sang các phiên bản tiếp theo nếu xuất hiện nhu cầu thực tế cấp thiết.

> [!NOTE] Phân Định Ranh Giới & Thẩm Quyền Phê Duyệt
> - **Yêu cầu nghiệp vụ đã xác nhận**: Các quy tắc nghiệp vụ cốt lõi về thanh toán và tách hóa đơn đã được **CONFIRMED** theo các quyết định của Chủ nhà hàng: D4 / REV-04 (Phiên bàn đa hóa đơn & quyết toán hóa đơn con độc lập) và OD-13 đối với Phương án 2 của BillingRequested (hóa đơn chưa thanh toán vẫn được phép chỉnh sửa, các món gọi thêm được gắn vào hóa đơn mở hiện tại hoặc tạo hóa đơn con mới, hóa đơn đã thanh toán là bất biến). Quyết định D3 / REV-03 (Xử lý giao dịch VietQR đến muộn & sổ treo đối soát) cung cấp chính sách vận hành đã được duyệt khi chọn VietQR làm kênh thanh toán.
> - **Đề xuất kỹ thuật**: Thuật toán làm tròn toán học cụ thể (hấp thụ phần chênh lệch lẻ đơn vị Đồng vào hóa đơn con cuối cùng), luồng thao tác giao diện người dùng và ranh giới giao dịch cơ sở dữ liệu là các thiết kế kỹ thuật thuộc diện **PROPOSED**, chờ thẩm định của Kiến trúc sư giải pháp trong Stage 2.

---

## 4. Hệ Quả Nghiệp Vụ & Kỹ Thuật (Consequences)
- **Tích cực**: Thanh toán nhanh chóng, tin cậy; chuẩn xác về mặt toán học; không có sai lệch tiền lẻ; tích hợp VietQR mượt mà.
- **Tiêu cực**: Thu ngân cần hướng dẫn khách lựa chọn chia theo món hoặc chia đều thành các phần bằng nhau.

---

## 5. Câu Hỏi & Giả Định (Questions & Assumptions)
- *Câu hỏi*: Câu hỏi mở `OQ-01` theo dõi việc có nên xem xét lại tính năng tách hóa đơn theo tỷ lệ phần trăm tùy ý sau giai đoạn MVP hay không.
