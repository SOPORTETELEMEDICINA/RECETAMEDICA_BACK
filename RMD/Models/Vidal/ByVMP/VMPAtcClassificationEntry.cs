namespace RMD.Models.Vidal.ByVMP
{
    public class VMPAtcClassificationEntry
    {
        public int IdAtcClassification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? IdParent { get; set; } // Puede ser nulo si no existe un padre
    }
}
