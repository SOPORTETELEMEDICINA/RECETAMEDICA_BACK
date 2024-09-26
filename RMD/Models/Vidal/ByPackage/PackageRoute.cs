namespace RMD.Models.Vidal.ByPackage
{
    public class PackageRoute
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public int Ranking { get; set; }
        public bool OutOfSPC { get; set; }
        public int ParentId { get; set; }
    }
}
