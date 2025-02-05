namespace RMD.Models.Pacientes
{
    public class UsuarioPaciente
    {
        public Guid IdUsuario { get; set; } // T1.IdUsuario
        public Guid IdTipoUsuario { get; set; } // T1.IdTipoUsuario
        public Guid IdPaciente { get; set; } // T2.IdPaciente
        public Guid? IdGEMP { get; set; } // T1.IdGEMP
        public string GrupoEmpresarial { get; set; } = string.Empty; // T3.Nombre
        public Guid? IdSucursal { get; set; } // T1.IdSucursal
        public string Sucursal { get; set; } = string.Empty; // T13.Nombre
        public string Usr { get; set; } = string.Empty; // T1.Usr
        public string Nombres { get; set; } = string.Empty; // T1.Nombres
        public string PrimerApellido { get; set; } = string.Empty; // T1.PrimerApellido
        public string SegundoApellido { get; set; } = string.Empty; // T1.SegundoApellido
        public string FechaNacimiento { get; set; } = string.Empty; // T2.FechaNacimiento
        public int Edad { get; set; } // Calculada en el SP
        public int? IdEntidadNacimiento { get; set; } // T2.IdEntidadNacimiento
        public string EntidadNacimiento { get; set; } = string.Empty; // T12.Nombre
        public string Genero { get; set; } = string.Empty; // T2.Genero
        public string Alergias { get; set; } = string.Empty; // T2.Alergias
        public string Molecules { get; set; } = string.Empty; // T2.Molecules
        public string Patologias { get; set; } = string.Empty; // T2.Patologias
        public string Movil { get; set; } = string.Empty; // T1.Movil
        public string Email { get; set; } = string.Empty; // T1.Email
        public string Domicilio { get; set; } = string.Empty; // T1.Domicilio
        public int? IdAsentamiento { get; set; } // T1.IdAsentamiento
        public string Asentamiento { get; set; } = string.Empty; // T5.Nombre
        public int? IdTipoAsentamiento { get; set; } // T5.IdTipoAsentamiento
        public string TipoAsentamiento { get; set; } = string.Empty; // T6.TipoAsentamiento
        public int? IdCP { get; set; } // T5.IdCP
        public string CodigoPostal { get; set; } = string.Empty; // T7.CodigoPostal
        public int? IdMunicipio { get; set; } // T5.IdMunicipio
        public short NoMunicipio { get; set; } // T8.NoMunicipio
        public string Municipio { get; set; } = string.Empty; // T8.Nombre
        public int? IdCiudad { get; set; } // T5.IdCiudad
        public string Ciudad { get; set; } = string.Empty; // T9.Nombre
        public int? IdEntidad { get; set; } // T7.IdEntidad
        public string Estado { get; set; } = string.Empty; // T10.Nombre
        public string Abreviatura { get; set; } = string.Empty; // T10.Abreviatura
        public string Firma { get; set; } = string.Empty; // T11.Firma
        public string Imagen { get; set; } = string.Empty; // T11.Imagen
    }
}
