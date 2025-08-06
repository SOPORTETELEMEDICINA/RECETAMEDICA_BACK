using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationGroupResponse
    {
        [JsonProperty("result")]
        public IndicationGroupResult Result { get; set; }
    }
}
