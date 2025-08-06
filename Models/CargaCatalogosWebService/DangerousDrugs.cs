using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DangerousDrugs
    {
        [JsonProperty("national_code")]
        public string NationalCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("presentation")]
        public string Presentation { get; set; }

        [JsonProperty("preparation_recommendations")]
        public string PreparationRecommendations { get; set; }

        [JsonProperty("administration_recommendations")]
        public string AdministrationRecommendations { get; set; }

        [JsonProperty("dangerous_cause")]
        public string DangerousCause { get; set; }

        [JsonProperty("List_NIOSH")]
        public string ListNIOSH { get; set; }
    }
}
