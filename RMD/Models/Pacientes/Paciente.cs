using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Pacientes
{
    public class Paciente
    {
        [Key]
        public Guid IdPaciente { get; set; }

        [Required]
        public Guid IdUsuario { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }

        public int IdEntidadNacimiento { get; set; }

        [Required]
        [MaxLength(10)]
        public string Genero { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Alergias { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Molecules { get; set; } = string.Empty;
        [MaxLength(999999999)]
        public string? Patologias { get; set; } = string.Empty;

        [Required]
        public Guid IdMedico { get; set; }

        /// <summary>
        /// Renderiza (mapea) un objeto Paciente a partir de un SqlDataReader.
        /// </summary>
        /// <param name="reader">Instancia del SqlDataReader con los datos</param>
        /// <returns>Una instancia de Paciente con los datos mapeados</returns>
        public static Paciente FromDataReader(SqlDataReader reader)
        {
            var paciente = new Paciente
            {
                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                IdUsuario = reader.GetGuid(reader.GetOrdinal("IdUsuario")),
                IdTipoIdentificacion = reader.IsDBNull(reader.GetOrdinal("IdTipoIdentificacion"))
                                        ? 0 : reader.GetInt32(reader.GetOrdinal("IdTipoIdentificacion")),
                NumeroIdentificacion = reader.IsDBNull(reader.GetOrdinal("NumeroIdentificacion"))
                                        ? string.Empty : reader.GetString(reader.GetOrdinal("NumeroIdentificacion")),
                FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento"))
                                        ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                IdEntidadNacimiento = reader.IsDBNull(reader.GetOrdinal("IdEntidadNacimiento"))
                                        ? 0 : reader.GetInt32(reader.GetOrdinal("IdEntidadNacimiento")),
                Genero = reader.IsDBNull(reader.GetOrdinal("Genero"))
                                        ? string.Empty : reader.GetString(reader.GetOrdinal("Genero")),
                Alergias = reader.IsDBNull(reader.GetOrdinal("Alergias"))
                                        ? string.Empty : reader.GetString(reader.GetOrdinal("Alergias")),
                Molecules = reader.IsDBNull(reader.GetOrdinal("Molecules"))
                                        ? string.Empty : reader.GetString(reader.GetOrdinal("Molecules")),
                Patologias = reader.IsDBNull(reader.GetOrdinal("Patologias"))
                                        ? string.Empty : reader.GetString(reader.GetOrdinal("Patologias")),
                IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico"))
            };

            return paciente;
        }

    }
}
