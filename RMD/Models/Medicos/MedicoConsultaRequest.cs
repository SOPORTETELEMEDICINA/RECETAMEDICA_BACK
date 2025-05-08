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
        public static MedicoConsultaRequest FromDataReader(IDataRecord r) => new MedicoConsultaRequest
        {
            IdGEMP = r.GetGuid(r.GetOrdinal("IdGEMP")),
            GrupoEmpresarial = r.GetString(r.GetOrdinal("GrupoEmpresarial")),
            IdSucursal = r.GetGuid(r.GetOrdinal("IdSucursal")),
            Sucursal = r.GetString(r.GetOrdinal("Sucursal")),
            Usr = r.GetString(r.GetOrdinal("Usr")),
            IdMedico = r.GetGuid(r.GetOrdinal("IdMedico")),
            IdUsuario = r.GetGuid(r.GetOrdinal("IdUsuario")),
            CedulaGeneral = r.GetString(r.GetOrdinal("CedulaGeneral")),
            Universidad = r.GetString(r.GetOrdinal("Universidad")),
            Especialidad = r.GetString(r.GetOrdinal("Especialidad")),
            CedulaEspecialidad = r.GetString(r.GetOrdinal("CedulaEspecialidad")),
            Horario = r.GetString(r.GetOrdinal("Horario")),
            Nombres = r.GetString(r.GetOrdinal("Nombres")),
            PrimerApellido = r.GetString(r.GetOrdinal("PrimerApellido")),
            SegundoApellido = r.GetString(r.GetOrdinal("SegundoApellido")),
            Movil = r.GetString(r.GetOrdinal("Movil")),
            Email = r.GetString(r.GetOrdinal("Email")),
            Domicilio = r.GetString(r.GetOrdinal("Domicilio")),
            IdAsentamiento = r.IsDBNull(r.GetOrdinal("IdAsentamiento"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdAsentamiento")),
            Asentamiento = r.GetString(r.GetOrdinal("Asentamiento")),
            IdTipoAsentamiento = r.IsDBNull(r.GetOrdinal("IdTipoAsentamiento"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdTipoAsentamiento")),
            TipoAsentamiento = r.GetString(r.GetOrdinal("TipoAsentamiento")),
            IdCP = r.IsDBNull(r.GetOrdinal("IdCP"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdCP")),
            CodigoPostal = r.GetString(r.GetOrdinal("CodigoPostal")),
            IdMunicipio = r.IsDBNull(r.GetOrdinal("IdMunicipio"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdMunicipio")),
            NoMunicipio = r.GetInt16(r.GetOrdinal("NoMunicipio")),
            Municipio = r.GetString(r.GetOrdinal("Municipio")),
            IdCiudad = r.IsDBNull(r.GetOrdinal("IdCiudad"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdCiudad")),
            Ciudad = r.GetString(r.GetOrdinal("Ciudad")),
            IdEntidad = r.IsDBNull(r.GetOrdinal("IdEntidad"))
                                    ? (int?)null
                                    : r.GetInt32(r.GetOrdinal("IdEntidad")),
            Estado = r.GetString(r.GetOrdinal("Estado")),
            Abreviatura = r.GetString(r.GetOrdinal("Abreviatura")),
            Firma = r.GetString(r.GetOrdinal("Firma")),
            Imagen = r.GetString(r.GetOrdinal("Imagen")),
            Status = r.GetString(r.GetOrdinal("Status"))
        };
    }
}
