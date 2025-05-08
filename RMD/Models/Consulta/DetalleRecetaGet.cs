namespace RMD.Models.Consulta
{
    public class DetalleRecetaGet
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public string MedicamentoType { get; set; }
        public int MedicamentoId { get; set; }
        public string Medicamento { get; set; }
        public decimal CantidadDiaria { get; set; }
        public int UnidadDispensacionId { get; set; }
        public string UnidadDispensacion { get; set; }
        public int RutaAdministracionId { get; set; }
        public string RutaAdministracion { get; set; }
        public string Indicacion { get; set; }
        public string? IndicacionNombre { get; set; }
        public string Frecuencia { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public bool Surtido { get; set; }
        public static DetalleRecetaGet FromDataReader(IDataRecord r) => new DetalleRecetaGet
        {
            IdDetalleReceta = r.GetGuid(r.GetOrdinal("IdDetalleReceta")),
            IdReceta = r.GetGuid(r.GetOrdinal("IdReceta")),
            MedicamentoType = r.GetString(r.GetOrdinal("MedicamentoType")),
            MedicamentoId = r.GetInt32(r.GetOrdinal("MedicamentoId")),
            Medicamento = r.GetString(r.GetOrdinal("Medicamento")),
            CantidadDiaria = r.GetDecimal(r.GetOrdinal("CantidadDiaria")),
            UnidadDispensacionId = r.GetInt32(r.GetOrdinal("UnidadDispensacionId")),
            UnidadDispensacion = r.GetString(r.GetOrdinal("UnidadDispensacion")),
            RutaAdministracionId = r.GetInt32(r.GetOrdinal("RutaAdministracionId")),
            RutaAdministracion = r.GetString(r.GetOrdinal("RutaAdministracion")),
            Indicacion = r.GetString(r.GetOrdinal("Indicacion")),
            IndicacionNombre = r.IsDBNull(r.GetOrdinal("IndicacionNombre"))
                                       ? null
                                       : r.GetString(r.GetOrdinal("IndicacionNombre")),
            Frecuencia = r.GetString(r.GetOrdinal("Frecuencia")),
            Observaciones = r.IsDBNull(r.GetOrdinal("Observaciones"))
                                       ? null
                                       : r.GetString(r.GetOrdinal("Observaciones")),
            Duracion = r.GetInt32(r.GetOrdinal("Duracion")),
            UnidadDuracion = r.GetString(r.GetOrdinal("UnidadDuracion")),
            PeriodoInicio = r.GetDateTime(r.GetOrdinal("PeriodoInicio")),
            PeriodoTerminacion = r.IsDBNull(r.GetOrdinal("PeriodoTerminacion"))
                                       ? null
                                       : r.GetDateTime(r.GetOrdinal("PeriodoTerminacion")),
            Surtido = r.GetBoolean(r.GetOrdinal("Surtido"))
        };
    }

}
