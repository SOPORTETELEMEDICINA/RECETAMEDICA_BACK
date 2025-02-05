namespace RMD.Models.Consulta
{
    public class DetalleReceta
    {
        public Guid IdDetalleReceta { get; set; }
        public Guid IdReceta { get; set; }  // Relación con la receta
        public int MedicamentoId { get; set; }  // ID del medicamento
        public string MedicamentoType { get; set; } = string.Empty;  // Tipo del medicamento
        public int UnidadDispensacionId { get; set; }  // ID de la unidad de dispensación
        public int RutaAdministracionId { get; set; }  // ID de la ruta de administración
        public decimal CantidadDiaria { get; set; }  // Cantidad diaria
        public string? Indicacion { get; set; }  // Indicación (opcional)
        public string? IndicacionNombre { get; set; }
        public string Frecuencia { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; }  // Duración del tratamiento
        public string UnidadDuracion { get; set; } = string.Empty;  // Unidad de duración
        public DateTime PeriodoInicio { get; set; }  // Fecha de inicio
        public DateTime? PeriodoTerminacion { get; set; }  // Fecha de terminación (opcional)
    }
}
