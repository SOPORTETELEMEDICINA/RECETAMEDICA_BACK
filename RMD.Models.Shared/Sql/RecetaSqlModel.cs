using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Sql
{

    [Table("Receta", Schema = "dbo")]
    public class RecetaSqlModel
    {
        [Key]
        [Column("IdReceta")]
        public Guid IdReceta { get; set; }

        [Required]
        public Guid IdMedico { get; set; }

        [Required]
        public Guid IdPaciente { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PacPeso { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PacTalla { get; set; }

        [Required]
        public bool PacEmbarazo { get; set; }

        public int? PacSemAmenorrea { get; set; }

        [Required]
        public bool PacLactancia { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PacCreatinina { get; set; }

        public string Alergias { get; set; }

        public string Molecules { get; set; }

        public string Patologias { get; set; }

        [Required]
        public Guid IdSucursal { get; set; }

        [Required]
        public Guid IdGEMP { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaUltimaModificacion { get; set; }

        [Required]
        public int Estatus { get; set; }
    }
}
