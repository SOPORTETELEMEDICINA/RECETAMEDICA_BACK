using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Cim10IndicationGroup
    {
        [JsonProperty("indicationGroupId")]
        public string IndicationGroupId { get; set; }

        [JsonProperty("cim10Id")]
        public string Cim10Id { get; set; }
    }
}
