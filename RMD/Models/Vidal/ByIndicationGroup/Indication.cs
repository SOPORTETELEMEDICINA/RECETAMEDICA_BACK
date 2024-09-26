namespace RMD.Models.Vidal.ByIndicationGroup
{
    public class Indication
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
