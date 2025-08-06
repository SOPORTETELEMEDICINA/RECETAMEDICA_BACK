using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Dcpf
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code_dcp")]
        public string CodeDcp { get; set; }
    }
}
