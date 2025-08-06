namespace RMD.Models.PuntoVenta
{
    public class PuntoVentaRecetaResponse
    {
        public PuntoVentaRecetaModel Receta { get; set; }
        public List<DetalleRecetaModel> Detalles { get; set; }
    }
}
