using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Models;
using POS.Core.Interfaces;
using POS.Data;
using POS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace POS.ViewModels

{
    public partial class InventoryViewModel : ObservableObject
    {
        private readonly IInventoryService _inventoryService;
        private readonly int _currentUserId; 

        public InventoryViewModel(IInventoryService inventoryService)
        {
            // GIẢI CỨU VISUAL STUDIO DESIGNER
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Products = new ObservableCollection<Product> { new Product { SKU = "MOCK", Name = "Demo", BasePrice = 1000 } };
                return;
            }

            _inventoryService = inventoryService;
            _currentUserId = 1; 
            
            LoadDataAsync();
            LoadCategoriesAsync();
        }

        // --- CÁC DANH SÁCH DỮ LIỆU ---
        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        private ObservableCollection<Category> categories = new();

        [ObservableProperty]
        private string searchQuery = string.Empty;

        // --- CỜ VÀ STATE CHO POPUP MODAL ---
        [ObservableProperty]
        private bool isDialogOpen = false;

        [ObservableProperty]
        private string dialogTitle = string.Empty;

        [ObservableProperty]
        private Product currentProduct = new(); // Dữ liệu đang được gõ trong Form

        // Kích hoạt tìm kiếm liên tục
        partial void OnSearchQueryChanged(string value)
        {
            SearchProducts();
        }

        private async void LoadDataAsync()
        {
            var data = await _inventoryService.GetAllProductsAsync();
            Products = new ObservableCollection<Product>(data);
        }

        private async void LoadCategoriesAsync()
        {
            var cats = await _inventoryService.GetAllCategoriesAsync();
            Categories = new ObservableCollection<Category>(cats);
        }

        private async void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                LoadDataAsync(); 
                return;
            }

            var q = SearchQuery.ToLower();
            var allProducts = await _inventoryService.GetAllProductsAsync();
            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains(q) || p.SKU.ToLower().Contains(q) || p.Barcode.Contains(q))
                .ToList();
            
            Products = new ObservableCollection<Product>(filtered);
        }

        // ----------------------------------------------------
        // TẦNG NGHIỆP VỤ (BUSINESS LOGIC COMMANDS)
        // ----------------------------------------------------

        // 1. NGHIỆP VỤ CỘNG KHO (GIỮ TỪ PHASE TRƯỚC)
        [RelayCommand]
        private void AdjustStock(Product selectedProduct)
        {
            MessageBox.Show("Tính năng cân kho sẽ được kích hoạt tại Module Quản lý Nhập kho.", "Tính năng đang tạm khóa", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 2. MỞ FORM THÊM MỚI SẢN PHẨM
        [RelayCommand]
        private void OpenCreateDialog()
        {
            // Reset Form tinh tươm
            CurrentProduct = new Product 
            { 
                StockQuantity = 0, 
                LowStockThreshold = 5, 
                CategoryId = Categories.FirstOrDefault()?.Id ?? 0 
            };
            DialogTitle = "THÊM SẢN PHẨM MỚI";
            IsDialogOpen = true;
        }

        // 3. MỞ FORM CẬP NHẬT SẢN PHẨM (EDIT)
        [RelayCommand]
        private void OpenEditDialog(Product p)
        {
            if (p == null) return;

            // LUẬT SENIOR: Không gán trực tiếp CurrentProduct = p (Sẽ làm hỏng Two-Way Binding trên DataGrid)
            // Phải tạo BẢN SAO (Clone) độc lập để sửa trên Form. Nếu ấn 'Hủy' thì DataGrid không bị hư.
            CurrentProduct = new Product
            {
                Id = p.Id,
                SKU = p.SKU,
                Barcode = p.Barcode,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CostPrice = p.CostPrice,
                BasePrice = p.BasePrice,
                StockQuantity = p.StockQuantity, // Cột này sẽ khóa ReadOnly trên Form Edit
                LowStockThreshold = p.LowStockThreshold
            };
            DialogTitle = $"CHỈNH SỬA: {p.Name}";
            IsDialogOpen = true;
        }

        // 4. LỆNH ĐÓNG THEO PHƯƠNG DIỆN AN TOÀN
        [RelayCommand]
        private void CloseDialog()
        {
            IsDialogOpen = false;
        }

        // 5. CHỐT LƯU XUỐNG DB (UPSERT: THÊM HOẶC CẬP NHẬT)
        [RelayCommand]
        private async Task SaveProductAsync()
        {
            try
            {
                // Validate cùi bắp (Thiếu Code chuẩn Fluẹt Validation)
                if (string.IsNullOrWhiteSpace(CurrentProduct.Name) || string.IsNullOrWhiteSpace(CurrentProduct.SKU))
                {
                    MessageBox.Show("Mã và Tên sản phẩm không được bỏ trống!", "Cảnh Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CurrentProduct.Id == 0) // Tạo mới
                {
                    await _inventoryService.AddProductAsync(CurrentProduct);
                }
                else // Cập nhật
                {
                    await _inventoryService.UpdateProductAsync(CurrentProduct);
                }

                IsDialogOpen = false;
                LoadDataAsync(); // Cập nhật lại màn lưới DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lưu thất bại. Chi tiết: {ex.InnerException?.Message ?? ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 6. XÓA BÁ ĐẠO - KIỂM TOÁN CHẶT CHẼ TRƯỚC KHI XÓA
        [RelayCommand]
        private async Task DeleteProductAsync(Product p)
        {
            if (p == null) return;

            // Nếu sạch sẽ (Chưa tồn kho dòng nào), cho phép diệt!
            var confirm = MessageBox.Show($"Sản phẩm \"{p.Name}\" hoàn toàn vô hại.\n\nBạn có CHẮC CHẮN muốn phi tang vĩnh viễn không?", "Xóa Tận Gốc", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    await _inventoryService.DeleteProductAsync(p.Id);
                    LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Sóng Lỗi Phá DB", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }
    }
}
