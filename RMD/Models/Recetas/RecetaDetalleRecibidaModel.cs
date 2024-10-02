namespace RMD.Models.Recetas
{
    public class RecetaDetalleRecibidaModel
    {
        public Guid IdDetalleReceta { get; set; } = Guid.Empty; // Se genera en el controlador si no se proporciona
        public Guid? IdReceta { get; set; } // Relación con la receta principal, se asigna en el controlador
        public string Medicamento { get; set; }
        public decimal CantidadDiaria { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public string Indicacion { get; set; }
        public int Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
    }
}
