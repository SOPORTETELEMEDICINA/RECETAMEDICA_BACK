namespace RMD.Models.PuntoVenta
{
    public class SurtirRecetaRequest
    {
        public Guid IdReceta { get; set; }
        public List<DetalleSurtidoRequest> DetallesReceta { get; set; } = new();
    }
    public class DetalleSurtidoRequest
    {
        public Guid IdDetalleReceta { get; set; }
        public int MedicamentoId { get; set; }
        public int PiezasSurtidas { get; set; }
    }
}
