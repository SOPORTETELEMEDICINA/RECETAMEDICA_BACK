using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Login
{
    /// <summary>Credenciales para autenticación.</summary>
    public class UserCredentials
    {
        [Required, MaxLength(50)]
        public string Usr { get; set; } = string.Empty;  // Usuario o email

        [Required, MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        /// <summary>Origen del login (WEB, APP, etc.).</summary>
        [MaxLength(20)]
        public string Plataform { get; set; } = "WEB";
    }
}