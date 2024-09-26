namespace RMD.Models.Vidal.ByUCDV
{
    public class UCDV
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Updated { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public int GalenicFormVidalId { get; set; }
    }

}
