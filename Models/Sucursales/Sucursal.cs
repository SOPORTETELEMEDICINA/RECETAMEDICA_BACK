using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Sucursales
{
    public class Sucursal
    {
        [Key]
        public Guid IdSucursal { get; set; }

        [Required]
        public Guid IdGEMP { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RegistroSanitario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Responsable { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CedulaResponsable { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string TelefonoResponsable { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EmailResponsable { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Domicilio { get; set; } = string.Empty;

        [Required]
        public int IdAsentamiento { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
