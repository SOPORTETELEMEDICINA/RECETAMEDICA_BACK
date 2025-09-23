using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.CIM10
{

    public class Cim10CatalogWSdataModel
    {
        [JsonPropertyName("cim10Id")]
        public string Cim10Id { get; set; }

        [JsonPropertyName("parentId")]
        public string ParentId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
