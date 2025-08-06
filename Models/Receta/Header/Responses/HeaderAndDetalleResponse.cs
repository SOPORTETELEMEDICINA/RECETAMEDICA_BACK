using RMD.Models.Receta.Header.Internos;

namespace RMD.Models.Receta.Header.Responses
{
    public class HeaderAndDetalleResponse
    {
        public HeaderToListResponse Receta { get; set; }
        public List<DetalleRecetaGet> Detalles { get; set; }
    }

}
