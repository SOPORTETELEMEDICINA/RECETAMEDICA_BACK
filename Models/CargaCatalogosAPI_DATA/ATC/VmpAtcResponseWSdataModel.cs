using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosAPI_DATA.ATC
{
    public class VmpAtcResponseWSdataModel
    {
        [JsonProperty("result")]
        public VmpAtcResultWSdataModel Result { get; set; }
    }


}
