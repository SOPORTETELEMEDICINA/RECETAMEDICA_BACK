using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class ATCClassResponse
    {
        [JsonProperty("result")]
        public ATCClassResult Result { get; set; }
    }
}
