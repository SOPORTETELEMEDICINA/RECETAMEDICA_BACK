namespace RMD.Models.Vidal.ByRoute
{
    public class Routes
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public bool Systemic { get; set; }
        public bool Topical { get; set; }
        public int ParentId { get; set; }
    }
}
