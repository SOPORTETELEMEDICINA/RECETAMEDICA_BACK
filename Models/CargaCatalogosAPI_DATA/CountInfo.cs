using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA
{
    public class CountInfo
    {
        [JsonPropertyName("total")]
        public string Total { get; set; }
    }
}
