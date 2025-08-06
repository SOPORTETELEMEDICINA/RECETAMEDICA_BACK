namespace RMD.Models.Medicos
{
    public class MedicoConsultaRequest
    {
        public Guid IdGEMP { get; set; }
        public string GrupoEmpresarial { get; set; } = string.Empty;
        public Guid IdSucursal { get; set; }
        public string Sucursal { get; set; } = string.Empty;
        public string Usr { get; set; } = string.Empty;
        public Guid IdMedico { get; set; }
        public Guid IdUsuario { get; set; }
        public string CedulaGeneral { get; set; } = string.Empty;
        public string Universidad { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string CedulaEspecialidad { get; set; } = string.Empty;
        public string Horario { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public string Movil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;

        // Campos adicionales
        public int? IdAsentamiento { get; set; } // Puede ser nullable
        public string Asentamiento { get; set; } = string.Empty; // T5.Nombre
        public int? IdTipoAsentamiento { get; set; } // Puede ser nullable
        public string TipoAsentamiento { get; set; } = string.Empty; // T6.TipoAsentamiento
        public int? IdCP { get; set; } // Puede ser nullable
        public string CodigoPostal { get; set; } = string.Empty;
        public int? IdMunicipio { get; set; } // Puede ser nullable
        public short NoMunicipio { get; set; }  // T8.NoMunicipio
        public string Municipio { get; set; } = string.Empty;
        public int? IdCiudad { get; set; } // Puede ser nullable
        public string Ciudad { get; set; } = string.Empty;
        public int? IdEntidad { get; set; } // Puede ser nullable
        public string Estado { get; set; } = string.Empty; // T10.Nombre
        public string Abreviatura { get; set; } = string.Empty; // T10.Abreviatura
        public string Firma { get; set; } = string.Empty; // T11.Firma
        public string Imagen { get; set; } = string.Empty; // T11.Imagen
        public string Status { get; set; }
    }
}
