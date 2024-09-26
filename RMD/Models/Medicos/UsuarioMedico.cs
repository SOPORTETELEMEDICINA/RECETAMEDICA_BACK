namespace RMD.Models.Medicos
{
    public class UsuarioMedico
    {
        public Guid IdMedico { get; set; }
        public Guid? IdGEMP { get; set; } // Assuming this can be null
        public Guid? IdSucursal { get; set; } // Assuming this can be null
        public required string Nombres { get; set; }
        public required string PrimerApellido { get; set; } 
        public string SegundoApellido { get; set; } = string.Empty;
        public required string CedulaGeneral { get; set; } 
        public required string Universidad { get; set; } 
        public string Especialidad { get; set; } = string.Empty;
        public string CedulaEspecialidad { get; set; } = string.Empty;
        public required string Horario { get; set; }
        public required string Movil { get; set; } 
        public required string Email { get; set; } 
        public string Domicilio { get; set; } = string.Empty;
        public int IdAsentamiento { get; set; }
        public string Asentamiento { get; set; } = string.Empty;
        public string TipoAsentamiento { get; set; } = string.Empty;
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; } = string.Empty;
        public int IdMunicipio { get; set; }
        public string Municipio { get; set; } = string.Empty;
        public int IdCiudad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public int IdEntidad { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
