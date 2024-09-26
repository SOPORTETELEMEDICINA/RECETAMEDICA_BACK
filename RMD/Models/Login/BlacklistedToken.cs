using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Login
{
    public class BlacklistedToken
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
    }
}
