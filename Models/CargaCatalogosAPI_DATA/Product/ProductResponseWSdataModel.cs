using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.Product
{
    public class ProductResponseWSdataModel
    {
        [JsonPropertyName("result")]
        public ProductResultWSdataModel Result { get; set; }
    }
}
