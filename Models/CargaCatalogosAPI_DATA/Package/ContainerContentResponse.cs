using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA.Package
{
    public class ContainerContentResponse
    {
        [JsonPropertyName("result")]
        public ContainerContentResult Result { get; set; }
    }
}
