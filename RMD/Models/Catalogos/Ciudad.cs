using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Representa una ciudad.
    /// </summary>
    public class Ciudad
    {
        [Key]
        public int IdCiudad { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCiudad { get; set; } = string.Empty;
    }
}
