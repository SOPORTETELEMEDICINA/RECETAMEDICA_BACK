namespace RMD.Models.Vidal.ByVTM
{
    public class VTMMolecule
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public bool Homeopathy { get; set; }
        public string Role { get; set; } = string.Empty;    
    }
}
