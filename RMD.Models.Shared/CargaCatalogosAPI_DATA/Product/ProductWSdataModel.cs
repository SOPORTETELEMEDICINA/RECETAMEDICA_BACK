using System.Text.Json.Serialization;

namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.Product
{
  

    public class ProductWSdataModel
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; }

        [JsonPropertyName("galenicName")]
        public string GalenicName { get; set; }

        [JsonPropertyName("atccode")]
        public string AtcCode { get; set; }

        [JsonPropertyName("atccodemj")]
        public string AtcCodeMJ { get; set; }

        [JsonPropertyName("pictogram")]
        public int? Pictogram { get; set; }

        [JsonPropertyName("isnarcotic")]
        public string IsNarcotic { get; set; }

        [JsonPropertyName("psicoannexid")]
        public string PsicoAnnexId { get; set; }

        [JsonPropertyName("monodrug")]
        public string Monodrug { get; set; }

        [JsonPropertyName("perfus_ml")]
        public string Perfus_Ml { get; set; }

        [JsonPropertyName("admin_gota")]
        public string Admin_Gota { get; set; }

        [JsonPropertyName("orphan")]
        public string Orphan { get; set; }

        [JsonPropertyName("biosimilar")]
        public string Biosimilar { get; set; }

        [JsonPropertyName("norep")]
        public string NoRep { get; set; }

        [JsonPropertyName("noreptypeid")]
        public string NoRepTypeId { get; set; }

        [JsonPropertyName("no_comp")]
        public string No_Comp { get; set; }

        [JsonPropertyName("parallel_import")]
        public string Parallel_Import { get; set; }

        [JsonPropertyName("radiopharmaceutical")]
        public string Radiopharmaceutical { get; set; }

        [JsonPropertyName("mar")]
        public string Mar { get; set; }

        [JsonPropertyName("mar_pac")]
        public string Mar_Pac { get; set; }
    }

}

