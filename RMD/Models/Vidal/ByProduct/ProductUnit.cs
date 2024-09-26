namespace RMD.Models.Vidal.ByProduct
{
    public class ProductUnit
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public int ParentConversionRateDenominator { get; set; }
        public int ParentConversionRateNumerator { get; set; }
        public int ParentConversionRateUnitId { get; set; }
        public string DerivedByWeight { get; set; } = string.Empty;
        public string DerivedBySize { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int ConversionRateRefUnit { get; set; }
        public int ConversionRateUnit { get; set; }
        public string ConversionRate { get; set; } = string.Empty;
        public double Coeff { get; set; }
    }
}
