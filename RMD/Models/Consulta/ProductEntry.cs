using RMD.Models.Shared;

namespace RMD.Models.Consulta
{
    public class ProductEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<RelatedLink> RelatedLinks { get; set; } = new List<RelatedLink>();
    }
}
