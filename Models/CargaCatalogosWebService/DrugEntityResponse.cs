using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugEntityResponse
    {
        [JsonProperty("result")]
        public DrugEntityResult Result { get; set; }
    }
}
