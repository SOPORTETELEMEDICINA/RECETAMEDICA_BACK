using System.Xml.Linq;

namespace RMD.Models.Consulta
{
    public class PrescriptionResponseXML
    {
        public XDocument XMLResponse { get; set; }
        public List<PrescriptionLineModel> MedicamentoActivo { get; set; } = new();
    }
}
