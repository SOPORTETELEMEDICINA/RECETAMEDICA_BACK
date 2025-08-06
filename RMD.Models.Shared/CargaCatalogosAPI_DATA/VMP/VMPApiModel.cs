namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.VMP
{
    public class VMPApiModel
    {  
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public int IdVMP { get; set; }
        public string Name { get; set; }
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string ActivePrinciples { get; set; }
        public int GalenicFormVidalId { get; set; }
        public string GalenicForm { get; set; }
        public bool RegulatoryGenericPrescription { get; set; }
        // ReSharper disable once InconsistentNaming
        public int IdVTM { get; set; }
        // Nuevas propiedades para los enlaces (links)
        // ReSharper disable once InconsistentNaming
        public string VMP { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string PRODUCTS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string ATC_CLASSIFICATION { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string MOLECULES { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string UNITS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string CONTRAINDICATION { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string PHYSICO_CHEMICAL_INTERACTIONS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string ROUTES { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string INDICATORS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string INDICATIONS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string SIDE_EFFECTS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string ALDS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string UCDVS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string UCDS { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string PRESCRIBABLES { get; set; } = string.Empty;   
        // ReSharper disable once InconsistentNaming
        public string ALLERGIES { get; set; } = string.Empty;
           // ReSharper disable once InconsistentNaming
        public string OPT_DOCUMENT { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string DOCUMENTS { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
    }
}
