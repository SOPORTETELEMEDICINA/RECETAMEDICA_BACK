namespace RMD.Models.Receta.Detalle.Request
{
    public class DetalleRequest
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public string Descripcion { get; set; }
    }

}
