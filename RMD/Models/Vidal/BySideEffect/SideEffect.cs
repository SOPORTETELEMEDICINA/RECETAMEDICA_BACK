namespace RMD.Models.Vidal.BySideEffect
{
    public class SideEffect
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Apparatus { get; set; } = string.Empty;
        public int ApparatusVidalId { get; set; }
    }

}
