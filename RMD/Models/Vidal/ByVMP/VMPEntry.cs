namespace RMD.Models.Vidal.ByVMP
{
    public class VMPEntry
    {
        public int IdVmp { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public string RouteValue { get; set; } = string.Empty;
        public int GalenicFormId { get; set; }
        public string GalenicFormValue { get; set; } = string.Empty;
        public bool RegulatoryGenericPrescription { get; set; }
        public int? IdVTM { get; set; }
    }
}
