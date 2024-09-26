using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Pacientes
{
    public class PacienteCreateConListas
    {
        public Guid IdUsuario { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public int IdEntidadNacimiento { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genero { get; set; } = string.Empty;

        public List<string>? Alergias { get; set; } = new List<string>();

        public List<string>? Molecules { get; set; } = new List<string>();

        public List<string>? Patologias { get; set; } = new List<string>();
    }

}
