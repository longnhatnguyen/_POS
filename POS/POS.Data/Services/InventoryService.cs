using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Interfaces;

namespace POS.Data.Services;

public class InventoryService : IInventoryService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public InventoryService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products.Include(p => p.Category).ToListAsync();
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Update(product);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Dữ liệu sản phẩm đã bị thay đổi bởi thiết bị/người dùng khác. Vui lòng tải lại.");
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        bool isProductHaveTransactions = await context.InventoryTransactions.AnyAsync(t => t.ProductId == id);
            
        if (isProductHaveTransactions)
        {
            throw new Exception("HÀNG RÀO KIỂM TOÁN KÍCH HOẠT:\n\nSản phẩm này đã từng tham gia vào Dòng chảy Tiền/Hàng (Tồn tại thẻ kho nội bộ).\n\nDev Lead Antigravity Cấm Tuyệt Đối việc Vứt bỏ khỏi DB làm lệch sổ hóa đơn cũ. Yêu cầu thiết lập Thuộc tính [Ngừng Kinh Doanh] thay thế!");
        }

        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
