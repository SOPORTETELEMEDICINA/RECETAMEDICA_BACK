using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergySimplifiedResponse
    {
        [JsonProperty("result")]
        public AllergySimplifiedResult Result { get; set; }
    }
}
