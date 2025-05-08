namespace RMD.Models.Recetas
{
    public class Receta_FormatoRequest
    {
        public int IdFormatoReceta { get; set; }
        public string Formato { get; set; }
        public string Logo { get; set; }
        public string LogoSuperior { get; set; }
        public string LogoInferior { get; set; }
        public static Receta_FormatoRequest FromDataReader(SqlDataReader reader)
        {
            return new Receta_FormatoRequest
            {
                IdFormatoReceta = reader.GetInt32(reader.GetOrdinal("IdFormatoReceta")),
                Formato = reader["Formato"]?.ToString(),
                Logo = reader["Logo"]?.ToString(),
                LogoSuperior = reader["LogoSuperior"]?.ToString(),
                LogoInferior = reader["LogoInferior"]?.ToString()
            };
        }

    }

}
