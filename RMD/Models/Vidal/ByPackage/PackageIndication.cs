namespace RMD.Models.Vidal.ByPackage
{
    public class PackageIndication
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;  
        public DateTime Updated { get; set; }
        public string VidalCategories { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;    
    }

}
