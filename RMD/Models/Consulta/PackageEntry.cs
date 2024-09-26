namespace RMD.Models.Consulta
{
    public class PackageEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;  // Agrega esta línea.
        public string Name { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
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
        public string Vmp { get; set; } = string.Empty;
        public string Ucd { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;  // Agrega esta línea.
        public List<RelatedLink> RelatedLinks { get; set; } = new List<RelatedLink>(); // Agrega esta línea.
    }
}