using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergySimplified
    {
        [JsonProperty("moleculeId")]
        public string MoleculeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
