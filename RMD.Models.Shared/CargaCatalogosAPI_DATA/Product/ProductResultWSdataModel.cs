using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.Product
{
    public class ProductResultWSdataModel
    {
        [JsonPropertyName("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }
        [JsonPropertyName("count")]
        public List<CountInfo> Count { get; set; }
        [JsonPropertyName("table")]
        public List<ProductWSdataModel> Table { get; set; }
    }
}
