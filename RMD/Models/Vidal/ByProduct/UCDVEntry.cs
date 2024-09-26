namespace RMD.Models.Vidal.ByProduct
{
    public class UCDVEntry
    {
        public int VidalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GalenicFormVidalId { get; set; }
        public string GalenicFormName { get; set; } = string.Empty;
        public int VmpId { get; set; }
    }
}
