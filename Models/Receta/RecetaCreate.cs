//namespace RMD.Models.Recetas
//{
//    public class RecetaCreate
//    {
//        public Guid IdReceta { get; set; }  // ID único de la receta
//        public Guid IdMedico { get; set; }  // ID del médico
//        public Guid IdPaciente { get; set; }  // ID del paciente
//        public decimal PacPeso { get; set; }  // Peso del paciente
//        public decimal PacTalla { get; set; }  // Talla del paciente
//        public bool PacEmbarazo { get; set; }  // Embarazo
//        public int? PacSemAmenorrea { get; set; }  // Semanas de amenorrea (opcional)
//        public bool PacLactancia { get; set; }  // Lactancia
//        public decimal? PacCreatinina { get; set; }  // Creatinina (opcional)
//        public string Alergias { get; set; } = string.Empty;  // Alergias (como string delimitado por comas)
//        public string Molecules { get; set; } = string.Empty;  // Moléculas (como string delimitado por comas)
//        public string Patologias { get; set; } = string.Empty;  // Patologías (como string delimitado por comas)
//        public Guid IdSucursal { get; set; }  // ID de la sucursal
//        public Guid IdGEMP { get; set; }  // ID del GEMP
//        public DateTime FechaCreacion { get; set; }  // Fecha de creación
//        public DateTime FechaUltimaModificacion { get; set; }  // Fecha de última modificación
//        public bool Timbrada { get; set; } = false; // Indica si la receta está timbrada

//        public static RecetaCreate FromDataReader(SqlDataReader reader)
//        {
//            return new RecetaCreate
//            {
//                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
//                IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
//                IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
//                PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
//                PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
//                PacEmbarazo = reader.GetBoolean(reader.GetOrdinal("PacEmbarazo")),
//                PacSemAmenorrea = reader.IsDBNull(reader.GetOrdinal("PacSemAmenorrea"))
//                                            ? null
//                                            : reader.GetInt32(reader.GetOrdinal("PacSemAmenorrea")),
//                PacLactancia = reader.GetBoolean(reader.GetOrdinal("PacLactancia")),
//                PacCreatinina = reader.IsDBNull(reader.GetOrdinal("PacCreatinina"))
//                                            ? null
//                                            : reader.GetDecimal(reader.GetOrdinal("PacCreatinina")),
//                Alergias = reader["Alergias"]?.ToString() ?? string.Empty,
//                Molecules = reader["Molecules"]?.ToString() ?? string.Empty,
//                Patologias = reader["Patologias"]?.ToString() ?? string.Empty,
//                IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
//                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
//                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
//                FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion"))
//            };
//        }

//    }
//}
