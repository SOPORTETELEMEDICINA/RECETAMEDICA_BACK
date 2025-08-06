using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Pacientes.Request
{
    public class PacienteRequest
    {
        [Key]
        public Guid IdPaciente { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }

        public int IdEntidadNacimiento { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genero { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string Alergias { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string Molecules { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string Patologias { get; set; } = string.Empty;

        [Required]
        public Guid IdMedico { get; set; }
        
    }
}
