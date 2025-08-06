using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC
{
    public class VmpAtcWSdataModel
    {
        [JsonPropertyName("atcclassid")]
        public string AtcClassId { get; set; }

        [JsonPropertyName("commonnamegroupid")]
        public string VmpId { get; set; }
    }
}
