namespace RMD.Models.Vidal.ByIndicationGroup
{
    public class VMP
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public bool RegulatoryGenericPrescription { get; set; }
    }
}
