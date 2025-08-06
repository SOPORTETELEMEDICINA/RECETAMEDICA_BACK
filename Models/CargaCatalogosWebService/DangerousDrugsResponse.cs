using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DangerousDrugsResponse
    {
        [JsonProperty("result")]
        public DangerousDrugsResult Result { get; set; }
    }
}
