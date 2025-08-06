namespace RMD.Shared.Models.Vidal.Tools
{
    public class AlertaModel
    {
        public string AlertType { get; set; } = default!;
        public string Json { get; set; } = default!;
        public string Html { get; set; } = default!;
        public int? ProductId { get; set; }
        public int? CodigoNacional { get; set; }
    }

}
