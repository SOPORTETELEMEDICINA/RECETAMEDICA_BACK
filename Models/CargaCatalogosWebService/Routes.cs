using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Routes
    {
        [JsonProperty("routeId")]
        public string RouteId { get; set; }
        [JsonProperty("parentId")]
        public string ParentId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("shortname")]
        public string ShortName { get; set; }
    }

}
