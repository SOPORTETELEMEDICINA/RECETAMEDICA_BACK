namespace RMD.Models.Consulta
{
    public class RecetaGetSQL
    {
        public Guid IdReceta { get; set; }
        public string Folio { get; set; }
        public Guid IdMedico { get; set; }
        public string NombresMedico { get; set; }
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        public string Universidad { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombresPaciente { get; set; }
        public string PrimerApellidoPaciente { get; set; }
        public string SegundoApellidoPaciente { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public int PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal PacCreatinina { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Patologias { get; set; }
        public Guid IdSucursal { get; set; }
        public string Sucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public string GrupoEmpresarial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }

        // Nuevos campos agregados para el estatus de la receta
        public int Estatus { get; set; }
        public string Descripcion { get; set; }
        public bool Timbrada { get; set; }
        public static RecetaGetSQL FromDataReader(IDataRecord r) => new RecetaGetSQL
        {
            IdReceta = r.GetGuid(r.GetOrdinal("IdReceta")),
            Folio = r.GetString(r.GetOrdinal("Folio")),
            IdMedico = r.GetGuid(r.GetOrdinal("IdMedico")),
            NombresMedico = r.GetString(r.GetOrdinal("NombresMedico")),
            PrimerApellidoMedico = r.GetString(r.GetOrdinal("PrimerApellidoMedico")),
            SegundoApellidoMedico = r.GetString(r.GetOrdinal("SegundoApellidoMedico")),
            Universidad = r.GetString(r.GetOrdinal("Universidad")),
            CedulaGeneral = r.GetString(r.GetOrdinal("CedulaGeneral")),
            Especialidad = r.GetString(r.GetOrdinal("Especialidad")),
            CedulaEspecialidad = r.GetString(r.GetOrdinal("CedulaEspecialidad")),
            IdPaciente = r.GetGuid(r.GetOrdinal("IdPaciente")),
            NombresPaciente = r.GetString(r.GetOrdinal("NombresPaciente")),
            PrimerApellidoPaciente = r.GetString(r.GetOrdinal("PrimerApellidoPaciente")),
            SegundoApellidoPaciente = r.GetString(r.GetOrdinal("SegundoApellidoPaciente")),
            IdTipoIdentificacion = r.GetInt32(r.GetOrdinal("IdTipoIdentificacion")),
            TipoIdentificacion = r.GetString(r.GetOrdinal("TipoIdentificacion")),
            NumeroIdentificacion = r.GetString(r.GetOrdinal("NumeroIdentificacion")),
            FechaNacimientoPaciente = r.GetDateTime(r.GetOrdinal("FechaNacimientoPaciente")),
            PacPeso = r.GetDecimal(r.GetOrdinal("PacPeso")),
            PacTalla = r.GetDecimal(r.GetOrdinal("PacTalla")),
            PacEmbarazo = r.GetBoolean(r.GetOrdinal("PacEmbarazo")),
            PacSemAmenorrea = r.GetInt32(r.GetOrdinal("PacSemAmenorrea")),
            PacLactancia = r.GetBoolean(r.GetOrdinal("PacLactancia")),
            PacCreatinina = r.GetDecimal(r.GetOrdinal("PacCreatinina")),
            Alergias = r.GetString(r.GetOrdinal("Alergias")),
            Molecules = r.GetString(r.GetOrdinal("Molecules")),
            Patologias = r.GetString(r.GetOrdinal("Patologias")),
            IdSucursal = r.GetGuid(r.GetOrdinal("IdSucursal")),
            Sucursal = r.GetString(r.GetOrdinal("Sucursal")),
            IdGEMP = r.GetGuid(r.GetOrdinal("IdGEMP")),
            GrupoEmpresarial = r.GetString(r.GetOrdinal("GrupoEmpresarial")),
            FechaCreacion = r.GetDateTime(r.GetOrdinal("FechaCreacion")),
            FechaUltimaModificacion = r.GetDateTime(r.GetOrdinal("FechaUltimaModificacion")),
            Estatus = r.GetInt32(r.GetOrdinal("Estatus")),
            Descripcion = r.GetString(r.GetOrdinal("Descripcion")),
            Timbrada = r.GetBoolean(r.GetOrdinal("Timbrada"))
        };
    }
}
