using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugEntityComposition
    {
        [JsonProperty("drugentityId")]
        public string DrugentityId { get; set; }

        [JsonProperty("moleculeId")]
        public string MoleculeId { get; set; }

        [JsonProperty("perVolume")]
        public string PerVolume { get; set; }

        [JsonProperty("perVolumeUnit")]
        public string PerVolumeUnit { get; set; }

        [JsonProperty("perVolumeUnitId")]
        public string PerVolumeUnitId { get; set; }
    }
}
