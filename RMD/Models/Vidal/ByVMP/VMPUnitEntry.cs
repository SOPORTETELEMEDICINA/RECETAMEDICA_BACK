namespace RMD.Models.Vidal.ByVMP
{
    public class VMPUnitEntry
    {
        public int IdUnit { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public string ParentConversionRate { get; set; } = string.Empty;
        public int? IdParent { get; set; }
        public string DerivedByWeight { get; set; } = string.Empty;
        public string DerivedBySize { get; set; } = string.Empty;
        public int Rank { get; set; }
    }
}
