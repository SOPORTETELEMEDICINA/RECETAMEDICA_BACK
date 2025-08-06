using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class UCDVResponse
    {
        [JsonProperty("result")]
        public UCDVResult Result { get; set; }
    }
}
