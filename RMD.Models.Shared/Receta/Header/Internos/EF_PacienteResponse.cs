namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class EF_PacienteResponse
    {
        public Guid IdPaciente { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? IdEntidadNacimiento { get; set; }
        public char? Genero { get; set; }
        public string? Alergias { get; set; }
        public string? Molecules { get; set; }
        public string? Patologias { get; set; }
        public Guid? IdMedico { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }

}
