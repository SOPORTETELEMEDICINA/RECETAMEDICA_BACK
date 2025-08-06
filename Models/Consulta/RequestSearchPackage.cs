namespace RMD.Models.Consulta
{
    public class RequestSearchPackage
    {
        public int IdPackage { get; set; } // T1.IdPackage
        public string NamePackage { get; set; } = string.Empty; // T1.[Name] AS NamePackage
        public string SummaryPackage { get; set; } = string.Empty; // T1.Summary AS SummaryPackage
        public string ShortLabel { get; set; } = string.Empty; // T1.ShortLabel
        public string CompanyName { get; set; } = string.Empty; // T1.CompanyName
        public int IdProduct { get; set; } // T1.ProductId AS IdProduct
        public string NameProduct { get; set; } = string.Empty; // T2.[Name] AS NameProduct
        public string SummaryProduct { get; set; } = string.Empty; // T2.Summary AS SummaryProduct
        public int? IdVmp { get; set; } // T2.IdVmp
        public string NameVMP { get; set; } = string.Empty; // T3.[Name] AS NameVMP
        public string ActivePrinciples { get; set; } = string.Empty; // T3.ActivePrinciples
        public string GalenicForm { get; set; } = string.Empty; // T3.GalenicForm
        public int? IdVTM { get; set; } // T3.IdVTM
        public string NameVTM { get; set; } = string.Empty; // T4.[Name] AS NameVTM
        public int? IdDrug { get; set; } // T1.DrugId AS IdDrug
        public string Cip13 { get; set; } = string.Empty; // T1.Cip13
        public int? UcdId { get; set; } // T1.UcdId
        public string VmpDescription { get; set; } = string.Empty; // T5.VmpDescription
    }
}
