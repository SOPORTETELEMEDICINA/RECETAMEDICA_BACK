using System.Formats.Tar;

namespace RMD.Models.Vidal.ByProduct
{
    public class ProductAtcClassification
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<AtcEntry> AtcEntries { get; set; } = new List<AtcEntry>();
    }
}
