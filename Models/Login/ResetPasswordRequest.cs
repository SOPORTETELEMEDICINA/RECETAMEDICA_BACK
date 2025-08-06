using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Login
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }

}
