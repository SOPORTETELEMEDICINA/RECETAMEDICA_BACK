using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class MoleculeResponse
    {
        [JsonProperty("result")]
        public MoleculeResult Result { get; set; }
    }
}
