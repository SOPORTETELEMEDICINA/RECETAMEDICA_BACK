namespace RMD.Models.Pacientes
{
    public class EntidadNacimiento
    {
        public int IdEntidad { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        /// <summary>
        /// Renderiza (mapea) un objeto EntidadNacimiento a partir de un SqlDataReader.
        /// </summary>
        /// <param name="reader">Instancia del SqlDataReader con los datos</param>
        /// <returns>Una instancia de EntidadNacimiento mapeada</returns>
        public static EntidadNacimiento FromDataReader(SqlDataReader reader)
        {
            return new EntidadNacimiento
            {
                IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                Abreviatura = reader.IsDBNull(reader.GetOrdinal("Abreviatura")) ? string.Empty : reader.GetString(reader.GetOrdinal("Abreviatura"))
            };
        }
    }
}
