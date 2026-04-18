using System;

namespace POS.Core.Models
{
    public enum TransactionType
    {
        PurchaseIn = 1,     // Nhập kho
        SalesOut = 2,       // Xuất bán (Hóa đơn)
        Adjustment = 3,     // Điều chỉnh (Mất mát/Hỏng/Kiểm kê)
        RefundIn = 4        // Khách trả lại hàng
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        // Số lượng dương là Nhập kho, Số lượng âm là Xuất kho
        public int QuantityChanged { get; set; }
        
        public TransactionType Type { get; set; }
        
        // Ghi lại mốc thời gian vĩnh viễn (Server Time UTC)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Ai đã thao tác?
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public string Notes { get; set; } = string.Empty;
    }
}
