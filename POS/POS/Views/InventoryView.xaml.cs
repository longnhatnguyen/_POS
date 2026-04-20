using System.Windows.Controls;
using POS.ViewModels;

namespace POS.Views
{
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();

            // Móc DataContext từ bộ DI Container của App ra để giải quyết Dependencies
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<InventoryViewModel>(App.ServiceProvider);
            }
        }
    }
}
