using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void navDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardContent.Visibility = Visibility.Visible;
            DynamicContent.Visibility = Visibility.Collapsed;
            DynamicContent.Content = null; // Giải phóng bộ nhớ
        }

        private void navInventory_Click(object sender, RoutedEventArgs e)
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            // Gọi màn hình Kho
            DynamicContent.Content = new POS.Views.InventoryView();
            DynamicContent.Visibility = Visibility.Visible;
        }
    }
}