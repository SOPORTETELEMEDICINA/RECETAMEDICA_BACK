using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class IndicationGroup
    {
        [JsonProperty("indicationGroupId")]
        public string IndicationGroupId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
