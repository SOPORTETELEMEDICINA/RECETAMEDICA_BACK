namespace RMD.Models.Vidal.ByPackage
{
    public class PackageUnit
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string VidalCategories { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public string ParentConversionRate { get; set; } = string.Empty;
        public string DerivedByWeight { get; set; } = string.Empty;
        public string DerivedBySize { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int? RefUnit { get; set; }
        public int? Unit { get; set; }
        public string Rate { get; set; } = string.Empty;
        public double? Coeff { get; set; }
    }


}
