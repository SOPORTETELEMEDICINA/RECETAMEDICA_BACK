namespace RMD.Models.Pacientes
{
    public class PacienteRequest
    {
        public Guid IdPaciente { get; set; }
        public string Nombres { get; set; } = string.Empty; 
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; } = string.Empty;
        public int IdAsentamiento { get; set; }
        public string Asentamiento { get; set; } = string.Empty;
        public string TipoAsentamiento { get; set; } = string.Empty;
        public int IdCiudad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public int IdMunicipio { get; set; }
        public string Municipio { get; set; } = string.Empty;
        public int IdEntidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Movil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string Molecules { get; set; } = string.Empty;
        public string Patologias { get; set; } = string.Empty;
    }
}
