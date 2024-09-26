using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Representa un código postal.
    /// </summary>
    public class CP
    {
        [Key]
        public int IdCP { get; set; }

        [Required]
        [StringLength(10)]
        public string CodigoPostal { get; set; } = string.Empty;
    }
}
