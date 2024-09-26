namespace RMD.Models.Vidal.ByProduct
{
    public class ProductRoute
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public int Ranking { get; set; }
        public bool OutOfSPC { get; set; }
        public int ParentId { get; set; }
    }
}
