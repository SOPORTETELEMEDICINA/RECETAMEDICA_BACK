using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Indication
    {
        [JsonProperty("indicationId")]
        public string IndicationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_noaccent")]
        public string Name_NoAccent { get; set; }
    }
}
