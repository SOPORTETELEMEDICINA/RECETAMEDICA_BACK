namespace RMD.Models.Receta.Header.Internos
{
    public class DetalleCronico
    {
        public Guid Id { get; set; }
        public Guid IdRecetaOrigen { get; set; }
        public Guid? IdRecetaNueva { get; set; }

        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; }
        public int UnidadDispensacionId { get; set; }
        public int RutaAdministracionId { get; set; }
        public decimal CantidadDiaria { get; set; }
        public string? Indicacion { get; set; }
        public string? IndicacionNombre { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public bool? IsNarcotic { get; set; }
        public string? PsicoAnnexId { get; set; }
    }

}