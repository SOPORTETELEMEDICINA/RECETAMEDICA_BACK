using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.GalenicForm
{
    public class GalenicForm
    {
        [JsonPropertyName("formId")]
        public string FormId { get; set; }

        [JsonPropertyName("parentId")]
        public string ParentId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("shortname")]
        public string ShortName { get; set; }

        [JsonPropertyName("class")]
        public string Class { get; set; }
    }
}
