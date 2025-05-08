namespace RMD.Models.Consulta
{
    public class RequestSearchAllergy
    {
        public int IdAllergy { get; set; }
        public string NameAllergy { get; set; } = string.Empty;
        public static RequestSearchAllergy FromDataReader(System.Data.IDataRecord reader)
        {
            return new RequestSearchAllergy
            {
                IdAllergy = reader.GetInt32(reader.GetOrdinal("IdAllergy")),
                NameAllergy = reader.GetString(reader.GetOrdinal("NameAllergy"))
            };
        }
    }
}
