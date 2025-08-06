namespace RMD.Shared.Models.Vidal.Producto
{
    public class Producto
    {
        public int IdInterno { get; set; }
        public int IdProduct { get; set; }
        public string? Summary { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? IdItemType { get; set; } = string.Empty;
        public string? IdMarketStatus { get; set; } = string.Empty;
        public bool HasPublishedDoc { get; set; }
        public bool WithoutPrescription { get; set; }
        public int IdAmmType { get; set; }
        public string? BestDocType { get; set; } = string.Empty;
        public bool SafetyAlert { get; set; }
        public int IdCompany { get; set; }
        public string? CompanyName { get; set; } = string.Empty;
        public string? TypeCompany { get; set; } = string.Empty;
        public int IdVmp { get; set; }
        public int IdGalenicForm { get; set; }
        public string? GalenicForm { get; set; } = string.Empty;
        public DateTime VidalUpdateDate { get; set; }
        public string? GalenicName { get; set; } = string.Empty;
        public string? AtcCode { get; set; } = string.Empty;
        public string? AtcCodeMJ { get; set; } = string.Empty;
        public int? Pictogram { get; set; }
        public bool? IsNarcotic { get; set; }
        public string? PsicoAnnexId { get; set; } = string.Empty;
        public string? Monodrug { get; set; } = string.Empty;
        public string? Perfus_Ml { get; set; } = string.Empty;
        public string? Admin_Gota { get; set; } = string.Empty;
        public bool? Orphan { get; set; }
        public bool? Biosimilar { get; set; }
        public bool? NoRep { get; set; }
        public string? NoRepTypeId { get; set; } = string.Empty;
        public bool? No_Comp { get; set; }
        public bool? Parallel_Import { get; set; }
        public bool? Radiopharmaceutical { get; set; }
        public bool? Mar { get; set; }
        public bool? Mar_Pac { get; set; }

        public string? PACKAGES { get; set; } = string.Empty;
        public string? MOLECULES { get; set; } = string.Empty;
        public string? ACTIVE_EXCIPIENTS { get; set; } = string.Empty;
        public string? RECOS { get; set; } = string.Empty;
        public string? FOREIGN_PRODUCTS { get; set; } = string.Empty;
        public string? INDICATIONS { get; set; } = string.Empty;
        public string? CONTRAINDICATION { get; set; } = string.Empty;
        public string? RESTRICTED_PRESCRIPTIONS { get; set; } = string.Empty;
        public string? PDS { get; set; } = string.Empty;
        public string? UCDS { get; set; } = string.Empty;
        public string? UNITS { get; set; } = string.Empty;
        public string? FOOD_INTERACTIONS { get; set; } = string.Empty;
        public string? PHYSICO_CHEMICAL_INTERACTIONS { get; set; } = string.Empty;
        public string? ROUTES { get; set; } = string.Empty;
        public string? INDICATORS { get; set; } = string.Empty;
        public string? SIDE_EFFECTS { get; set; } = string.Empty;
        public string? ALDS { get; set; } = string.Empty;
        public string? UCDVS { get; set; } = string.Empty;
        public string? ALLERGIES { get; set; } = string.Empty;
        public string? NATIONAL_VMPS { get; set; } = string.Empty;

        public DateTime? FechaActualizacion { get; set; }
        public bool? Estatus { get; set; }
    }
}
