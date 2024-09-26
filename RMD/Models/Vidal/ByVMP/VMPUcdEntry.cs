namespace RMD.Models.Vidal.ByVMP
{
    public class VMPUcdEntry
    {
        public int IdUcd { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarketStatus { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
    }
}
