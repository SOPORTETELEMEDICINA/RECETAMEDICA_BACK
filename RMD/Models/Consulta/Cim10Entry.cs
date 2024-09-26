namespace RMD.Models.Consulta
{
    public class Cim10Entry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public List<RelatedLink> RelatedLinks { get; set; } = new List<RelatedLink>();
    }
}
