using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class Molecule
    {
        [JsonProperty("moleculeId")]
        public string MoleculeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("name_noaccent")]
        public string Name_NoAccent { get; set; }

        [JsonProperty("moleculesnomedId")]
        public string MoleculesnomedId { get; set; }

        [JsonProperty("moleculesnomedName")]
        public string MoleculesnomedName { get; set; }
    }
}
