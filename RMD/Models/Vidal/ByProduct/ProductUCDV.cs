namespace RMD.Models.Vidal.ByProduct
{
    public class ProductUCDV
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<UCDVEntry> UCDVEntries { get; set; } = new List<UCDVEntry>();
    }

}
