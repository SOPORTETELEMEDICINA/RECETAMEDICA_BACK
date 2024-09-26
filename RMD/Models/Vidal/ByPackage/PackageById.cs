namespace RMD.Models.Vidal.ByPackage
{
    public class PackageById
    {
        public string Title { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string MarketStatus { get; set; } = string.Empty;
        public bool OTC { get; set; }
        public bool IsCeps { get; set; }
        public int DrugId { get; set; }
        public string Cip13 { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public bool TFR { get; set; }
        public string Company { get; set; } = string.Empty;
        public bool NarcoticPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public bool WithoutPrescription { get; set; }
        public string Product { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public string UCD { get; set; } = string.Empty;
    }
}
