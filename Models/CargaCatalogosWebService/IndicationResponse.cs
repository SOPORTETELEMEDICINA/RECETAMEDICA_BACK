using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationResponse
    {
        [JsonProperty("result")]
        public IndicationResult Result { get; set; }
    }
}
