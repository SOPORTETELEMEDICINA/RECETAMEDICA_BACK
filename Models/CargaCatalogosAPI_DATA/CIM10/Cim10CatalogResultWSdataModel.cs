using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.CIM10
{
    public class Cim10CatalogResultWSdataModel
    {
        [JsonPropertyName("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonPropertyName("count")]
        public List<CountInfo> Count { get; set; }

        [JsonPropertyName("table")]
        public List<Cim10CatalogWSdataModel> Table { get; set; }
    }
}
