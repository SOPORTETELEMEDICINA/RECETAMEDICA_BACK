using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationGroupIndication
    {
        [JsonProperty("indicationGroupId")]
        public string IndicationGroupId { get; set; }

        [JsonProperty("indicationId")]
        public string IndicationId { get; set; }
    }
}
