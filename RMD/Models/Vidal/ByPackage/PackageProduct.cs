namespace RMD.Models.Vidal.ByPackage
{
    public class PackageProduct
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public string OTC { get; set; } = string.Empty;
        public string IsCeps { get; set; } = string.Empty;
        public string DrugId { get; set; } = string.Empty;
        public string Cip13 { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public string TFR { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string NarcoticPrescription { get; set; } = string.Empty;
        public string SafetyAlert { get; set; } = string.Empty;
        public string WithoutPrescription { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public string ATCName { get; set; } = string.Empty;
        public string VMP { get; set; } = string.Empty;
        public string UCD { get; set; } = string.Empty;
        public List<string> RelatedLinks { get; set; } = [];
    }

}
