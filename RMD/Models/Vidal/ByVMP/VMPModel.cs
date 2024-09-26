namespace RMD.Models.Vidal.ByVMP
{
    public class VMPModel
    {
        public int IdVMP { get; set; }
        public string Name { get; set; }
        public string ActivePrinciples { get; set; }
        public int IdRoute { get; set; }
        public string RouteName { get; set; }
        public int GalenicFormVidalId { get; set; }
        public string GalenicForm { get; set; }
        public bool RegulatoryGenericPrescription { get; set; }
        public int IdVTM { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

}
