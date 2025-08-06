using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class ATCClass
    {
        [JsonProperty("atcClassId")]
        public string AtcClassId { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
