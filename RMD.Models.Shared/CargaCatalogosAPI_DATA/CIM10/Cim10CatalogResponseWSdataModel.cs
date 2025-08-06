using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.CIM10
{
    public class Cim10CatalogResponseWSdataModel
    {
        [JsonPropertyName("result")]
        public Cim10CatalogResultWSdataModel Result { get; set; }
    }
}

