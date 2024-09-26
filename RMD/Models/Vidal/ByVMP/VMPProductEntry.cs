namespace RMD.Models.Vidal.ByVMP
{
    public class VMPProductEntry
    {
        public int IdProduct { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public int IdVMP { get; set; }
        public int IdCompany { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int IdGalenicForm { get; set; }
        public string GalenicForm { get; set; } = string.Empty;
    }
}
