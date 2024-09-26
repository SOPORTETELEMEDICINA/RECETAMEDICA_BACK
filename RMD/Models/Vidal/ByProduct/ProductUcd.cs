namespace RMD.Models.Vidal.ByProduct
{
    public class ProductUcd
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public string UcdId { get; set; } = string.Empty;
        public string Ucd13 { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public int ProductId { get; set; }
        public int UcdLinkedId { get; set; }
    }
}
