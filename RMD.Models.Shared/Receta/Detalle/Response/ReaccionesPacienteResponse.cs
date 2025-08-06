namespace RMD.Shared.Models.Receta.Detalle.Response
{
    public class ReaccionesPacienteResponse
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
