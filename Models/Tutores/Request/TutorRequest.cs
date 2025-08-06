using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Tutores.Request
{
    public class TutorRequest
    {
        [Required]
        public Guid IdPaciente { get; set; }

        [Required, MaxLength(128)]
        public string NombreTutor { get; set; }

        [Required, MaxLength(128)]
        public string PrimerApellidoTutor { get; set; }

        [MaxLength(128)]
        public string SegundoApellidoTutor { get; set; }

        [Required]
        [RegularExpression("1|2", ErrorMessage = "TipoIdentificacion inválido (1 = DNI, 2 = NIE)")]
        public string TipoIdentificacionTutor { get; set; }

        [Required, StringLength(9, MinimumLength = 9)]
        public string NumeroIdentificacionTutor { get; set; }

        [RegularExpression(@"\d{2}/\d{2}/\d{4}", ErrorMessage = "Formato de fecha inválido (DD/MM/YYYY)")]
        public string? FechaNacimientoTutor { get; set; }

        [Range(1, 2)]
        public int? SexoTutor { get; set; }

        [EmailAddress, MaxLength(256)]
        public string? EmailTutor { get; set; }

        [Phone, MaxLength(256)]
        public string? TelefonoTutor { get; set; }
    }
}
