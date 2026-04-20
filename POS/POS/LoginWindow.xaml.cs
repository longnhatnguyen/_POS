using System.Linq;
using System.Windows;
using POS.Data;
using POS.Core.Security;

namespace POS
{
    public partial class LoginWindow : Wpf.Ui.Controls.FluentWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Vui lòng nhập đủ thông tin!");
                return;
            }

            // Gọi DB kiểm tra trực tiếp (Bản chất chuẩn nên dùng qua UserService/Repository, ở đây làm tắt cho giai đoạn đầu)
            using (var db = new AppDbContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    ShowError("Sai tài khoản hoặc mật khẩu!");
                    return;
                }

                if (!user.IsActive)
                {
                    ShowError("Tài khoản đã bị vô hiệu hóa!");
                    return;
                }

                bool isMatch = PasswordHasher.VerifyPassword(password, user.PasswordHash);
                if (!isMatch)
                {
                    ShowError("Sai tài khoản hoặc mật khẩu!");
                    return;
                }

                // Nếu đăng nhập thành công
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                MessageBox.Show(
                    "SMART POS - Enterprise Edition\n" +
                    "Version 1.0.0 (Alpha Build)\n\n" +
                    "Phát triển bởi: Chuyên gia hệ thống Antigravity\n" +
                    "Hỗ trợ tính năng: \n- Bảo mật 2 lớp (SHA-256)\n- Quản lý kho Atomic Transaction\n- Chống lỗi trôi hóa đơn", 
                    "Giới thiệu Phần mềm", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }
    
    }
}
