namespace RMD.Models.Consulta
{
    public class AllergyEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public List<RelatedLink> RelatedLinks { get; set; } = [];
    }
}
