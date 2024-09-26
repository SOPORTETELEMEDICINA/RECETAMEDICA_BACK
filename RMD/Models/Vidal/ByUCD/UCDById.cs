namespace RMD.Models.Vidal.ByUCD
{
    public class UCDById
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
    }
}
