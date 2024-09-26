namespace RMD.Models.Vidal.ByProduct
{
    public class AllergyEntry
    {
        public int AllergyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MoleculesId { get; set; }
        public int CrossAllergiesId { get; set; }
    }
}
