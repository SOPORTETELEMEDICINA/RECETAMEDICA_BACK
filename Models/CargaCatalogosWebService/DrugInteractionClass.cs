using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugInteractionClass
    {
        [JsonProperty("drugInteractionClassId")]
        public string DrugInteractionClassId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
