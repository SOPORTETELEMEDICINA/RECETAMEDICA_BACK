namespace RMD.Models.Recetas
{
    public class DetalleRecetaRequest
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public string Descripcion { get; set; }
    }

}
