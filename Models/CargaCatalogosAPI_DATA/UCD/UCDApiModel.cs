namespace RMD.Models.CargaCatalogosAPI_DATA.UCD
{
    public class UCDApiModel
    {
        public int IdUCD { get; set; }
        public string Summary { get; set; }
        public string Name { get; set; }
        public string MarketStatus { get; set; }
        public bool SafetyAlert { get; set; }
        public int IdVmp { get; set; }
        public string VmpDescription { get; set; }
        public string UnitsLink { get; set; }
        public string RoutesLink { get; set; }
        public string IndicatorsLink { get; set; }
        public string PackagesLink { get; set; }
        public string ProductsLink { get; set; }
        public string SideEffectsLink { get; set; }
        public string PrescribablesLink { get; set; }
        public string AtcClassificationLink { get; set; }
        public string ProductLink { get; set; }
        public string MoleculesLink { get; set; }
        public string ActiveExcipientsLink { get; set; }
        public string VmpLink { get; set; }
    }
}
