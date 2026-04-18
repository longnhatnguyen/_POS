using System.Windows;
using Microsoft.EntityFrameworkCore;
using POS.Data;

namespace POS
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // [LỖI KINH ĐIỂN CỦA WPF]: Tắt chế độ 'Tự tử' mặc định 
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Đảm bảo CSDL được tạo (Nơi EF Core sinh ra app.db)
            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
            }

            // Mở Màn hình đăng nhập
            var loginWindow = new LoginWindow();
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                // Nếu hàm Login trả về True -> Mở Dashboard
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                // Bấm dấu X tắt form Login hoặc thoát -> Tắt hoàn toàn Process
                Application.Current.Shutdown();
            }
        }
    }
}
