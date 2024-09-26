namespace RMD.Models.Vidal.ByProduct
{
    public class ProductById
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public string AmmType { get; set; } = string.Empty;
        public string BestDocType { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Vmp { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public string IdVMP { get; set; } = string.Empty; // Nueva propiedad
    }
}
