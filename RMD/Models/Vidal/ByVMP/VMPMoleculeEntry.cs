namespace RMD.Models.Vidal.ByVMP
{
    public class VMPMoleculeEntry
    {
        public int IdMolecule { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public double PerVolumeValue { get; set; }
        public string PerVolumeUnit { get; set; } = string.Empty;
    }
}
