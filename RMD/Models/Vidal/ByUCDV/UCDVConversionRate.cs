namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVConversionRate
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public int RefUnit { get; set; }
        public int Unit { get; set; }
        public Rate Rate { get; set; }
        public double Coeff { get; set; }
    }

    public class Rate
    {
        public int Denominator { get; set; }
        public int Numerator { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
