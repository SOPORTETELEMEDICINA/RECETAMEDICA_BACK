using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationGroupIndicationResponse
    {
        [JsonProperty("result")]
        public IndicationGroupIndicationResult Result { get; set; }
    }
}
