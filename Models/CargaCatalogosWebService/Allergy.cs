using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Allergy
    {
        [JsonProperty("allergyId")]
        public string AllergyId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("allergysnomedId")]
        public string AllergysnomedId { get; set; }

        [JsonProperty("allergysnomedName")]
        public string AllergysnomedName { get; set; }
    }
}
