using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Contraindication
    {
        [JsonProperty("contraIndicationId")]
        public string ContraindicationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
