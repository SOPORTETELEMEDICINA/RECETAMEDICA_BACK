using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC
{
    public class VmpAtcResponseWSdataModel
    {
        [JsonPropertyName("result")]
        public VmpAtcResultWSdataModel Result { get; set; }
    }


}
