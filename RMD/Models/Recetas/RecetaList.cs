namespace RMD.Models.Recetas
{
    public class RecetaList
    {
        public string Folio { get; set; }
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public string NombreMedico { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public string PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal PacCreatinina { get; set; }
        public string Patologias { get; set; }
        public Guid IdSucursal { get; set; }
        public string Sucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public string GrupoEmpresarial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public int Estatus { get; set; }
        public string EstatusReceta { get; set; }
        public int GenerarQR { get; set; }
        public int GenerarPDF { get; set; }
        public int GenerarEventoMedicamentoso { get; set; }
        public static RecetaList FromDataReader(SqlDataReader reader)
        {
            return new RecetaList
            {
                Folio = reader.GetString(reader.GetOrdinal("Folio")),
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")),
                CedulaGeneral = reader.GetString(reader.GetOrdinal("CedulaGeneral")),
                Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                CedulaEspecialidad = reader.GetString(reader.GetOrdinal("CedulaEspecialidad")),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                NombrePaciente = reader.GetString(reader.GetOrdinal("NombrePaciente")),
                FechaNacimientoPaciente = reader.GetDateTime(reader.GetOrdinal("FechaNacimientoPaciente")),
                PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
                PacEmbarazo = reader.GetBoolean(reader.GetOrdinal("PacEmbarazo")),
                PacSemAmenorrea = reader.GetString(reader.GetOrdinal("PacSemAmenorrea")),
                PacLactancia = reader.GetBoolean(reader.GetOrdinal("PacLactancia")),
                PacCreatinina = reader.GetDecimal(reader.GetOrdinal("PacCreatinina")),
                Patologias = reader.GetString(reader.GetOrdinal("Patologias")),
                IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                Sucursal = reader.GetString(reader.GetOrdinal("Sucursal")),
                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                GrupoEmpresarial = reader.GetString(reader.GetOrdinal("GrupoEmpresarial")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion")),
                Estatus = reader.GetInt32(reader.GetOrdinal("Estatus")),
                EstatusReceta = reader.GetString(reader.GetOrdinal("EstatusReceta")),
                GenerarQR = ColumnExists(reader, "GenerarQR") ? reader.GetInt32(reader.GetOrdinal("GenerarQR")) : 1,
                GenerarPDF = ColumnExists(reader, "GenerarPDF") ? reader.GetInt32(reader.GetOrdinal("GenerarPDF")) : 1,
                GenerarEventoMedicamentoso = ColumnExists(reader, "GenerarEventoMedicamentoso") ? reader.GetInt32(reader.GetOrdinal("GenerarEventoMedicamentoso")) : 1
            };
        }
        private static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            var schemaTable = reader.GetSchemaTable();
            foreach (DataRow row in schemaTable.Rows)
            {
                if (row["ColumnName"].ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
