using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.Package
{
    public class PackageResponseWSdataModel
    {
        [JsonPropertyName("result")]
        public PackageResultWSdataModel Result { get; set; }
    }
}
