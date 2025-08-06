using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugInteractionClassResponse
    {
        [JsonProperty("result")]
        public DrugInteractionClassResult Result { get; set; }
    }
}
