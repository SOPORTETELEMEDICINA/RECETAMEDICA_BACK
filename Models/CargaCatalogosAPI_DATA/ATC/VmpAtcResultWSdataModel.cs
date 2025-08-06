using System.Text.Json.Serialization;
namespace RMD.Models.CargaCatalogosAPI_DATA.ATC
{
    public class VmpAtcResultWSdataModel
    {
        [JsonPropertyName("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonPropertyName("count")]
        public List<CountInfo> Count { get; set; }

        [JsonPropertyName("table")]
        public List<VmpAtcWSdataModel> Table { get; set; }
    }
}
