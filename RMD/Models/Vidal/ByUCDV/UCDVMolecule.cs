namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVMolecule
    {
        public string Title { get; set; } = string.Empty;
        public string AlternateLink { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string PerVolume { get; set; } = string.Empty;
        public int PerVolumeValue { get; set; }
        public string PerVolumeUnit { get; set; } = string.Empty;
        public int TotalQuantityValue { get; set; }
        public string TotalQuantityUnit { get; set; } = string.Empty;
    }
}
