using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Cim10IndicationGroupResponse
    {
        [JsonProperty("result")]
        public Cim10IndicationGroupResult Result { get; set; }
    }
}
