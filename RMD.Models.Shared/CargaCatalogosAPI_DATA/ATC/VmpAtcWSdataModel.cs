using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC
{
    public class VmpAtcWSdataModel
    {
        [JsonPropertyName("atcclassId")]
        public string AtcClassId { get; set; }

        [JsonPropertyName("commonnamegroupId")]
        public string VmpId { get; set; }
    }
}
