using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugEntityCompositionResponse
    {
        [JsonProperty("result")]
        public DrugEntityCompositionResult Result { get; set; }
    }
}
