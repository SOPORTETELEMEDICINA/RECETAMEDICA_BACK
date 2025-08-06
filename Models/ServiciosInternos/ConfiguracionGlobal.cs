using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMD.Models.ServiciosInternos
{
    [Table("ConfiguracionGlobal", Schema = "Configuracion")]
    public class ConfiguracionGlobal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Clave { get; set; }

        [Required]
        public string Valor { get; set; }

        [MaxLength(255)]
        public string Descripcion { get; set; }

        [MaxLength(50)]
        public string TipoDato { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }
    }


}
