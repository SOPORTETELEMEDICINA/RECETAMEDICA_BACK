namespace RMD.Models.Vidal.ByProduct
{
    public class ProductMolecule
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool Reconstitued { get; set; }
        public string HeaderName { get; set; } = string.Empty;
        public double HeaderQuantity { get; set; }
        public string MoleculeTitle { get; set; } = string.Empty;
        public int MoleculeId { get; set; }
        public bool SafetyAlert { get; set; }
        public int Ranking { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string PerVolume { get; set; } = string.Empty;
        public double PerVolumeValue { get; set; }
        public int UnitId { get; set; }
    }
}
