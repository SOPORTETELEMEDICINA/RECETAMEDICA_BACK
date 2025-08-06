using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class CountInfo
    {
        [JsonProperty("total")]
        public string Total { get; set; }
    }
}
