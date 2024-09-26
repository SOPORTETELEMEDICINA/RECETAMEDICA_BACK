namespace RMD.Models.Vidal.ByVMP
{
    public class VMPIndicationEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Puede ser "INDICATION" o "INDICATION_GROUP"
    }
}
