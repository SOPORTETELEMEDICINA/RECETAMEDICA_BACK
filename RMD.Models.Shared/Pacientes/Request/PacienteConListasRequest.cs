using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Pacientes.Request
{
    public class PacienteConListasRequest
    {
        [Key]
        public Guid IdPaciente { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public int IdEntidadNacimiento { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genero { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public List<string> Alergias { get; set; }
        [MaxLength(999999999)]
        public List<string> Molecules { get; set; } 
        [MaxLength(999999999)]
        public List<string> Patologias { get; set; } 

        //[Required]
        //public Guid IdMedico { get; set; }

    }
}
