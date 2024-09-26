using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Representa una entidad federativa.
    /// </summary>
    public class Entidad
    {
        [Key]
        public int IdEntidad { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreEntidad { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string AbreviaturaEntidad { get; set; } = string.Empty;
    }
}
