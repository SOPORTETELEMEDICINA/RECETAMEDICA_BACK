using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class RouteResponse
    {
        [JsonProperty("result")]
        public RouteResult Result { get; set; }
    }
}
