namespace RMD.Models.Consulta
{
    public class DetalleRecetaResponse
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public Guid IdPaciente { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public string Medicamento { get; set; }
        public string Descripcion { get; set; }
    }

}
