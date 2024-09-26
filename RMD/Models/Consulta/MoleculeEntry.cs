namespace RMD.Models.Consulta
{
    public class MoleculeEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string VidalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public bool Homeopathy { get; set; }
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<RelatedLink> RelatedLinks { get; set; } = [];
    }
}
