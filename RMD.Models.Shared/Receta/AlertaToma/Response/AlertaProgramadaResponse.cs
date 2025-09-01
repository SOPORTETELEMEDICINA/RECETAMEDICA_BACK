namespace RMD.Shared.Models.Receta.AlertaToma.Response
{
    public class AlertaProgramadaResponse
    {
        public Guid IdAlertaProgramada { get; set; }
        public Guid IdAlerta { get; set; }
        public int TipoAlerta { get; set; } // 1 = Normal, 2 = Manual
        public Guid IdPaciente { get; set; }
        public Guid? IdReceta { get; set; }
        public Guid? IdDetalleReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public string Medicamento { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaToma { get; set; }

    }
}