namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.UCDV
{
    public class UCDVApiModel
    {
        public int IdUCDV { get; set; }
        public string Summary { get; set; }
        public string Name { get; set; }
        public int IdConditioningUnit { get; set; }
        public string ConditioningUnit { get; set; }
        public decimal Quantity { get; set; }
        public int QuantityUnitId { get; set; }
        public string QuantityUnit { get; set; } = string.Empty;
        public int GalenicFormId { get; set; }
        public string GalenicForm { get; set; }
        public string RoutesLink { get; set; }
        public string UnitsLink { get; set; }
        public string MoleculesLink { get; set; }
        public string PackagesLink { get; set; }
        public string ProductsLink { get; set; }
        public string PrescribablesLink { get; set; }
        public string VmpLink { get; set; }

        public string NationalVmppsLink { get; set; }
        public string NationalVmpsLink { get; set; }
    }
}
