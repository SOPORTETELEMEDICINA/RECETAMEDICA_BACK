using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Shared.Models.Receta.Header.Responses
{
    public class HeaderUpdateResponse
    {
        public Header_UpdatePacienteParsedRequest Receta { get; set; }
        public List<Detalle_UpdateResponse> Detalles { get; set; }
    }
}
