using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.CIM10
{

    public class Cim10CatalogWSdataModel
    {
        [JsonPropertyName("cim10id")]
        public string Cim10Id { get; set; }

        [JsonPropertyName("parentid")]
        public string ParentId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
