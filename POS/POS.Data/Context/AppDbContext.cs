using Microsoft.EntityFrameworkCore;
using POS.Core.Entities;

namespace POS.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed some initial Categories for convenience
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Đồ uống", Description = "Các loại nước giải khát" },
            new Category { Id = 2, Name = "Đồ ăn vặt", Description = "Bánh kẹo, snack" }
        );
    }
}
