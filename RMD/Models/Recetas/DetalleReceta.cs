using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Recetas
{
    public class DetalleReceta
    {
        [Key]
        public Guid IdDetalleReceta { get; set; }

        [Required]
        public Guid IdReceta { get; set; }

        [Required]
        [MaxLength(255)]
        public string Medicamento { get; set; } = string.Empty;

        [Required]
        public decimal CantidadDiaria { get; set; }

        [Required]
        [MaxLength(50)]
        public string UnidadDispensacion { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RutaAdministracion { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Indicacion { get; set; } = string.Empty;

        [Required]
        public int Duracion { get; set; }

        [Required]
        [MaxLength(50)]
        public string UnidadDuracion { get; set; } = string.Empty;

        [Required]
        public DateTime PeriodoInicio { get; set; }

        public DateTime? PeriodoTerminacion { get; set; }
    }
}
