using System.Security.Cryptography;
using System.Text;

namespace POS.Core.Security
{
    public static class PasswordHasher
    {
        // Sử dụng thuật toán chuẩn Enterprise SHA-256 để băm
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) 
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = HashPassword(inputPassword);
            return string.Equals(hashOfInput, storedHash, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
