using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Representa un municipio.
    /// </summary>
    public class Municipio
    {
        [Key]
        public int IdMunicipio { get; set; }

        [Required]
        [StringLength(50)]
        public short NoMunicipio { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreMunicipio { get; set; } = string.Empty;
    }
}
