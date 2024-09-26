// Asentamiento.cs
using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    public class Asentamiento
    {
        [Key]
        public int IdAsentamiento { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAsentamiento { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreAsentamiento { get; set; } = string.Empty;
    }
}
