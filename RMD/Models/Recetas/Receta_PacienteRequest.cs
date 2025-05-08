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
        public int IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
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
        public string QRData { get; set; }
        public string Folio { get; set; }
        public int Estatus { get; set; }
        public string EstatusReceta { get; set; }

        public static Receta_PacienteRequest FromDataReader(SqlDataReader reader)
        {
            return new Receta_PacienteRequest
            {
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                NombresMedico = reader["NombresMedicos"]?.ToString(),
                PrimerApellidoMedico = reader["PrimerApellidoMedico"]?.ToString(),
                SegundoApellidoMedico = reader["SegundoApellidoMedico"]?.ToString(),
                Movil = reader["Movil"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Universidad = reader["Universidad"]?.ToString(),
                CedulaGeneral = reader["CedulaGeneral"]?.ToString(),
                Especialidad = reader["Especialidad"]?.ToString(),
                CedulaEspecialidad = reader["CedulaEspecialidad"]?.ToString(),
                Horario = reader["Horario"]?.ToString(),
                Firma = reader["Firma"]?.ToString(),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                NombresPaciente = reader["NombresPaciente"]?.ToString(),
                PrimerApellidoPaciente = reader["PrimerApellidoPaciente"]?.ToString(),
                SegundoApellidoPaciente = reader["SegundoApellidoPaciente"]?.ToString(),
                IdTipoIdentificacion = reader.GetInt32(reader.GetOrdinal("IdTipoIdentificacion")),
                TipoIdentificacion = reader["TipoIdentificacion"]?.ToString(),
                NumeroIdentificacion = reader["NumeroIdentificacion"]?.ToString(),
                EdadPaciente = reader.GetInt32(reader.GetOrdinal("EdadPaciente")),
                PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                Genero = reader["Genero"]?.ToString(),
                PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
                Diagnosticos = reader["Diagnosticos"]?.ToString(),
                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                NombreGrupoEmpresarial = reader["NombreGrupoEmpresarial"]?.ToString(),
                LogoBase64 = reader["LogoBase64"]?.ToString(),
                IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                NumeroSucursal = reader["NumeroSucursal"]?.ToString(),
                NombreSucursal = reader["NombreSucursal"]?.ToString(),
                RegistroSanitario = reader["RegistroSanitario"]?.ToString(),
                Domicilio = reader["Domicilio"]?.ToString(),
                IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                NombreAsentamiento = reader["NombreAsentamiento"]?.ToString(),
                IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                TipoAsentamiento = reader["TipoAsentamiento"]?.ToString(),
                IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                CodigoPostal = reader["CodigoPostal"]?.ToString(),
                IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                NombreMunicipio = reader["NombreMunicipio"]?.ToString(),
                IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                NombreCiudad = reader["NombreCiudad"]?.ToString(),
                IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                Estado = reader["Estado"]?.ToString(),
                Abreviatura = reader["Abreviatura"]?.ToString(),
                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                QRData = reader["QRData"]?.ToString(),
                Folio = reader["Folio"]?.ToString(),
                Estatus = reader.GetInt32(reader.GetOrdinal("Estatus")),
                EstatusReceta = reader["EstatusReceta"]?.ToString()
            };
        }

    }

}
