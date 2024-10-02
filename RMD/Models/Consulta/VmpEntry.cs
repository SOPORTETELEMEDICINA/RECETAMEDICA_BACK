using RMD.Models.Shared;

namespace RMD.Models.Consulta
{
    public class VmpEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public bool RegulatoryGenericPrescription { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<RelatedLink> RelatedLinks { get; set; } = [];
    }
}
