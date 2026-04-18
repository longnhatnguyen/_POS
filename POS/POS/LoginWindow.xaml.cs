using System.Linq;
using System.Windows;
using POS.Data;
using POS.Core.Security;

namespace POS
{
    public partial class LoginWindow : Window
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

    
    }
}
