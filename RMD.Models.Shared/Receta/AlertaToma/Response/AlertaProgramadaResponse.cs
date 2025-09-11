namespace RMD.Shared.Models.Receta.AlertaToma.Response
{
    public class AlertaProgramadaResponse
    {
        public Guid IdAlerta { get; set; }
        public int TipoAlerta { get; set; } // 1 = Normal, 2 = Manual
        public Guid IdPaciente { get; set; }
        public Guid? IdReceta { get; set; }
        public Guid? IdDetalleReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public DateTime FechaHoraPrimerToma { get; set; }
        public DateTime FechaHoraUltimaToma { get; set; }
        public int? IdRutaAdministracion { get; set; }
        public decimal? TotalDosis { get; set; }
        public bool Activo { get; set; }
    }
}
