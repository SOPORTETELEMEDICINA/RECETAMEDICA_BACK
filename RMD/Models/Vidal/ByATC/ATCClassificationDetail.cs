namespace RMD.Models.Vidal.ByATC
{
    public class ATCClassificationDetail
    {
        public string Id { get; set; } = string.Empty;  
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;    
        public DateTime Updated { get; set; }
        public string ProductsLink { get; set; } = string.Empty;
        public string ChildrenLink { get; set; } = string.Empty;
        public string MoleculesLink { get; set; } = string.Empty;
        public string VMPsLink { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }

}
