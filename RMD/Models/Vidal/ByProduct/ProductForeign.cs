namespace RMD.Models.Vidal.ByProduct
{
    public class ProductForeign
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocalName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public string GalenicForm { get; set; } = string.Empty;
        public int GalenicFormId { get; set; }
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string AtcClass { get; set; } = string.Empty;
        public string AtcCode { get; set; } = string.Empty;
        public int ForeignProductId { get; set; }
    }
}
