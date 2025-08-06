using System.Text.Json.Serialization;

namespace RMD.Models.CargaCatalogosAPI_DATA
{
    public class ParamInfo
    {
        [JsonPropertyName("tableName")]
        public string TableName { get; set; }

        [JsonPropertyName("columnNameList")]
        public string ColumnNameList { get; set; }

        [JsonPropertyName("orderBy")]
        public string OrderBy { get; set; }

        [JsonPropertyName("c")]
        public string C { get; set; }

        [JsonPropertyName("numMax")]
        public string NumMax { get; set; }

        [JsonPropertyName("page")]
        public string Page { get; set; }

        [JsonPropertyName("nbpage")]
        public string Nbpage { get; set; }

        [JsonPropertyName("contains")]
        public string Contains { get; set; }

        [JsonPropertyName("tk")]
        public string Tk { get; set; }
    }
}
