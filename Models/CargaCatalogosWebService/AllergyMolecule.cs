using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergyMolecule
    {
        [JsonProperty("allergyId")]
        public string AllergyId { get; set; }

        [JsonProperty("moleculeId")]
        public string MoleculeId { get; set; }
    }
}
