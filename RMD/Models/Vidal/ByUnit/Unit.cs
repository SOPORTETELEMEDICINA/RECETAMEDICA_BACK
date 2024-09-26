namespace RMD.Models.Vidal.ByUnit
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public string ParentConversionRate { get; set; } = string.Empty;
    }
}
