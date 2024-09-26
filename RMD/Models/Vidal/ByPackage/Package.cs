namespace RMD.Models.Vidal.ByPackage
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string MarketStatus { get; set; } = string.Empty;
        public bool Otc { get; set; }
        public bool IsCeps { get; set; }
        public int DrugId { get; set; }
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
