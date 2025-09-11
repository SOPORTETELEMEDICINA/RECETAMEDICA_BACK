namespace RMD.Shared.Models.Receta.Header.Internos
{
    public sealed class DetalleParaFarmaciaInHeader
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }

        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; } = string.Empty;

        public string DescripcionParaFarmacia { get; set; } = string.Empty;

        public decimal CantidadDiaria { get; set; }
        public int UnidadDispensacionId { get; set; }
        public int RutaAdministracionId { get; set; }

        public string? Indicacion { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; } = string.Empty;

        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
        public DateTime? FechaSurtido { get; set; }

        public string? IndicacionNombre { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string? Observaciones { get; set; }

    }
}
