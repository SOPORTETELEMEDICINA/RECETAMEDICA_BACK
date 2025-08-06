using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Shared.Models.Receta.Header.Responses
{
    public class HeaderAndDetalleResponse
    {
        public HeaderToListResponse Receta { get; set; }
        public List<DetalleRecetaGet> Detalles { get; set; }
    }

}
