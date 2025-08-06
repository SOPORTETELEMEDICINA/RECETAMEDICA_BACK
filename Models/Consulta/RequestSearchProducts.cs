namespace RMD.Models.Consulta
{
    public class RequestSearchProducts
    {
        public int IdProduct {  get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public string Summary {  get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int IdVMP{ get; set; }
        public string NameVMP { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;

    }
}
