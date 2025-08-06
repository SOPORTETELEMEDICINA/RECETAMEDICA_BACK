using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationResult
    {
        [JsonProperty("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonProperty("count")]
        public List<CountInfo> Count { get; set; }

        [JsonProperty("table")]
        public List<Indication> Table { get; set; }
    }
}
