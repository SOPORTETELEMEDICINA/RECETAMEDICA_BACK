namespace RMD.Models.Vidal.ByUCD
{
    public class UCDByIdPackage
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public int VidalId { get; set; } 
        public string MarketStatus { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public List<string> RelatedLinks { get; set; }
    }
}
