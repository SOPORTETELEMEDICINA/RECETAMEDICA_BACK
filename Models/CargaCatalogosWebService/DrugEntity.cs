using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class DrugEntity
    {
        [JsonProperty("drugEntityId")]
        public string DrugEntityId { get; set; }

        [JsonProperty("entityTypeId")]
        public string EntityTypeId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("formId")]
        public string FormId { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("classdcId")]
        public string ClassdcId { get; set; }

        [JsonProperty("CommonnameGroupId")]
        public string CommonnameGroupId { get; set; }

        [JsonProperty("divisibilityValue")]
        public string DivisibilityValue { get; set; }
    }
}
