namespace RMD.Models.Vidal.ByUCD
{
    public class Ucd
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public DateTime Updated { get; set; }
        public string Summary { get; set; }
        public int VidalId { get; set; }
        public string Name { get; set; }
        public string MarketStatus { get; set; }
        public bool SafetyAlert { get; set; }
        public string VmpName { get; set; }
        public int VmpVidalId { get; set; }
    }

}
