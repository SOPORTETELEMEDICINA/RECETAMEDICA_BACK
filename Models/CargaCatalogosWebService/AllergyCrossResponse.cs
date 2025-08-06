using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergyCrossResponse
    {
        [JsonProperty("result")]
        public AllergyCrossResult Result { get; set; }
    }
}
