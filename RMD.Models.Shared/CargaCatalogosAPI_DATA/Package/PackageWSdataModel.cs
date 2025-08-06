using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.Package
{
    public class PackageWSDATAModel
    {
        [JsonPropertyName("packageId")]
        public string PackageId { get; set; } = string.Empty;

        [JsonPropertyName("national_code")]
        public string NationalCode { get; set; } = string.Empty;

        [JsonPropertyName("visaType")]
        public string VisaType { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("pvl")]
        public string Pvl { get; set; } = string.Empty;

        [JsonPropertyName("pvp")]
        public string Pvp { get; set; } = string.Empty;

        [JsonPropertyName("pvpNotification")]
        public string PvpNotification { get; set; } = string.Empty;

        [JsonPropertyName("registrationDate")]
        public string RegistrationDate { get; set; } = string.Empty;

        [JsonPropertyName("packageContributionId")]
        public string PackageContributionId { get; set; } = string.Empty;

        [JsonPropertyName("packageStateId")]
        public string PackageStateId { get; set; } = string.Empty;

        [JsonPropertyName("packageFinancingId")]
        public string PackageFinancingId { get; set; } = string.Empty;

        [JsonPropertyName("registryStateId")]
        public string RegistryStateId { get; set; } = string.Empty;

        [JsonPropertyName("packageTypeId")]
        public string PackageTypeId { get; set; } = string.Empty;

        [JsonPropertyName("storageId")]
        public string StorageId { get; set; } = string.Empty;

        [JsonPropertyName("cpd")]
        public string Cpd { get; set; } = string.Empty;

        [JsonPropertyName("dh")]
        public string Dh { get; set; } = string.Empty;

        [JsonPropertyName("ecm")]
        public string Ecm { get; set; } = string.Empty;

        [JsonPropertyName("uh")]
        public string Uh { get; set; } = string.Empty;

        [JsonPropertyName("prescription")]
        public string Prescription { get; set; } = string.Empty;

        [JsonPropertyName("longtermTreatment")]
        public string LongtermTreatment { get; set; } = string.Empty;

        [JsonPropertyName("dhsc")]
        public string Dhsc { get; set; } = string.Empty;

        [JsonPropertyName("packageCommercializationId")]
        public string PackageCommercializationId { get; set; } = string.Empty;

        [JsonPropertyName("dhgdId")]
        public string DhgdId { get; set; } = string.Empty;

        [JsonPropertyName("volumeml")]
        public string VolumeMl { get; set; } = string.Empty;

        [JsonPropertyName("volumeml_unit")]
        public string VolumeMlUnit { get; set; } = string.Empty;

        [JsonPropertyName("contentgram")]
        public string ContentGram { get; set; } = string.Empty;

        [JsonPropertyName("contentgram_unit")]
        public string ContentGramUnit { get; set; } = string.Empty;

        [JsonPropertyName("contents")]
        public string Contents { get; set; } = string.Empty;

        [JsonPropertyName("dcsacode")]
        public string Dcsacode { get; set; } = string.Empty;

        [JsonPropertyName("packagecontainerId")]
        public string PackageContainerId { get; set; } = string.Empty;

        [JsonPropertyName("multiple")]
        public string Multiple { get; set; } = string.Empty;

        [JsonPropertyName("contents_unit_Id")]
        public string ContentsUnitId { get; set; } = string.Empty;

        [JsonPropertyName("billable")]
        public string Billable { get; set; } = string.Empty;

        [JsonPropertyName("pvlFinancing")]
        public string PvlFinancing { get; set; } = string.Empty;

        [JsonPropertyName("pvlNotification")]
        public string PvlNotification { get; set; } = string.Empty;

        [JsonPropertyName("pvpFinancing")]
        public string PvpFinancing { get; set; } = string.Empty;

        [JsonPropertyName("ean13")]
        public string Ean13 { get; set; } = string.Empty;

        [JsonPropertyName("listId")]
        public string ListId { get; set; } = string.Empty;

        [JsonPropertyName("serialization")]
        public string Serialization { get; set; } = string.Empty;

        [JsonPropertyName("packageFormatId")]
        public string PackageFormatId { get; set; } = string.Empty;

        [JsonPropertyName("pvpiva")]
        public string PvpIva { get; set; } = string.Empty;

        [JsonPropertyName("pvlfinan")]
        public string PvlFinan { get; set; } = string.Empty;

        [JsonPropertyName("pvpfinan")]
        public string PvpFinan { get; set; } = string.Empty;

        [JsonPropertyName("lowerPrice")]
        public string LowerPrice { get; set; } = string.Empty;

        [JsonPropertyName("lowestPrice")]
        public string LowestPrice { get; set; } = string.Empty;

        [JsonPropertyName("discountpercentRD")]
        public string DiscountPercentRD { get; set; } = string.Empty;

        [JsonPropertyName("ucdvId")]
        public string UcdvId { get; set; } = string.Empty;

        [JsonPropertyName("scp")]
        public string Scp { get; set; } = string.Empty;

        [JsonPropertyName("laboratoryTraderId")]
        public string LaboratoryTraderId { get; set; } = string.Empty;

        [JsonPropertyName("psicoannexid")]
        public string PsicoAnnexId { get; set; } = string.Empty;
    }
}
