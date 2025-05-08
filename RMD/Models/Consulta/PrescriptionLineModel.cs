using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RMD.Models.Consulta
{
    /// <summary>
    /// Representa una línea de prescripción de un paciente.
    /// </summary>
    public class PrescriptionLineModel
    {
        /// <summary>
        /// Identificador de la receta.
        /// </summary>
        public Guid IdReceta { get; set; }

        /// <summary>
        /// Identificador del paciente.
        /// </summary>
        public Guid IdPaciente { get; set; }

        /// <summary>
        /// Identificador del detalle de la receta.
        /// </summary>
        public Guid IdDetalleReceta { get; set; }

        /// <summary>
        /// Tipo de fármaco (por ejemplo, "PRODUCT", "VMP").
        /// </summary>
        public string DrugType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico del fármaco.
        /// </summary>
        public int Drug { get; set; }

        /// <summary>
        /// Dosis diaria.
        /// </summary>
        public int Dose { get; set; }

        /// <summary>
        /// Identificador de la unidad de dosis.
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Tipo de frecuencia de administración.
        /// </summary>
        public string FrequencyType { get; set; } = string.Empty;

        /// <summary>
        /// Duración del tratamiento.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Unidad de medida de la duración (por ejemplo, "DAYS", "WEEKS").
        /// </summary>
        public string DurationType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la ruta de administración.
        /// </summary>
        public int Route { get; set; }

        /// <summary>
        /// Indicación opcional asociada a la línea de prescripción.
        /// </summary>
        public string? Indication { get; set; }

        /// <summary>
        /// Crea una instancia de <see cref="PrescriptionLineModel"/> a partir de un <see cref="IDataRecord"/>.
        /// </summary>
        public static PrescriptionLineModel FromDataReader(IDataRecord reader)
        {
            return new PrescriptionLineModel
            {
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                DrugType = reader.GetString(reader.GetOrdinal("DrugType")),
                Drug = reader.GetInt32(reader.GetOrdinal("Drug")),
                Dose = reader.GetInt32(reader.GetOrdinal("Dose")),
                UnitId = reader.GetInt32(reader.GetOrdinal("UnitId")),
                FrequencyType = reader.GetString(reader.GetOrdinal("Frecuency")),
                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                DurationType = reader.GetString(reader.GetOrdinal("DurationType")),
                Route = reader.GetInt32(reader.GetOrdinal("Route")),
                Indication = reader.IsDBNull(reader.GetOrdinal("Indication"))
                                     ? null
                                     : reader.GetString(reader.GetOrdinal("Indication"))
            };
        }
    }
}
