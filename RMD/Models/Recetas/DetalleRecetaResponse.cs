using System;
using System.Data.SqlClient;

namespace RMD.Models.Recetas
{
    public class DetalleRecetaResponse
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public Guid IdPaciente { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public string Medicamento { get; set; }
        public string Descripcion { get; set; }

        /// <summary>
        /// Crea una instancia de DetalleRecetaResponse a partir de un SqlDataReader.
        /// </summary>
        /// <param name="reader">El SqlDataReader con los datos.</param>
        /// <returns>Instancia de DetalleRecetaResponse con los datos mapeados.</returns>
        public static DetalleRecetaResponse FromDataReader(SqlDataReader reader)
        {
            return new DetalleRecetaResponse
            {
                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                MedicamentoType = reader["MedicamentoType"]?.ToString() ?? string.Empty,
                Medicamento = reader["Medicamento"]?.ToString() ?? string.Empty,
                Descripcion = reader["Descripcion"]?.ToString() ?? string.Empty
            };
        }
    }
}
