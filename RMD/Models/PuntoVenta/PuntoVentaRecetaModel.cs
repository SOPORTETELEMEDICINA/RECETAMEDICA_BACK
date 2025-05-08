namespace RMD.Models.PuntoVenta
{
    public class PuntoVentaRecetaModel
    {
        // Información de la receta
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }

        // Información del médico
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

        // Información del paciente
        public Guid IdPaciente { get; set; }
        public string NombresPaciente { get; set; }
        public string PrimerApellidoPaciente { get; set; }
        public string SegundoApellidoPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public string Genero { get; set; }
        public decimal PacTalla { get; set; }

        // Información del grupo empresarial
        public Guid IdGEMP { get; set; }
        public string NombreGrupoEmpresarial { get; set; }
        public string LogoBase64 { get; set; }

        // Información de la sucursal
        public Guid IdSucursal { get; set; }
        public string NumeroSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string RegistroSanitario { get; set; }
        public string DomicilioSucursal { get; set; }
        public int IdAsentamiento { get; set; }

        // Información del asentamiento
        public string NombreAsentamiento { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public string TipoAsentamiento { get; set; }
        public int IdCP { get; set; }

        // Información del código postal
        public string CodigoPostal { get; set; }

        // Información de la ubicación
        public int IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; }
        public int IdCiudad { get; set; }
        public string NombreCiudad { get; set; }
        public int IdEntidad { get; set; }
        public string Estado { get; set; }
        public string Abreviatura { get; set; }

        public static PuntoVentaRecetaModel FromDataReader(SqlDataReader reader)
        {
            return new PuntoVentaRecetaModel
            {
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                NombresMedico = reader["NombresMedicos"]?.ToString() ?? string.Empty,
                PrimerApellidoMedico = reader["PrimerApellidoMedico"]?.ToString() ?? string.Empty,
                SegundoApellidoMedico = reader["SegundoApellidoMedico"]?.ToString() ?? string.Empty,
                Movil = reader["Movil"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Universidad = reader["Universidad"]?.ToString() ?? string.Empty,
                CedulaGeneral = reader["CedulaGeneral"]?.ToString() ?? string.Empty,
                Especialidad = reader["Especialidad"]?.ToString() ?? string.Empty,
                CedulaEspecialidad = reader["CedulaEspecialidad"]?.ToString() ?? string.Empty,
                Horario = reader["Horario"]?.ToString() ?? string.Empty,
                Firma = reader["Firma"]?.ToString() ?? string.Empty,
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                NombresPaciente = reader["NombresPacientes"]?.ToString() ?? string.Empty,
                PrimerApellidoPaciente = reader["PrimerApellidoPacientes"]?.ToString() ?? string.Empty,
                SegundoApellidoPaciente = reader["SegundoApellidoPacientes"]?.ToString() ?? string.Empty,
                PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                Genero = reader["Genero"]?.ToString() ?? string.Empty,
                PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                NombreGrupoEmpresarial = reader["NombreGrupoEmpresarial"]?.ToString() ?? string.Empty,
                LogoBase64 = reader["LogoBase64"]?.ToString() ?? string.Empty,
                IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                NumeroSucursal = reader["NumeroSucursal"]?.ToString() ?? string.Empty,
                NombreSucursal = reader["NombreSucursal"]?.ToString() ?? string.Empty,
                RegistroSanitario = reader["RegistroSanitario"]?.ToString() ?? string.Empty,
                DomicilioSucursal = reader["DomicilioSucursal"]?.ToString() ?? string.Empty,
                IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                NombreAsentamiento = reader["NombreAsentamiento"]?.ToString() ?? string.Empty,
                IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                TipoAsentamiento = reader["TipoAsentamiento"]?.ToString() ?? string.Empty,
                IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                CodigoPostal = reader["CodigoPostal"]?.ToString() ?? string.Empty,
                IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                NombreMunicipio = reader["NombreMunicipio"]?.ToString() ?? string.Empty,
                IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                NombreCiudad = reader["NombreCiudad"]?.ToString() ?? string.Empty,
                IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                Estado = reader["Estado"]?.ToString() ?? string.Empty,
                Abreviatura = reader["Abreviatura"]?.ToString() ?? string.Empty
            };
        }
    }
}
