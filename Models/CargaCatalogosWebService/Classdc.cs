using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Classdc
    {
        [JsonProperty("classdcId")]
        public string ClassdcId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
