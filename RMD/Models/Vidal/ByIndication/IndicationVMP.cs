namespace RMD.Models.Vidal.ByIndication
{
    public class IndicationVMP
    {
        public string Title { get; set; } = string.Empty;
        public string VmpLink { get; set; } = string.Empty;
        public string ProductsLink { get; set; } = string.Empty;
        public string AtcClassificationLink { get; set; } = string.Empty;
        public string MoleculesLink { get; set; } = string.Empty;
        public string UnitsLink { get; set; } = string.Empty;
        public string ContraindicationsLink { get; set; } = string.Empty;
        public string PhysicoChemicalInteractionsLink { get; set; } = string.Empty;
        public string RoutesLink { get; set; } = string.Empty;
        public string IndicatorsLink { get; set; } = string.Empty;
        public string IndicationsLink { get; set; } = string.Empty;
        public string SideEffectsLink { get; set; } = string.Empty;
        public string AldsLink { get; set; } = string.Empty;
        public string UcdvsLink { get; set; } = string.Empty;
        public string UcdsLink { get; set; } = string.Empty;
        public string PrescribablesLink { get; set; } = string.Empty;
        public string AllergiesLink { get; set; } = string.Empty;
        public string OptDocumentsLink { get; set; } = string.Empty;
        public string DocumentsLink { get; set; } = string.Empty;
        public string VtmLink { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;
        public bool RegulatoryGenericPrescription { get; set; }
    }

    public class IndicationVMPResponse
    {
        public int StartIndex { get; set; }
        public int TotalResults { get; set; }
        public int ItemsPerPage { get; set; }
        public List<IndicationVMP> Vmps { get; set; } = [];
    }
}
