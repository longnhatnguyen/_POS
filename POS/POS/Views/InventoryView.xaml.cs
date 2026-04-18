using System.Windows.Controls;
using POS.ViewModels;

namespace POS.Views
{
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();

            // Rút dây nguồn DataContext khỏi XAML và đẩy vào Code-Behind.
            // Điều này ép trình thiết kế XAML của Visual Studio không được chạm tay vào Database.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = new InventoryViewModel();
            }
        }
    }
}
