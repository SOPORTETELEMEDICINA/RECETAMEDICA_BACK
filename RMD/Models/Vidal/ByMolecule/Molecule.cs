namespace RMD.Models.Vidal.ByMolecule
{
    public class Molecule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public bool Homeopathy { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
