using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergyCross
    {
        [JsonProperty("allergyId1")]
        public string AllergyId1 { get; set; }

        [JsonProperty("allergyId2")]
        public string AllergyId2 { get; set; }
    }
}
