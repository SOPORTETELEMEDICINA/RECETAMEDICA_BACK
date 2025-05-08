namespace RMD.Models.Consulta
{
    public class RequestSearchMolecules
    {
        public int IdMolecule { get; set; }
        public string NameMolecule { get; set; } = string.Empty;
        public static RequestSearchMolecules FromDataReader(System.Data.IDataRecord reader)
        {
            return new RequestSearchMolecules
            {
                IdMolecule = reader.GetInt32(reader.GetOrdinal("IdMolecule")),
                NameMolecule = reader.GetString(reader.GetOrdinal("NameMolecule"))
            };
        }
    }
}
