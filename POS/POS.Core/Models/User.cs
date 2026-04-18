namespace POS.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public string Username { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Vai trò: Master, Admin, Cashier
        /// </summary>
        public string Role { get; set; } = "Cashier";
        
        public bool IsActive { get; set; } = true;
    }
}
