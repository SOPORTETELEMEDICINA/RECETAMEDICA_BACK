namespace RMD.Models.PuntoVenta
{
    //public class DetalleRecetaModel
    //{
    //    public Guid IdDetalleReceta { get; set; }
    //    public Guid IdReceta { get; set; }
    //    public int MedicamentoId { get; set; }
    //    public string MedicamentoType { get; set; }
    //    public decimal CantidadDiaria { get; set; }
    //    public int UnidadDispensacionId { get; set; }
    //    public int RutaAdministracionId { get; set; }
    //    public string Indicacion { get; set; }
    //    public int Duracion { get; set; }
    //    public string UnidadDuracion { get; set; }
    //    public DateTime PeriodoInicio { get; set; }
    //    public DateTime? PeriodoTerminacion { get; set; }
    //    public bool Surtido { get; set; }
    //    public DateTime? FechaSurtido { get; set; }
    //}
    public class DetalleRecetaModel
    {
        public Guid IdDetalleReceta { get; set; } // Identificador único del detalle de la receta
        public Guid IdReceta { get; set; }        // Identificador de la receta asociada
        public string MedicamentoType { get; set; } // Tipo de medicamento (PRODUCT, PACKAGE, VMP, UCD, UCDV)
        public int MedicamentoId { get; set; }    // Identificador del medicamento
        public string MedicamentoNombre { get; set; } // Nombre o descripción del medicamento según el tipo

        // Unidad de dispensación
        public int UnidadDispensacionId { get; set; } // ID de la unidad de dispensación
        public string UnidadDispensacion { get; set; } // Nombre de la unidad de dispensación

        // Ruta de administración
        public int RutaAdministracionId { get; set; } // ID de la ruta de administración
        public string RutaAdministracion { get; set; } // Nombre de la ruta de administración

        public decimal CantidadDiaria { get; set; } // Cantidad diaria de medicamento
        public string Indicacion { get; set; } // Instrucciones o indicaciones para el medicamento
        public string? IndicacionNombre { get; set; }
        public string Frecuencia { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; } // Duración del tratamiento en días
        public string UnidadDuracion { get; set; } // Unidad de duración (e.g., días, semanas, meses)
        public DateTime PeriodoInicio { get; set; } // Fecha de inicio del tratamiento
        public DateTime? PeriodoTerminacion { get; set; } // Fecha de terminación del tratamiento
        public bool Surtido { get; set; } // Estado de si fue surtido o no
        public static DetalleRecetaModel FromDataReader(SqlDataReader reader)
        {
            return new DetalleRecetaModel
            {
                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                MedicamentoType = reader["MedicamentoType"]?.ToString() ?? string.Empty,
                MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                MedicamentoNombre = reader["MedicamentoNombre"]?.ToString() ?? string.Empty,
                UnidadDispensacionId = reader.GetInt32(reader.GetOrdinal("UnidadDispensacionId")),
                UnidadDispensacion = reader["UnidadDispensacion"]?.ToString() ?? string.Empty,
                RutaAdministracionId = reader.GetInt32(reader.GetOrdinal("RutaAdministracionId")),
                RutaAdministracion = reader["RutaAdministracion"]?.ToString() ?? string.Empty,
                CantidadDiaria = reader.GetDecimal(reader.GetOrdinal("CantidadDiaria")),
                Indicacion = reader["Indicacion"]?.ToString() ?? string.Empty,
                IndicacionNombre = reader["IndicacionNombre"]?.ToString(),
                Frecuencia = reader["Frecuencia"]?.ToString() ?? string.Empty,
                Observaciones = reader["Observaciones"]?.ToString(),
                Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                UnidadDuracion = reader["UnidadDuracion"]?.ToString() ?? string.Empty,
                PeriodoInicio = reader.GetDateTime(reader.GetOrdinal("PeriodoInicio")),
                PeriodoTerminacion = reader.IsDBNull(reader.GetOrdinal("PeriodoTerminacion"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("PeriodoTerminacion")),
                Surtido = reader.GetBoolean(reader.GetOrdinal("Surtido"))
            };
        }
    }


}
