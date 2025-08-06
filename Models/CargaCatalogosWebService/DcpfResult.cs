using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DcpfResult
    {
        [JsonProperty("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonProperty("count")]
        public List<CountInfo> Count { get; set; }

        [JsonProperty("table")]
        public List<Dcpf> Table { get; set; }
    }
}
