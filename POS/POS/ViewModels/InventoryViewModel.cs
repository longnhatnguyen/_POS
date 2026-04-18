using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Models;
using POS.Data;
using POS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;

namespace POS.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        private readonly AppDbContext _context;
        private readonly InventoryRepository _inventoryRepo;
        private readonly int _currentUserId; 

        public InventoryViewModel()
        {
            // [GIẢI CỨU VISUAL STUDIO DESIGNER]: Cấp Dữ liệu Giả (Mock Data) để vẽ giao diện rõ ràng
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Products = new ObservableCollection<Product>
                {
                    new Product { SKU = "MOCK-001", Barcode = "893111", Name = "Nước Mắm Giả Lập (Bản Design)", BasePrice = 45000, StockQuantity = 150, Category = new Category { Name = "Gia Vị" } },
                    new Product { SKU = "MOCK-002", Barcode = "893222", Name = "Bia Heineken Lốc 6 Lon", BasePrice = 120000, StockQuantity = 24, Category = new Category { Name = "Đồ uống" } }
                };
                return;
            }

            // Trong luồng Enterprise, thường ta sẽ tiêm Injection qua lớp App.xaml.cs
            _context = new AppDbContext();
            _inventoryRepo = new InventoryRepository(_context);
            _currentUserId = 1; // Mặc định giả lập Admin đang đăng nhập
            
            LoadDataAsync();
        }

        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        private string searchQuery = string.Empty;

        // AutoTrigger khi người dùng gõ vào ô TÌM KIẾM
        partial void OnSearchQueryChanged(string value)
        {
            SearchProducts();
        }

        private async void LoadDataAsync()
        {
            var data = await _context.Products.Include(p => p.Category).ToListAsync();
            Products = new ObservableCollection<Product>(data);
        }

        private void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                LoadDataAsync(); 
                return;
            }

            var q = SearchQuery.ToLower();
            var filtered = _context.Products.Include(p => p.Category)
                .Where(p => p.Name.ToLower().Contains(q) || p.SKU.ToLower().Contains(q) || p.Barcode.Contains(q))
                .ToList();
            
            Products = new ObservableCollection<Product>(filtered);
        }

        [RelayCommand]
        private async Task AdjustStockAsync(Product selectedProduct)
        {
            if (selectedProduct == null) return;

            // Xử lý giả lập giao diện Dialog Nhập Số lượng - Cốt lõi là GHI TRANSACTION!
            string reason = "Bổ sung hàng hóa đầu ngày (Bản Demo)";
            int diffQty = 10; 
            
            try
            {
                bool success = await _inventoryRepo.AdjustStockAsync(selectedProduct.Id, diffQty, TransactionType.PurchaseIn, _currentUserId, reason);
                
                if (success)
                {
                    MessageBox.Show($"Đã CỘNG TỰ ĐỘNG +10 ({selectedProduct.Name}) vào kho.\n\nHệ thống đã lưu lại phiếu thao tác vĩnh viễn (Transaction) kèm chữ ký chống gian lận!", "Quyền lực của Entity Framework", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataAsync(); // Cập nhật lại Lưới Lưới
                }
            }
            catch(System.Exception ex)
            {
                MessageBox.Show($"Lỗi chặn Đổ vỡ hệ thống: {ex.Message}", "Cảnh Báo Nặng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private void CreateProduct()
        {
            MessageBox.Show("Nghiệp vụ Tạo sản phẩm mới (Mở Popup nhập mã/định giá) sẽ được phát triển ở nhánh tiếp theo!", "Tính năng", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
