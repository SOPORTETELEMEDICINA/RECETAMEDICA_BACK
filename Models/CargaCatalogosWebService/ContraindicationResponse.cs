using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class ContraindicationResponse
    {
        [JsonProperty("result")]
        public ContraindicationResult Result { get; set; }
    }
}
