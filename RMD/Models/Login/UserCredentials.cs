using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Login
{
    public class UserCredentials
    {
        public UserCredentials()
        {
            Usr = string.Empty;
            Password = string.Empty;
        }

        [Required]
        [MaxLength(50)]
        public string Usr { get; set; }  // Nombre de usuario

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }  // Contraseña
    }
}
