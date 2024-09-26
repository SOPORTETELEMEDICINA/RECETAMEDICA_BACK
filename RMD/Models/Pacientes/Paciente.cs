using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Pacientes
{
    public class Paciente
    {
        [Key]
        public Guid IdPaciente { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public int IdEntidadNacimiento { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genero { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Alergias { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Molecules { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Patologias { get; set; } = string.Empty;

        [Required]
        public Guid IdMedico { get; set; }

    }
}
