using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DcpResponse
    {
        [JsonProperty("result")]
        public DcpResult Result { get; set; }
    }
}
