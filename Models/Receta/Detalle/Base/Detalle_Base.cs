namespace RMD.Models.Receta.Detalle.Base
{
    public class Detalle_Base
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public int UnidadDispensacionId { get; set; }
        public int RutaAdministracionId { get; set; }
        public decimal CantidadDiaria { get; set; }
        public string? Indicacion { get; set; }
        public string? IndicacionNombre { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string? FrecuencyType { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; } = string.Empty;
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public int? PackageQty { get; set; }
        public int? CantidadSurtida { get; set; }
    }

}
