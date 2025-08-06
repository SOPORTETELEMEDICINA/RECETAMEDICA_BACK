namespace RMD.Models.Consulta
{
    /// <summary>
    /// Representa una línea de prescripción de un paciente.
    /// </summary>
    public class PrescriptionLineModel
    {
        /// <summary>
        /// Identificador de la receta.
        /// </summary>
        public Guid IdReceta { get; set; }

        /// <summary>
        /// Identificador del paciente.
        /// </summary>
        public Guid IdPaciente { get; set; }

        /// <summary>
        /// Identificador del detalle de la receta.
        /// </summary>
        public Guid IdDetalleReceta { get; set; }

        /// <summary>
        /// Tipo de fármaco (por ejemplo, "PRODUCT", "VMP").
        /// </summary>
        public string DrugType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico del fármaco.
        /// </summary>
        public int Drug { get; set; }

        /// <summary>
        /// Dosis diaria.
        /// </summary>
        public int Dose { get; set; }

        /// <summary>
        /// Identificador de la unidad de dosis.
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Tipo de frecuencia de administración.
        /// </summary>
        public int Frecuency { get; set; }
        public int IdFrecuencyType { get; set; }
        public string FrequencyType { get; set; } = string.Empty;

        /// <summary>
        /// Duración del tratamiento.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Unidad de medida de la duración (por ejemplo, "DAYS", "WEEKS").
        /// </summary>
        public string DurationType { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la ruta de administración.
        /// </summary>
        public int Route { get; set; }

        /// <summary>
        /// Indicación opcional asociada a la línea de prescripción.
        /// </summary>
        public string? Indication { get; set; }
    }
}
