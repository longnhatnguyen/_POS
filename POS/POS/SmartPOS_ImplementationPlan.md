# Lộ trình kỹ thuật: Smart POS WPF Project

Kế hoạch này tập trung vào việc xây dựng bộ khung ứng dụng bền bỉ, dễ mở rộng và tuân thủ các chuẩn mực Enterprise.

---

## Giai đoạn 1: Kiến trúc & Cơ sở dữ liệu
Thiết lập "bộ xương" cho ứng dụng.

### Cấu trúc Project (Solution Structure)
- **SmartPOS.Core:** (Class Library) Chứa các thực thể (Product, Order), Interfaces và Logic nghiệp vụ thuần túy.
- **SmartPOS.Data:** (Class Library) Sử dụng Entity Framework Core hoặc Dapper với **SQLite** để quản lý dữ liệu.
- **SmartPOS.Desktop:** (WPF App) Chức giao diện người dùng (Views, ViewModels).

### Lược đồ cơ sở dữ liệu (Database Schema - SQLite)
- `Products`: ID, SKU, Name, BasePrice, CostPrice, Barcode, StockQuantity.
- `Categories`: Phân loại hàng hóa.
- `Orders`: ID, OrderDate, TotalAmount, CustomerID, CashierID.
- `OrderDetails`: OrderID, ProductID, Quantity, PriceAtSale.

---

## Giai đoạn 2: Core Infrastructure (Cơ sở hạ tầng)
Đảm bảo ứng dụng chạy 24/7 không lỗi.

### Lớp Dịch vụ (Service Layer)
- **InventoryService:** Xử lý nhập/xuất kho với cơ chế `lock` để tránh tranh chấp dữ liệu khi có nhiều tác vụ cùng lúc.
- **SalesService:** Logic tính toán giá, quản lý giỏ hàng tạm thời.
- **LoggingService:** Sử dụng **Serilog** để ghi lại mọi sự kiện vào file cứng (.log).

---

## Giai đoạn 3: UI Development (WPF/XAML)
Thiết kế giao diện hiện đại, tối ưu cho tốc độ.

### Các màn hình chính
- **Dashboard:** Thống kê nhanh doanh thu trong ngày.
- **Sales Terminal:** Màn hình chính cho thu ngân với ô nhập Barcode siêu nhanh.
- **Inventory Manager:** Màn hình quản lý danh mục hàng hóa.

---

## Giai đoạn 4: Tính năng nâng cao & Ổn định
- **Print Service:** In hóa đơn ra file PDF hoặc máy in hóa đơn (ESC/POS).
- **Global Exception Handler:** Bắt mọi lỗi runtime để hiện thông báo lịch sự thay vì crash app.
