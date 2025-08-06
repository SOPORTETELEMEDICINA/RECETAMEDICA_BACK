//namespace RMD.Models.Consulta
//{
//    public class DetalleReceta
//    {
//        public Guid IdDetalleReceta { get; set; }
//        public Guid IdReceta { get; set; }
//        public int MedicamentoId { get; set; }
//        public string MedicamentoType { get; set; } = string.Empty;
//        public int UnidadDispensacionId { get; set; }
//        public int RutaAdministracionId { get; set; }
//        public decimal CantidadDiaria { get; set; }
//        public string? Indicacion { get; set; }
//        public string? IndicacionNombre { get; set; }
//        public string Frecuencia { get; set; } = string.Empty;
//        public string? Observaciones { get; set; }
//        public int Duracion { get; set; }
//        public string UnidadDuracion { get; set; } = string.Empty;
//        public DateTime PeriodoInicio { get; set; }
//        public DateTime? PeriodoTerminacion { get; set; }

//        public static DetalleReceta FromDataReader(SqlDataReader reader)
//        {
//            return new DetalleReceta
//            {
//                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
//                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
//                MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
//                MedicamentoType = reader["MedicamentoType"]?.ToString() ?? string.Empty,
//                UnidadDispensacionId = reader.GetInt32(reader.GetOrdinal("UnidadDispensacionId")),
//                RutaAdministracionId = reader.GetInt32(reader.GetOrdinal("RutaAdministracionId")),
//                CantidadDiaria = reader.GetDecimal(reader.GetOrdinal("CantidadDiaria")),
//                Indicacion = reader.IsDBNull(reader.GetOrdinal("Indicacion"))
//                                       ? null
//                                       : reader.GetString(reader.GetOrdinal("Indicacion")),
//                IndicacionNombre = reader.IsDBNull(reader.GetOrdinal("IndicacionNombre"))
//                                       ? null
//                                       : reader.GetString(reader.GetOrdinal("IndicacionNombre")),
//                Frecuencia = reader["Frecuencia"]?.ToString() ?? string.Empty,
//                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones"))
//                                       ? null
//                                       : reader.GetString(reader.GetOrdinal("Observaciones")),
//                Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
//                UnidadDuracion = reader["UnidadDuracion"]?.ToString() ?? string.Empty,
//                PeriodoInicio = reader.GetDateTime(reader.GetOrdinal("PeriodoInicio")),
//                PeriodoTerminacion = reader.IsDBNull(reader.GetOrdinal("PeriodoTerminacion"))
//                                       ? (DateTime?)null
//                                       : reader.GetDateTime(reader.GetOrdinal("PeriodoTerminacion"))
//            };
//        }
//    }
//}
