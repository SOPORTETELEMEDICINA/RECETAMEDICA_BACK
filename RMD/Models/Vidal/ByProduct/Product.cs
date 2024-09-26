namespace RMD.Models.Vidal.ByProduct
{
    public class Product
    {
        public string Title { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}
