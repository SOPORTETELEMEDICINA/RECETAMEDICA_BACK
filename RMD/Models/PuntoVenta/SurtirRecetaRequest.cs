namespace RMD.Models.PuntoVenta
{
    public class SurtirRecetaRequest
    {
        public Guid IdReceta { get; set; }
        public List<Guid> DetallesReceta { get; set; } = new();
    }

}
