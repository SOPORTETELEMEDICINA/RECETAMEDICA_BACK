using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class UCDV
    {
        [JsonProperty("ucdvId")]
        public string UCDVId { get; set; }
        [JsonProperty("commonNameGroupId")]
        public string CommonNameGroupId { get; set; }
        [JsonProperty("unitId")]
        public string UnitId { get; set; }
        [JsonProperty("condiId")]
        public string CondiId { get; set; }
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("formId")]
        public string FormId { get; set; }
        [JsonProperty("divisibilityValue")]
        public string DivisibilityValue { get; set; }
    }

}
