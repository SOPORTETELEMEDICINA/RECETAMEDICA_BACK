namespace RMD.Models.Vidal.ByProduct
{
    public class ProductAllergy
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<AllergyEntry> AllergyEntries { get; set; } = new List<AllergyEntry>();
    }
}
