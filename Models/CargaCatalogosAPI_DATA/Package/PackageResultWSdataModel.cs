using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.Package
{
    public class PackageResultWSdataModel
    {
        [JsonPropertyName("params_info")]
        public List<ParamInfo> ParamsInfo { get; set; }

        [JsonPropertyName("count")]
        public List<CountInfo> Count { get; set; }

        [JsonPropertyName("table")]
        public List<PackageWSDATAModel> Table { get; set; }
    }
}
