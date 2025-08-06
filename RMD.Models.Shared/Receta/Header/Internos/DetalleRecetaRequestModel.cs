namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class DetalleRecetaRequestModel
    {
        public int IdMedicamento { get; set; }  // ID del medicamento
        public string TipoMedicamento { get; set; } = string.Empty;  // Tipo de medicamento
        public int UnidadDispensacionId { get; set; }  // ID de la unidad de dispensación
        public int RutaAdministracionId { get; set; }  // ID de la ruta de administración
        public string? Indicacion { get; set; }  // Indicación (opcional)
        public string? IndicacionNombre { get; set; }
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string? FrecuencyType { get; set; }  // Tipo de frecuencia
        public string? Observaciones { get; set; }
        public decimal CantidadDiaria { get; set; }  // Cantidad diaria
        public int Duracion { get; set; }  // Duración
        public string UnidadDuracion { get; set; } = string.Empty;  // Unidad de duración
        public DateTime PeriodoInicio { get; set; }  // Fecha de inicio
        public DateTime? PeriodoTerminacion { get; set; }  // Fecha de terminación (opcional)
    }
}
