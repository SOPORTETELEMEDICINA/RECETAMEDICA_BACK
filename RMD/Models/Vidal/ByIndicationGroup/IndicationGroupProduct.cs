namespace RMD.Models.Vidal.ByIndicationGroup
{
    public class IndicationGroupProduct
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Summary { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public int AmmTypeId { get; set; }
        public string BestDocType { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Vmp { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
    }
}
