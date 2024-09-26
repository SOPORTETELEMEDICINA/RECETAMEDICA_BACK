namespace RMD.Models.Consulta
{
    public class PrescriptionLineModel
    {
        public string Drug { get; set; } = string.Empty;
        public int Dose { get; set; }
        public int UnitId { get; set; }
        public string FrequencyType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DurationType { get; set; } = string.Empty;
    }
}
