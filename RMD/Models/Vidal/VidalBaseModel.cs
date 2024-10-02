using RMD.Models.Shared;

namespace RMD.Models.Vidal
{
    public class VidalBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public List<RelatedLink> RelatedLinks { get; set; } = new();
    }
}
