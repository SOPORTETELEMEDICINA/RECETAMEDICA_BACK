namespace RMD.Models.PuntoVenta
{
    public class PacienteModel
    {
        public Guid IdPaciente { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? IdEntidadNacimiento { get; set; }
        public string Genero { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Patologias { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }

        // Datos del usuario asociado al paciente
        public string NombreUsuario { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Email { get; set; }
        public string Movil { get; set; }
    }

}
