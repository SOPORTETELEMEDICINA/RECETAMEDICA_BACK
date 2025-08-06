using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class AllergyMoleculeResponse
    {
        [JsonProperty("result")]
        public AllergyMoleculeResult Result { get; set; }
    }
}
