using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    public class UsuarioImagenRequest
    {
        [Key] // Define una propiedad como clave primaria
        public Guid IdUsuario { get; set; }
        public string? Imagen { get; set; } // Base64 string
        public string? Firma { get; set; } // Base64 string
    }

}
