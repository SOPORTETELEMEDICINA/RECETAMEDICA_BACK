namespace RMD.Models.Recetas
{
    public class RecetaDetalleModel
    {
        public Guid? IdDetalleReceta { get; set; }
        public Guid? IdReceta { get; set; }
        public string Medicamento { get; set; }
        public decimal? CantidadDiaria { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public string Indicacion { get; set; }
        public int? Duracion { get; set; }
        public string UnidadDuracion { get; set; }
        public DateTime? PeriodoInicio { get; set; }
        public DateTime? PeriodoTerminacion { get; set; }
    }

}
