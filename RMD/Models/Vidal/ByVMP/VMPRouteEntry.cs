namespace RMD.Models.Vidal.ByVMP
{
    public class VMPRouteEntry
    {
        public int IdRoute { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public int Ranking { get; set; }
        public bool OutOfSPC { get; set; }
        public int ParentId { get; set; }
    }
}
