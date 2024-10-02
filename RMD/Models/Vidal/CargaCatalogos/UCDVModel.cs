namespace RMD.Models.Vidal.CargaCatalogos
{
    public class UCDVModel
    {
        public int IdUCDV { get; set; }
        public string Name { get; set; }
        public int IdconditioningUnit { get; set; }
        public decimal Quantity { get; set; }
        public int QuantityUnitId { get; set; }
        public string QuantityUnit { get; set; }
        public int GalenicFormId { get; set; }
        public string GalenicForm { get; set; }
    }

}
