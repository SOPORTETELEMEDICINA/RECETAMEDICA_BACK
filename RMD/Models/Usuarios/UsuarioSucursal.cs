using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    public class UsuarioSucursal
    {
        [Key]
        public Guid IdUsuario { get; set; }
        [Key]
        public Guid IdSucursal { get; set; }
    }
}
