namespace POS.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        // Mã nội bộ (VD: SP001)
        public string SKU { get; set; } = string.Empty;
        
        // Mã vạch siêu thị (VD: 893123456789)
        public string Barcode { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        
        // Giá vốn
        public decimal CostPrice { get; set; }
        
        // Giá bán lẻ
        public decimal BasePrice { get; set; }
        
        // Cột tồn kho - Cấm sửa trực tiếp, phải được tính toán dựa vào Log
        public int StockQuantity { get; set; }
        
        // Ngưỡng báo động hụt kho
        public int LowStockThreshold { get; set; } = 5;
    }
}
