namespace RMD.Models.Vidal.ByUnit
{
    public class Units
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public string ParentConversionRate { get; set; } = string.Empty;
    }
}
