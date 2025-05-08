using System;
using System.Data.SqlClient;

namespace RMD.Models.Recetas
{
    public class Detalle_RecetaDetalleRequest
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public string MedicamentoType { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoNombre { get; set; }
        public int UnidadDispensacionId { get; set; }
        public string UnidadDispensacion { get; set; }
        public int RutaAdministracionId { get; set; }
        public string RutaAdministracion { get; set; }
        public decimal CantidadDiaria { get; set; }
        public string Indicacion { get; set; }
        public string IndicacionNombre { get; set; }
        public string Frecuencia { get; set; }
        public string Observaciones { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public bool Surtido { get; set; }
        public string Descripcion { get; set; }
        public int GenerarEventoMedicamentoso { get; set; }

        /// <summary>
        /// Crea una instancia de Detalle_RecetaDetalleRequest a partir de un SqlDataReader.
        /// </summary>
        /// <param name="reader">El SqlDataReader que contiene los datos.</param>
        /// <returns>Instancia de Detalle_RecetaDetalleRequest con los datos mapeados.</returns>
        public static Detalle_RecetaDetalleRequest FromDataReader(SqlDataReader reader)
        {
            return new Detalle_RecetaDetalleRequest
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
                IndicacionNombre = reader["IndicacionNombre"]?.ToString() ?? string.Empty,
                Frecuencia = reader["Frecuencia"]?.ToString() ?? string.Empty,
                Observaciones = reader["Observaciones"]?.ToString() ?? string.Empty,
                Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                UnidadDuracion = reader["UnidadDuracion"]?.ToString() ?? string.Empty,
                PeriodoInicio = reader.GetDateTime(reader.GetOrdinal("PeriodoInicio")),
                PeriodoTerminacion = reader.IsDBNull(reader.GetOrdinal("PeriodoTerminacion"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("PeriodoTerminacion")),
                Surtido = reader.GetBoolean(reader.GetOrdinal("Surtido")),
                Descripcion = reader["Descripcion"]?.ToString() ?? string.Empty,
                GenerarEventoMedicamentoso = reader.GetInt32(reader.GetOrdinal("GenerarEventoMedicamentoso"))
            };
        }
    }
}
