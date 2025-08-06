using Newtonsoft.Json;

namespace RMD.Models.CargaCatalogosWebService
{
    public class ParamInfo
    {
        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("columnNameList")]
        public string ColumnNameList { get; set; }

        [JsonProperty("orderBy")]
        public string OrderBy { get; set; }

        [JsonProperty("c")]
        public string C { get; set; }

        [JsonProperty("numMax")]
        public string NumMax { get; set; }

        [JsonProperty("page")]
        public string Page { get; set; }

        [JsonProperty("nbpage")]
        public string Nbpage { get; set; }

        [JsonProperty("contains")]
        public string Contains { get; set; }

        [JsonProperty("tk")]
        public string Tk { get; set; }
    }
}
