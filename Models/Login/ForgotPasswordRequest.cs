using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Login
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
