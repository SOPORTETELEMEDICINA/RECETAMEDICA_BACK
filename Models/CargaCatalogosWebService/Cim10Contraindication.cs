using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Cim10Contraindication
    {
        [JsonProperty("contraIndicationId")]
        public string ContraIndicationId { get; set; }

        [JsonProperty("cim10Id")]
        public string Cim10Id { get; set; }
    }
}
