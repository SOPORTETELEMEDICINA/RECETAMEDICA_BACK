using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Dcp
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
