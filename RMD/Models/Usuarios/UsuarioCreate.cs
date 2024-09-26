using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    public class UsuarioCreate
    {
        [Required]
        [MaxLength(50)]
        public string? Usr { get; set; }  // Actualizado a Usuario

        [Required]
        [MaxLength(255)]  // Actualizado a 255 caracteres para almacenar contraseñas cifradas
        public string? Password { get; set; }

        [Required]
        public Guid IdTipoUsuario { get; set; }

        public Guid? IdGEMP { get; set; }
        public Guid? IdSucursal { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Nombres { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PrimerApellido { get; set; }

        [MaxLength(50)]
        public string? SegundoApellido { get; set; }

        public int? IdAsentamiento { get; set; }

        [MaxLength(200)]
        public string? Domicilio { get; set; }

        [MaxLength(15)]
        public string? Movil { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

    }
}
