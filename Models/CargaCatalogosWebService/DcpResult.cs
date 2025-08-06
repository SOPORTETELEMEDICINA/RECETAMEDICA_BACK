using Newtonsoft.Json;
using System.Runtime.Intrinsics.Arm;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DcpResult
    {
        [JsonProperty("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonProperty("count")]
        public List<CountInfo> Count { get; set; }

        [JsonProperty("table")]
        public List<Dcp> Table { get; set; }
    }
}
