using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class ClassdcResponse
    {
        [JsonProperty("result")]
        public ClassdcResult Result { get; set; }
    }
}
