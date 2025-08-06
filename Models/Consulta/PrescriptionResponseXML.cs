namespace RMD.Models.Consulta
{
    public class PrescriptionResponseXML
    {
        public string XMLResponse { get; set; }
        public List<PrescriptionLineModel> MedicamentoActivo { get; set; } = new();
    }
}
