namespace RMD.Models.Receta.AlertaToma.Request
{
    public class ActivarAlertaManualRequest
    {
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public decimal CantidadDiaria { get; set; }
        public int UnidadDispensacionId { get; set; }
        public int IdRutaAdministracion { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime FechaHoraPrimerToma { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string? Notas { get; set; }
    }
}
