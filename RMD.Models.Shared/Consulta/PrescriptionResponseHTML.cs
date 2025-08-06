namespace RMD.Shared.Models.Consulta
{
    public class PrescriptionResponseHTML
    {
        public string HtmlResponse { get; set; } = string.Empty;
        public List<PrescriptionLineModel> MedicamentoActivo { get; set; } = new();
    }
}
