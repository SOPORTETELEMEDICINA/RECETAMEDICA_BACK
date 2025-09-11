namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class DetalleParaFarmaciaRecetaRequestModel
    {
        public Guid? IdDetalleReceta { get; set; }  // Puede coincidir con Detalle.IdDetalleReceta o generarse en el SP

        public int IdMedicamento { get; set; }  // ID del medicamento
        public string TipoMedicamento { get; set; } = string.Empty;  // Tipo de medicamento (ej. VMP, CNIS, etc.)

        public string DescripcionParaFarmacia { get; set; } = string.Empty;  // Texto específico para farmacia

        public decimal CantidadDiaria { get; set; }  // Cantidad diaria
        public int UnidadDispensacionId { get; set; }  // Unidad de dispensación
        public int RutaAdministracionId { get; set; }  // Ruta de administración

        public string? Indicacion { get; set; }  // Indicación (opcional)
        public int Duracion { get; set; }  // Duración
        public string UnidadDuracion { get; set; } = string.Empty;  // Unidad de duración

        public DateTime PeriodoInicio { get; set; }  // Fecha inicio
        public DateTime? PeriodoTerminacion { get; set; }  // Fecha terminación (opcional)
        public DateTime? FechaSurtido { get; set; }  // Fecha de surtido (opcional)

        public string? IndicacionNombre { get; set; }  // Nombre de indicación
        public int Frecuency { get; set; }  // Frecuencia
        public int IdFrecuencyType { get; set; }  // ID tipo frecuencia
        public string? Observaciones { get; set; }  // Observaciones (opcional)
    }
}
