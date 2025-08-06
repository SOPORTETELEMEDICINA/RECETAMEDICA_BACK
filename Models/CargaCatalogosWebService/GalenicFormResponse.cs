using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class GalenicFormResponse
    {
        [JsonProperty("result")]
        public GalenicFormResult Result { get; set; }
    }
}
