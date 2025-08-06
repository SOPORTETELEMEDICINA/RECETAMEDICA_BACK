namespace RMD.Models.Receta.AlertaToma.Request
{
    public class ActivarAlertaTomaRequest
    {
        public Guid IdReceta { get; set; }
        public Guid IdDetalleReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public DateTime FechaHoraPrimerToma { get; set; }
    }
}