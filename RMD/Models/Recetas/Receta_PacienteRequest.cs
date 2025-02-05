namespace RMD.Models.Recetas
{
    public class Receta_PacienteRequest
    {
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public string NombresMedico { get; set; }
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public string Universidad { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public string Horario { get; set; }
        public string Firma { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombresPaciente { get; set; }
        public string PrimerApellidoPaciente { get; set; }
        public string SegundoApellidoPaciente { get; set; }
        public int EdadPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public string Genero { get; set; }
        public decimal PacTalla { get; set; }
        public Guid IdGEMP { get; set; }
        public string NombreGrupoEmpresarial { get; set; }
        public string LogoBase64 { get; set; }
        public Guid IdSucursal { get; set; }
        public string NumeroSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string RegistroSanitario { get; set; }
        public string Domicilio { get; set; }
        public int IdAsentamiento { get; set; }
        public string NombreAsentamiento { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public string TipoAsentamiento { get; set; }
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; }
        public int IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; }
        public int IdCiudad { get; set; }
        public string NombreCiudad { get; set; }
        public int IdEntidad { get; set; }
        public string Estado { get; set; }
        public string Abreviatura { get; set; }
        public string Diagnosticos { get; set; }
        public DateTime Fecha { get; set; }
    }

}
