namespace RMD.Models.Consulta
{
    public class RecetaGetRequest
    {
        public RecetaGet Receta { get; set; }
        public List<DetalleRecetaGet> Detalles { get; set; }
    }

}
