namespace RMD.Models.Vidal.ByAllergy
{
    public class AllergyMolecule
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public bool SafetyAlert { get; set; }
        public bool Homeopathy { get; set; }
        public string Role { get; set; } = string.Empty;
    }

}
