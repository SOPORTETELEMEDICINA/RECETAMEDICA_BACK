namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVProduct
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public int AmmTypeId { get; set; }
        public string BestDocType { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
    }
}
