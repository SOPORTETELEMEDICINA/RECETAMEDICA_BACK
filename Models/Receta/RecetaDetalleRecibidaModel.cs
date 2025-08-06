//namespace RMD.Models.Recetas
//{
//    public class RecetaDetalleRecibidaModel
//    {
//        public Guid IdDetalleReceta { get; set; } = Guid.Empty; // Se genera en el controlador si no se proporciona
//        public Guid? IdReceta { get; set; } // Relación con la receta principal, se asigna en el controlador
//        public int MedicamentoId { get; set; }
//        public string MedicamentoType { get; set; }
//        public decimal CantidadDiaria { get; set; }
//        public int UnidadDispensacionId { get; set; }
//        public int RutaAdministracionId { get; set; }
//        public string Indicacion { get; set; } // Instrucciones o indicaciones para el medicamento
//        public string? IndicacionNombre { get; set; }
//        public string Frecuencia { get; set; }
//        public string? Observaciones { get; set; }
//        public int Duracion { get; set; }
//        public string UnidadDuracion { get; set; }
//        public DateTime PeriodoInicio { get; set; }
//        public DateTime? PeriodoTerminacion { get; set; }
//    }
//}
