using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Medicos
{
    public class Medico
    {
        [Key]
        public Guid IdMedico { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string CedulaGeneral { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Universidad { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CedulaEspecialidad { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Horario { get; set; } = string.Empty;
    }
}
