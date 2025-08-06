using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DcpfResponse
    {
        [JsonProperty("result")]
        public DcpfResult Result { get; set; }
    }
}
