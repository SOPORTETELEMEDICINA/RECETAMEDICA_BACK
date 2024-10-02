namespace RMD.Models.Vidal
{
    public class ATCDetail : VidalBaseModel
    {
         public string Route { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public bool RegulatoryGenericPrescription { get; set; }
    }
}
