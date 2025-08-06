using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.GalenicForm
{
    public class GalenicFormResponse
    {
        [JsonPropertyName("result")]
        public GalenicFormResult Result { get; set; }
    }
}
