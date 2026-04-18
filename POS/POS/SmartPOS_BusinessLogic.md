# 🛒 Smart POS: Nghiệp vụ chuyên sâu cho Nhà phát triển

Bản tài liệu này phân tích chi tiết các quy trình nghiệp vụ cốt lõi của một hệ thống POS (Point of Sale) chuyên nghiệp, nhấn mạnh vào các quy tắc để đảm bảo hệ thống chạy 24/7 mà không làm mất dữ liệu.

---

## 1. Quản lý Kho (Inventory Management)
Kho không đơn thuần là một con số "số lượng còn lại".

### A. Vòng đời dữ liệu sản phẩm
- **SKU (Stock Keeping Unit):** Mỗi sản phẩm phải có một mã duy nhất do cửa hàng tự định nghĩa.
- **Barcode (Mã vạch):** Mã vạch của nhà sản xuất. Một SKU có thể có nhiều Barcode (ví dụ: chai nước lọc lẻ và một lốc nước lọc).
- **Giá vốn (Cost Price):** Dùng để tính lợi nhuận. Giá vốn có thể thay đổi theo từng đợt nhập hàng (Cơ chế FIFO - Nhập trước xuất trước).
- **Đơn vị quy đổi:** Ví dụ: Nhập 1 Thùng (24 lon), bán lẻ 1 Lon. Logic kho phải tự động trừ kho 1/24 thùng hoặc quản lý theo đơn vị nhỏ nhất.

### B. Kiểm kê & Hao hụt
- **Inventory Adjustment:** Cho phép nhân viên điều chỉnh số lượng kho khi có sự cố (hỏng hóc, mất mát) nhưng **BẮT BUỘC** phải ghi lại lý do và người thực hiện.

---

## 2. Quy trình Bán hàng (The Checkout Flow)

### A. Trạng thái Giỏ hàng (The Cart Logic)
Mọi thay đổi trong giỏ hàng (thêm món, bớt món, đổi số lượng) phải được tính toán lại ngay lập tức:
1.  **Line Total:** `Quantity * Unit Price`
2.  **Line Discount:** Giảm giá trên từng món đồ (ví dụ: Mua 2 tặng 1).
3.  **Subtotal:** Tổng tiền trước thuế và giảm giá chung.
4.  **Grand Total:** `Subtotal - Order Discount + Tax`.

### B. Cơ chế Hóa đơn chờ (Pending Orders)
Tại quầy, thu ngân thường gặp khách hàng "quên ví" hoặc "chờ lấy thêm đồ". Hệ thống phải cho phép "treo" (Hold) hóa đơn hiện tại để phục vụ người tiếp theo mà không làm mất dữ liệu.

---

## 3. Thanh toán & Tính toàn vẹn dữ liệu (Payment & Integrity)

### A. Giao dịch nguyên tử (Atomic Transaction)
Khi bấm "Thanh toán", 4 việc sau **PHẢI** cùng thành công hoặc cùng thất bại:
1.  Lưu `SalesHeader` (Thông tin chung hóa đơn).
2.  Lưu `SalesDetails` (Danh sách món đồ đã bán).
3.  Cập nhật số dư kho trong bảng `Inventory`.
4.  Lưu log thanh toán (Tiền mặt/Chuyển khoản).

> LƯU Ý PHẦN MỀM: Nếu chỉ lưu hóa đơn mà chưa trừ kho, hệ thống sẽ báo sai hàng tồn, gây thiệt hại cho cửa hàng.

### B. Kết ca (Shift Closure)
Thu ngân khi bàn giao ca phải thực hiện:
- Kiểm tiền thực tế trong két.
- Đối chiếu với số liệu "Tiền dự kiến" trên hệ thống.
- In biên bản kết ca (Z-Report).

---

## 4. Bảo mật & Vai trò (Security)
Hệ thống POS trong thực tế cần phân quyền:
- **Nhân viên:** Chỉ bán hàng, không được sửa giá, không được xóa hóa đơn đã xuất.
- **Quản lý:** Được phép "Void" (Hủy) hóa đơn, duyệt các mức giảm giá lớn, sửa thông tin tồn kho.
