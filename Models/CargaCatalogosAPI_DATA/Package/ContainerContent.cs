using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.Package
{
    public class ContainerContent
    {
        [JsonPropertyName("containerId")]
        public string ContainerId { get; set; }

        [JsonPropertyName("packageId")]
        public string PackageId { get; set; }

        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [JsonPropertyName("containerQuantityValue")]
        public string ContainerQuantityValue { get; set; }

        [JsonPropertyName("containerQuantityUnitId")]
        public string ContainerQuantityUnitId { get; set; }

        [JsonPropertyName("contentQuantityValue")]
        public string ContentQuantityValue { get; set; }

        [JsonPropertyName("contentQuantityUnitId")]
        public string ContentQuantityUnitId { get; set; }
    }
}
