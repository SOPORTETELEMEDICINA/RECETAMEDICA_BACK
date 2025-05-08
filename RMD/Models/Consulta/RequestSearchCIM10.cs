namespace RMD.Models.Consulta
{
    public class RequestSearchCIM10
    {
        public int IdCIM10 { get; set; }
        public string NameCIM10 { get; set; } = string.Empty;
        public string Code {  get; set; }
        public static RequestSearchCIM10 FromDataReader(System.Data.IDataRecord reader)
        {
            return new RequestSearchCIM10
            {
                IdCIM10 = reader.GetInt32(reader.GetOrdinal("IdCIM10")),
                NameCIM10 = reader.GetString(reader.GetOrdinal("NameCIM10")),
                Code = reader.GetString(reader.GetOrdinal("Code"))
            };
        }
    }
}

