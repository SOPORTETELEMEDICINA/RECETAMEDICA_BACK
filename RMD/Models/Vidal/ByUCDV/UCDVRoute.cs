namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDVRoute
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public int Ranking { get; set; }
        public bool OutOfSPC { get; set; }
        public string ParentId { get; set; } = string.Empty;
    }
}
