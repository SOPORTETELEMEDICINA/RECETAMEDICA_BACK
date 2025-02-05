namespace RMD.Models.Consulta
{
    public class PrescriptionLineModel
    {
        public string DrugType { get; set; } = string.Empty; // Tipo de fármaco
        public int Drug { get; set; } // ID del fármaco
        public int Dose { get; set; } // Dosis
        public int UnitId { get; set; } // ID de la unidad
        public string FrequencyType { get; set; }// Tipo de frecuencia
        public int Duration { get; set; } // Duración
        public string DurationType { get; set; } = string.Empty; // Tipo de duración
        public int Route { get; set; } // Ruta (entero)
        public string? Indication { get; set; } // Indicación (opcional)
    }
}
