using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using System;
using System.Threading.Tasks;

namespace POS.Data.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hàm Cân kho an toàn (Atomic Transaction). Không ném ngoại lệ nếu không có lỗi thực tế.
        /// diffQty: Số lượng TĂNG THÊM (dương) hoặc TRỪ ĐI (âm).
        /// </summary>
        public async Task<bool> AdjustStockAsync(int productId, int diffQty, TransactionType type, int userId, string notes)
        {
            // Bắt đầu một Transaction: Tất cả thành công hoặc tất cả thất bại
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return false; // Không tìm thấy sản phẩm

                // 1. Tạo bản ghi giao dịch
                var invTrans = new InventoryTransaction
                {
                    ProductId = productId,
                    QuantityChanged = diffQty,
                    Type = type,
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    Notes = notes
                };
                _context.InventoryTransactions.Add(invTrans);

                // 2. Chỉnh sửa cột Kho (Phải khớp với số lượng thay đổi)
                product.StockQuantity += diffQty;

                // 3. Đóng đinh (Save) và Chốt (Commit)
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                // Nếu có bất kì lỗi gì (mất mạng, dead lock...), Rollback dọn dẹp sạch sẽ
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
