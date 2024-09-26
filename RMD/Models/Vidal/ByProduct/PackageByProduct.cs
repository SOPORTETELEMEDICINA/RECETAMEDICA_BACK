namespace RMD.Models.Vidal.ByProduct
{
    public class PackageByProduct
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool Otc { get; set; }
        public bool IsCeps { get; set; }
        public string DrugId { get; set; } = string.Empty;
        public string Cip13 { get; set; } = string.Empty;
        public string ShortLabel { get; set; } = string.Empty;
        public bool Tfr { get; set; }
        public string Company { get; set; } = string.Empty;
        public bool NarcoticPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public bool WithoutPrescription { get; set; }
        public string Product { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public string Ucd { get; set; } = string.Empty;
    }

}
