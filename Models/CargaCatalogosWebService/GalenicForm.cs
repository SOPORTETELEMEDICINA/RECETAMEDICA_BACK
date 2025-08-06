using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class GalenicForm
    {
        [JsonProperty("formId")]
        public string FormId { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortname")]
        public string ShortName { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }
    }
}
