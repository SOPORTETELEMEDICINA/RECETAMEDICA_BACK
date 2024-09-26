namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVUnit
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string DerivedByWeight { get; set; } = string.Empty;
        public string DerivedBySize { get; set; } = string.Empty;
        public int Rank { get; set; }
        public ParentConversionRate ParentConversionRate { get; set; } 
    }

    public class ParentConversionRate
    {
        public int Denominator { get; set; }
        public decimal Numerator { get; set; }
        public int UnitId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
