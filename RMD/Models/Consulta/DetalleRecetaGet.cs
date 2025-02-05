namespace RMD.Models.Consulta
{
    public class DetalleRecetaGet
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }
        public string MedicamentoType { get; set; }
        public int MedicamentoId { get; set; }
        public string Medicamento { get; set; }
        public decimal CantidadDiaria { get; set; }
        public int UnidadDispensacionId { get; set; }
        public string UnidadDispensacion { get; set; }
        public int RutaAdministracionId { get; set; }
        public string RutaAdministracion { get; set; }
        public string Indicacion { get; set; }
        public string? IndicacionNombre { get; set; }
        public string Frecuencia { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public bool Surtido { get; set; }
    }

}
