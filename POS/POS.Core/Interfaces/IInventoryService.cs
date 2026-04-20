using POS.Core.Models;

namespace POS.Core.Interfaces;

public interface IInventoryService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
