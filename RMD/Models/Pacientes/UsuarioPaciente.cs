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
        public int IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
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

        /// <summary>
        /// Renderiza (mapea) un objeto UsuarioPaciente a partir de un SqlDataReader.
        /// </summary>
        /// <param name="reader">Instancia del SqlDataReader con los datos</param>
        /// <returns>Una instancia de UsuarioPaciente con los datos mapeados</returns>
        public static UsuarioPaciente FromDataReader(SqlDataReader reader)
        {
            var usuario = new UsuarioPaciente
            {
                IdUsuario = reader.GetGuid(reader.GetOrdinal("IdUsuario")),
                IdTipoUsuario = reader.GetGuid(reader.GetOrdinal("IdTipoUsuario")),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                IdGEMP = reader.IsDBNull(reader.GetOrdinal("IdGEMP")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                GrupoEmpresarial = reader.IsDBNull(reader.GetOrdinal("GrupoEmpresarial")) ? string.Empty : reader.GetString(reader.GetOrdinal("GrupoEmpresarial")),
                IdSucursal = reader.IsDBNull(reader.GetOrdinal("IdSucursal")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                Sucursal = reader.IsDBNull(reader.GetOrdinal("Sucursal")) ? string.Empty : reader.GetString(reader.GetOrdinal("Sucursal")),
                Usr = reader.IsDBNull(reader.GetOrdinal("Usr")) ? string.Empty : reader.GetString(reader.GetOrdinal("Usr")),
                Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombres")),
                PrimerApellido = reader.IsDBNull(reader.GetOrdinal("PrimerApellido")) ? string.Empty : reader.GetString(reader.GetOrdinal("PrimerApellido")),
                SegundoApellido = reader.IsDBNull(reader.GetOrdinal("SegundoApellido")) ? string.Empty : reader.GetString(reader.GetOrdinal("SegundoApellido")),
                IdTipoIdentificacion = reader.IsDBNull(reader.GetOrdinal("IdTipoIdentificacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdTipoIdentificacion")),
                TipoIdentificacion = reader.IsDBNull(reader.GetOrdinal("TipoIdentificacion")) ? string.Empty : reader.GetString(reader.GetOrdinal("TipoIdentificacion")),
                NumeroIdentificacion = reader.IsDBNull(reader.GetOrdinal("NumeroIdentificacion")) ? string.Empty : reader.GetString(reader.GetOrdinal("NumeroIdentificacion")),
                FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? string.Empty : reader.GetString(reader.GetOrdinal("FechaNacimiento")),
                Edad = reader.IsDBNull(reader.GetOrdinal("Edad")) ? 0 : reader.GetInt32(reader.GetOrdinal("Edad")),
                IdEntidadNacimiento = reader.IsDBNull(reader.GetOrdinal("IdEntidadNacimiento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEntidadNacimiento")),
                EntidadNacimiento = reader.IsDBNull(reader.GetOrdinal("EntidadNacimiento")) ? string.Empty : reader.GetString(reader.GetOrdinal("EntidadNacimiento")),
                Genero = reader.IsDBNull(reader.GetOrdinal("Genero")) ? string.Empty : reader.GetString(reader.GetOrdinal("Genero")),
                Alergias = reader.IsDBNull(reader.GetOrdinal("Alergias")) ? string.Empty : reader.GetString(reader.GetOrdinal("Alergias")),
                Molecules = reader.IsDBNull(reader.GetOrdinal("Molecules")) ? string.Empty : reader.GetString(reader.GetOrdinal("Molecules")),
                Patologias = reader.IsDBNull(reader.GetOrdinal("Patologias")) ? string.Empty : reader.GetString(reader.GetOrdinal("Patologias")),
                Movil = reader.IsDBNull(reader.GetOrdinal("Movil")) ? string.Empty : reader.GetString(reader.GetOrdinal("Movil")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                Domicilio = reader.IsDBNull(reader.GetOrdinal("Domicilio")) ? string.Empty : reader.GetString(reader.GetOrdinal("Domicilio")),
                IdAsentamiento = reader.IsDBNull(reader.GetOrdinal("IdAsentamiento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                Asentamiento = reader.IsDBNull(reader.GetOrdinal("Asentamiento")) ? string.Empty : reader.GetString(reader.GetOrdinal("Asentamiento")),
                IdTipoAsentamiento = reader.IsDBNull(reader.GetOrdinal("IdTipoAsentamiento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                TipoAsentamiento = reader.IsDBNull(reader.GetOrdinal("TipoAsentamiento")) ? string.Empty : reader.GetString(reader.GetOrdinal("TipoAsentamiento")),
                IdCP = reader.IsDBNull(reader.GetOrdinal("IdCP")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdCP")),
                CodigoPostal = reader.IsDBNull(reader.GetOrdinal("CodigoPostal")) ? string.Empty : reader.GetString(reader.GetOrdinal("CodigoPostal")),
                IdMunicipio = reader.IsDBNull(reader.GetOrdinal("IdMunicipio")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                NoMunicipio = reader.IsDBNull(reader.GetOrdinal("NoMunicipio")) ? (short)0 : reader.GetInt16(reader.GetOrdinal("NoMunicipio")),
                Municipio = reader.IsDBNull(reader.GetOrdinal("Municipio")) ? string.Empty : reader.GetString(reader.GetOrdinal("Municipio")),
                IdCiudad = reader.IsDBNull(reader.GetOrdinal("IdCiudad")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                Ciudad = reader.IsDBNull(reader.GetOrdinal("Ciudad")) ? string.Empty : reader.GetString(reader.GetOrdinal("Ciudad")),
                IdEntidad = reader.IsDBNull(reader.GetOrdinal("IdEntidad")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? string.Empty : reader.GetString(reader.GetOrdinal("Estado")),
                Abreviatura = reader.IsDBNull(reader.GetOrdinal("Abreviatura")) ? string.Empty : reader.GetString(reader.GetOrdinal("Abreviatura")),
                Firma = reader.IsDBNull(reader.GetOrdinal("Firma")) ? string.Empty : reader.GetString(reader.GetOrdinal("Firma")),
                Imagen = reader.IsDBNull(reader.GetOrdinal("Imagen")) ? string.Empty : reader.GetString(reader.GetOrdinal("Imagen"))
            };

            return usuario;
        }
    }
}
