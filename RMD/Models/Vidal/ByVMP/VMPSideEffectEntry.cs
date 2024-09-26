namespace RMD.Models.Vidal.ByVMP
{
    public class VMPSideEffectEntry
    {
        public int IdSideEffect { get; set; }
        public string Name { get; set; } = string.Empty;
        public int IdApparatus { get; set; }
        public string ApparatusName { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }
}
