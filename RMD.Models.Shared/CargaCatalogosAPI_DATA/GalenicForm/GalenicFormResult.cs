using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.GalenicForm
{
    public class GalenicFormResult
    {
        [JsonPropertyName("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonPropertyName("count")]
        public List<CountInfo> Count { get; set; }

        [JsonPropertyName("table")]
        public List<GalenicForm> Table { get; set; }
    }
}
