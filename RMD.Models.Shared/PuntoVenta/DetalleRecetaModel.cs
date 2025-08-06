namespace RMD.Shared.Models.PuntoVenta
{

    public class DetalleRecetaModel
    {
        public Guid IdDetalleReceta { get; set; } // Identificador único del detalle de la receta
        public Guid IdReceta { get; set; }        // Identificador de la receta asociada
        public string MedicamentoType { get; set; } // Tipo de medicamento (PRODUCT, PACKAGE, VMP, UCD, UCDV)
        public int MedicamentoId { get; set; }    // Identificador del medicamento
        public string MedicamentoNombre { get; set; } // Nombre o descripción del medicamento según el tipo
        public int Piezas { get; set; } // Cantidad de piezas del medicamento (opcional, puede ser 0 si no aplica)

        // Unidad de dispensación
        public int UnidadDispensacionId { get; set; } // ID de la unidad de dispensación
        public string UnidadDispensacion { get; set; } // Nombre de la unidad de dispensación

        // Ruta de administración
        public int RutaAdministracionId { get; set; } // ID de la ruta de administración
        public string RutaAdministracion { get; set; } // Nombre de la ruta de administración

        public decimal CantidadDiaria { get; set; } // Cantidad diaria de medicamento
        public string Indicacion { get; set; } // Instrucciones o indicaciones para el medicamento
        public string? IndicacionNombre { get; set; }
        public int Frecuency { get; set; }
        public string FrecuencyType { get; set; }
        public string? Observaciones { get; set; }
        public int Duracion { get; set; } // Duración del tratamiento en días
        public string UnidadDuracion { get; set; } // Unidad de duración (e.g., días, semanas, meses)
        public DateTime PeriodoInicio { get; set; } // Fecha de inicio del tratamiento
        public DateTime? PeriodoTerminacion { get; set; } // Fecha de terminación del tratamiento

        public bool Surtido { get; set; } // Estado de si fue surtido o no
    }

}
