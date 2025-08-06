using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Cim10ContraindicationResponse
    {
        [JsonProperty("result")]
        public Cim10ContraindicationResult Result { get; set; }
    }
}
