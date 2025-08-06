namespace RMD.Shared.Models.Vidal.Package
{
    public class Package
    {
        public int IdInterno { get; set; }
        public int IdPackage { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Summary { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string? MarketStatus { get; set; } = string.Empty;
        public bool Otc { get; set; }
        public bool IsCeps { get; set; }
        public int DrugId { get; set; }
        public string? Cip13 { get; set; } = string.Empty;
        public string? ShortLabel { get; set; } = string.Empty;
        public bool Tfr { get; set; }
        public int IdCompany { get; set; }
        public string? CompanyName { get; set; } = string.Empty;
        public bool NarcoticPrescription { get; set; }
        public bool SafetyAlert { get; set; }
        public bool WithoutPrescription { get; set; }
        public int IdGalenicForm { get; set; }
        public string? GalenicForm { get; set; } = string.Empty;
        public string? UcdCode13 { get; set; } = string.Empty;
        public string? UcdCode7 { get; set; } = string.Empty;
        public int UcdId { get; set; }
        public DateTime VidalUpdateDate { get; set; }
        public string? VisaType { get; set; } = string.Empty;
        public string? Price { get; set; } = string.Empty;
        public string? Pvl { get; set; } = string.Empty;
        public string? Pvp { get; set; } = string.Empty;
        public string? PvpNotification { get; set; } = string.Empty;
        public string? RegistrationDate { get; set; } = string.Empty;
        public string? PackageContributionId { get; set; } = string.Empty;
        public string? PackageStateId { get; set; } = string.Empty;
        public string? PackageFinancingId { get; set; } = string.Empty;
        public string? RegistryStateId { get; set; } = string.Empty;
        public string? PackageTypeId { get; set; } = string.Empty;
        public string? StorageId { get; set; } = string.Empty;
        public string? Cpd { get; set; } = string.Empty;
        public string? Dh { get; set; } = string.Empty;
        public string? Ecm { get; set; } = string.Empty;
        public string? Uh { get; set; } = string.Empty;
        public string? Prescription { get; set; } = string.Empty;
        public string? LongtermTreatment { get; set; } = string.Empty;
        public string? Dhsc { get; set; } = string.Empty;
        public string? PackageCommercializationId { get; set; } = string.Empty;
        public string? DhgdId { get; set; } = string.Empty;
        public string? VolumeMl { get; set; } = string.Empty;
        public string? VolumeMl_Unit { get; set; } = string.Empty;
        public string? ContentGram { get; set; } = string.Empty;
        public string? ContentGram_Unit { get; set; } = string.Empty;
        public string? Contents { get; set; } = string.Empty;
        public string? Dcsacode { get; set; } = string.Empty;
        public string? PackageContainerId { get; set; } = string.Empty;
        public string? Multiple { get; set; } = string.Empty;
        public string? Contents_Unit_Id { get; set; } = string.Empty;
        public string? Billable { get; set; } = string.Empty;
        public string? PvlFinancing { get; set; } = string.Empty;
        public string? PvlNotification { get; set; } = string.Empty;
        public string? PvpFinancing { get; set; } = string.Empty;
        public string? Ean13 { get; set; } = string.Empty;
        public string? ListId { get; set; } = string.Empty;
        public string? Serialization { get; set; } = string.Empty;
        public string? PackageFormatId { get; set; } = string.Empty;
        public string? PvpIva { get; set; } = string.Empty;
        public string? Pvlfinan { get; set; } = string.Empty;
        public string? Pvpfinan { get; set; } = string.Empty;
        public string? LowerPrice { get; set; } = string.Empty;
        public string? LowestPrice { get; set; } = string.Empty;
        public string? DiscountPercentRD { get; set; } = string.Empty;
        public string? UcdvId { get; set; } = string.Empty;
        public string? Scp { get; set; } = string.Empty;
        public string? LaboratoryTraderId { get; set; } = string.Empty;
        public string? PsicoAnnexId { get; set; } = string.Empty;

        public string? LARGER_PACKS { get; set; } = string.Empty;
        public string? AFFILIATION_CENTER { get; set; } = string.Empty;
        public string? PDS { get; set; } = string.Empty;
        public string? PRICING_SCHEDULE { get; set; } = string.Empty;
        public string? UNITS { get; set; } = string.Empty;
        public string? ROUTES { get; set; } = string.Empty;
        public string? INDICATORS { get; set; } = string.Empty;
        public string? INDICATIONS { get; set; } = string.Empty;
        public string? SIDE_EFFECTS { get; set; } = string.Empty;
        public string? ALDS { get; set; } = string.Empty;
        public string? VAT_EXC_AFFILIATION_CENTER { get; set; } = string.Empty;
        public string? REFUND_INDICATIONS { get; set; } = string.Empty;
        public string? OPT_DOCUMENT { get; set; } = string.Empty;
        public string? DOCUMENT { get; set; } = string.Empty;
        public string? UCD { get; set; } = string.Empty;
        public string? NATIONAL_VMPPS { get; set; } = string.Empty;

        public DateTime? FechaActualizacion { get; set; }
        public bool? Estatus { get; set; }
    }

}
