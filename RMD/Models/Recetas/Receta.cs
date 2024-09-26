using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Recetas
{
    public class Receta
    {
        [Key]
        public Guid IdReceta { get; set; }

        [Required]
        public Guid IdMedico { get; set; }

        [Required]
        public Guid IdPaciente { get; set; }

        [Required]
        public decimal PacPeso { get; set; }

        [Required]
        public decimal PacTalla { get; set; }

        [Required]
        public bool PacEmbarazo { get; set; }

        public int? PacSemAmenorrea { get; set; }

        [Required]
        public bool PacLactancia { get; set; }

        public decimal? PacCreatinina { get; set; }

        public bool? PacAlergiaClase { get; set; }
        public bool? PacAlergiaMolecula { get; set; }

        [MaxLength(50)]
        public string PacDx1 { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PacDx2 { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PacDx3 { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PacDx4 { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PacDx5 { get; set; } = string.Empty;
    }
}
