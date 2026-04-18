using Microsoft.EntityFrameworkCore;
using POS.Core.Models;
using POS.Core.Security;

namespace POS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Nếu chưa được cấu hình từ bên ngoài (như DI), thì dùng chuỗi nội bộ này (cực kỳ tiện cho lúc gõ lệnh cmd tạo Migration)
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=app.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Giới hạn unique Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

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
        }
    }
}
