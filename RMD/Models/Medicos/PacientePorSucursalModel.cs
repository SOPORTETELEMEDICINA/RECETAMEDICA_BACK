namespace RMD.Models.Medicos
{
    public class PacientePorSucursalModel
    {
        public Guid IdUsuario { get; set; }
        public Guid IdPaciente { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public string LogoGEMP { get; set; }
        public string GEMP { get; set; }
        public string Sucursal { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string FechaNacimiento { get; set; }
        public int Edad {  get; set; }
        public string Genero { get; set; }
        public int IdEntidad { get; set; }
        public string EntidadNacimiento { get; set; }
        public string Domicilio { get; set; }
        public int? IdAsentamiento { get; set; }
        public string Asentamiento { get; set; }
        public string TipoAsentamiento { get; set; }
        public int? IdCP { get; set; }
        public string CodigoPostal { get; set; }
        public int? IdMunicipio { get; set; }
        public string Municipio { get; set; }
        public int? IdCiudad { get; set; }
        public string Ciudad { get; set; }
        public string Entidad { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public string Patologias { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Status { get; set; }
    }
}
