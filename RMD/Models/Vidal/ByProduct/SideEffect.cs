namespace RMD.Models.Vidal.ByProduct
{
    public class SideEffect
    {
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ApparatusVidalId { get; set; }
        public string ApparatusName { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }
}
