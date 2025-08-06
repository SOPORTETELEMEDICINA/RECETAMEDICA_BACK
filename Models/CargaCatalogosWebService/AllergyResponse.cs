using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergyResponse
    {
        [JsonProperty("result")]
        public AllergyResult Result { get; set; }
    }
}
