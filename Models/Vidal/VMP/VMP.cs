namespace RMD.Models.Vidal.VMP
{
    public class VMP
    {
        public int IdInterno { get; set; }
        public int IdVMP { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ActivePrinciples { get; set; } = string.Empty;
        public int? GalenicFormVidalId { get; set; }
        public string? GalenicForm { get; set; } = string.Empty;
        public bool? RegulatoryGenericPrescription { get; set; }
        public int? IdVTM { get; set; }
        public string? VMP_Text { get; set; } = string.Empty;
        public string? PRODUCTS { get; set; } = string.Empty;
        public string? ATC_CLASSIFICATION { get; set; } = string.Empty;
        public string? MOLECULES { get; set; } = string.Empty;
        public string? UNITS { get; set; } = string.Empty;
        public string? CONTRAINDICATION { get; set; } = string.Empty;
        public string? PHYSICO_CHEMICAL_INTERACTIONS { get; set; } = string.Empty;
        public string? ROUTES { get; set; } = string.Empty;
        public string? INDICATORS { get; set; } = string.Empty;
        public string? INDICATIONS { get; set; } = string.Empty;
        public string? SIDE_EFFECTS { get; set; } = string.Empty;
        public string? ALDS { get; set; } = string.Empty;
        public string? UCDVS { get; set; } = string.Empty;
        public string? UCDS { get; set; } = string.Empty;
        public string? PRESCRIBABLES { get; set; } = string.Empty;
        public string? ALLERGIES { get; set; } = string.Empty;
        public string? OPT_DOCUMENT { get; set; } = string.Empty;
        public string? DOCUMENTS { get; set; } = string.Empty;
        public DateTime? VidalUpdateDate { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool Estatus { get; set; }
    }
}
