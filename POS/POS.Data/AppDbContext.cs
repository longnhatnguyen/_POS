using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Security;

namespace POS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=app.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Giới hạn unique Username
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();

            // Seed Data: Master Admin Account
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = PasswordHasher.HashPassword("admin123"),
                FullName = "System Administrator",
                Role = "Master",
                IsActive = true
            });

            // Seed Data: Danh mục hàng hóa
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Đồ uống", Description = "Nước giải khát, bia, sữa..." },
                new Category { Id = 2, Name = "Đồ ăn vặt", Description = "Bánh kẹo, snack..." }
            );

            // Seed Data: Hàng hóa (Tồn kho mặc định lúc setup là 100 và 50)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, SKU = "SP001", Barcode = "893123450001", Name = "Nước suối Aquafina 500ml", CategoryId = 1, CostPrice = 3000, BasePrice = 5000, StockQuantity = 100, LowStockThreshold = 10 },
                new Product { Id = 2, SKU = "SP002", Barcode = "893123450002", Name = "Bánh Snack Oishi", CategoryId = 2, CostPrice = 4000, BasePrice = 6000, StockQuantity = 50, LowStockThreshold = 5 }
            );
        }
    }
}
