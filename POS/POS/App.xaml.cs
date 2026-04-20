using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Data.Services;
using POS.Core.Interfaces;
using POS.ViewModels;
using POS.Views;

namespace POS
{
    public partial class App : Application
    {
        private static IHost? _host;
        
        public static IServiceProvider ServiceProvider => _host!.Services;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Đăng ký DBContext theo mô hình Factory để tránh leak Memory trên Desktop
                    services.AddDbContextFactory<AppDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=app.db");
                    });

                    // Đăng ký Services
                    services.AddSingleton<IInventoryService, InventoryService>();
                    // services.AddSingleton<InventoryRepository>(); // Nếu cần

                    // Đăng ký ViewModels
                    services.AddTransient<InventoryViewModel>();

                    // Đăng ký Views
                    services.AddTransient<InventoryView>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<LoginWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await _host!.StartAsync();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Đảm bảo CSDL được tạo (Nơi EF Core sinh ra app.db)
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var db = await dbFactory.CreateDbContextAsync();
                await db.Database.MigrateAsync();
            }

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host!.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}
