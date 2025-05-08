namespace RMD.Models.Recetas
{
    public class MedicamentoActivoConsulta
    {
        public Guid IdReceta { get; set; }
        public Guid IdPaciente { get; set; }
        public Guid IdDetalleReceta { get; set; }
        public string DrugType { get; set; }
        public int Drug { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Medicamento { get; set; }
        public static MedicamentoActivoConsulta FromDataReader(SqlDataReader reader)
        {
            return new MedicamentoActivoConsulta
            {
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                DrugType = reader.GetString(reader.GetOrdinal("DrugType")),
                Drug = reader.GetInt32(reader.GetOrdinal("Drug")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                Medicamento = reader.IsDBNull(reader.GetOrdinal("Medicamento"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("Medicamento"))
            };
        }
    }

}
